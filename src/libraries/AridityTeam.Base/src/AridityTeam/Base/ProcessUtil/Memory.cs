/*
 * Copyright (c) 2025 The Aridity Team
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AridityTeam.Base.ProcessUtil
{
    /// <summary>
    /// Provides methods for unmanaged memory allocation and deallocation.
    /// </summary>
    public class Memory : IMemory
    {
        private readonly RemovableConcurrentBag<IntPtr> _allocatedPointers = [];
        private readonly MemoryPool _memoryPool;
        private readonly ConcurrentDictionary<IntPtr, int> _blockSizes = new();

        // Performance tracking data
        private readonly List<long> _allocationTimes = [];
        private readonly List<long> _deallocationTimes = [];
        private readonly List<long> _reallocationTimes = [];
        private long _totalAllocatedBytes;
        private readonly long _totalFreedBytes = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory"/> class with a specified block size for the memory pool.
        /// </summary>
        /// <param name="blockSize">The size of each block in the memory pool.</param>
        public Memory(int blockSize = 1024, int initialBlockCount = 10) // Default block size is 1024 bytes
        {
            _memoryPool = new MemoryPool(blockSize, initialBlockCount);
        }

        /// <summary>
        /// Allocates a block of memory of the specified size.
        /// </summary>
        /// <param name="size">The size of the memory block to allocate, in bytes.</param>
        /// <param name="result">An output parameter that will hold the pointer to the allocated memory.</param>
        /// <returns>True if the allocation was successful; otherwise, false.</returns>
        public bool UncheckedMalloc(int size, out IntPtr result)
        {
            var stopwatch = Stopwatch.StartNew();

            if (size <= _memoryPool.BlockSize)
            {
                result = _memoryPool.Rent(); // Rent a block from the pool
                if (result != IntPtr.Zero)
                {
                    _allocatedPointers.Add(result); // Track the allocated pointer
                    _blockSizes[result] = size; // Track the size of the allocated block
                }
                return result != IntPtr.Zero;
            }
            result = Marshal.AllocHGlobal(size); // Allocate a new block if the size is larger than the pool block size

            stopwatch.Stop();

            if (result != IntPtr.Zero)
            {
                _allocatedPointers.Add(result); // Track the allocated pointer
                _blockSizes[result] = size; // Track the size of the allocated block
                _totalAllocatedBytes += size;
                _allocationTimes.Add(stopwatch.ElapsedTicks);
            }
            return result != IntPtr.Zero;
        }

        public IntPtr AlignedMalloc(int size, int alignment)
        {
            var ptr = Marshal.AllocHGlobal(size + alignment);
            var alignedPtr = (IntPtr)((long)(ptr) + alignment - 1 & ~(alignment - 1));
            return alignedPtr;
        }

        /// <summary>
        /// Frees a block of memory that was previously allocated.
        /// </summary>
        /// <param name="ptr">The pointer to the memory block to free.</param>
        public void Free(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var stopwatch = Stopwatch.StartNew();
                if (_allocatedPointers.Remove(ptr)) // Remove from tracking if it was allocated from the pool
                {
                    _memoryPool.Return(ptr); // Return the block to the pool for reuse
                }
                else
                {
                    Marshal.FreeHGlobal(ptr); // Free the block if it was not from the pool
                    _blockSizes.TryRemove(ptr, out _); // Remove the size tracking
                }
                stopwatch.Stop();
                _deallocationTimes.Add(stopwatch.ElapsedTicks);
            }
        }

        /// <summary>
        /// Reallocates a block of memory to a new size.
        /// </summary>
        /// <param name="ptr">The pointer to the memory block to reallocate.</param>
        /// <param name="newSize">The new size of the memory block, in bytes.</param>
        /// <returns>A pointer to the reallocated memory block.</returns>
        public IntPtr Realloc(IntPtr ptr, int newSize)
        {
            var stopwatch = Stopwatch.StartNew();
            IntPtr newPtrResult;

            if (ptr == IntPtr.Zero)
            {
                return UncheckedMalloc(newSize, out newPtrResult) ? newPtrResult : IntPtr.Zero;
            }

            if (UncheckedMalloc(newSize, out newPtrResult))
            {
                // Copy old memory to new memory
                var oldSize = _blockSizes[ptr]; // Get the size of the old block
                for (var i = 0; i < Math.Min(oldSize, newSize); i++)
                {
                    Marshal.WriteByte(newPtrResult, i, Marshal.ReadByte(ptr, i));
                }
                Free(ptr); // Free the old memory
            }
            stopwatch.Stop();
            _reallocationTimes.Add(stopwatch.ElapsedTicks);
            return newPtrResult;
        }

        /// <summary>
        /// Frees all the currently allocated block of memory.
        /// </summary>
        public void FreeAll()
        {
            if (_allocatedPointers == null)
                return;

            foreach (var ptr in _allocatedPointers.GetAll())
            {
                Free(ptr); // Use the Free method to handle both pool and unmanaged memory
            }
            _allocatedPointers.Clear(); // Clear the tracking collection
        }

        /// <summary>
        /// Get all allocated memory pointers.
        /// </summary>
        /// <returns>An enumerable collection of allocated pointers.</returns>
        public IEnumerable<IntPtr> GetAllocatedPointers()
        {
            return _allocatedPointers.GetAll(); // Return all tracked pointers
        }

        /// <summary>
        /// Clears the memory pool, freeing all blocks in the pool.
        /// </summary>
        public void ClearPool()
        {
            _memoryPool.Clear(); // Free all blocks in the pool
        }

        // Performance reporting methods
        public void PrintPerformanceMetrics()
        {
            Console.WriteLine("Performance Metrics:");
            Console.WriteLine($"Total Allocated Bytes: {_totalAllocatedBytes}");
            Console.WriteLine($"Total Freed Bytes: {_totalFreedBytes}");
            Console.WriteLine($"Average Allocation Time: {GetAverage(_allocationTimes)} ticks");
            Console.WriteLine($"Average Deallocation Time: {GetAverage(_deallocationTimes)} ticks");
            Console.WriteLine($"Average Reallocation Time: {GetAverage(_reallocationTimes)} ticks");
        }

        private long GetAverage(List<long> times)
        {
            return times.Count > 0 ? (long)times.Average() : 0;
        }
    }
}
