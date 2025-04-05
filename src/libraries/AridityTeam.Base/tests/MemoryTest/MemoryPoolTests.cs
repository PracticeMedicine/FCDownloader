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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AridityTeam.Base.ProcessUtil;
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("xUnit", "xUnit1031", Justification = "i need to wait for the tasks in TestMemoryPool_ThreadSafety")]

namespace AridityTeam.Base.Tests.MemoryTest
{
    public class MemoryPoolTests
    {
        [Fact]
        public void TestMemoryPool_RentAndReturn()
        {
            var memory = new Memory(); // Initialize with a block size of 1024 bytes

            // Rent a block from the pool
            Assert.IsTrue(memory.UncheckedMalloc(1024, out var ptr1));
            Assert.AreNotEqual(nint.Zero, ptr1);

            // Return the block to the pool
            memory.Free(ptr1);

            // Rent another block (should reuse the returned block)
            Assert.IsTrue(memory.UncheckedMalloc(1024, out var ptr2));
            Assert.AreEqual(ptr1, ptr2); // Verify that the same block is reused
            memory.Free(ptr2);
        }

        [Fact]
        public void TestMemoryPool_Clear()
        {
            var memory = new Memory(); // Initialize with a block size of 1024 bytes

            // Rent a block from the pool
            Assert.IsTrue(memory.UncheckedMalloc(1024, out var ptr));
            Assert.AreNotEqual(nint.Zero, ptr);

            // Clear the pool
            memory.ClearPool();

            // Verify that the block is no longer in the pool
            Assert.IsTrue(memory.UncheckedMalloc(1024, out var newPtr));
            Assert.AreNotEqual(ptr, newPtr); // A new block should be allocated
            memory.Free(newPtr);
        }

        [Fact]
        public void TestMemoryPool_LargeAllocation()
        {
            var memory = new Memory(); // Initialize with a block size of 1024 bytes

            // Allocate a block larger than the pool block size
            Assert.IsTrue(memory.UncheckedMalloc(2048, out var ptr));
            Assert.AreNotEqual(nint.Zero, ptr);

            // Free the block
            memory.Free(ptr);
        }

        [Fact]
        public void TestMemoryPool_ThreadSafety()
        {
            var memory = new Memory(); // Initialize with a block size of 1024 bytes
            var lockObject = new object();

            // Simulate multiple threads accessing the pool
            Task[] tasks = new Task[10];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    lock (lockObject)
                    {
                        Assert.IsTrue(memory.UncheckedMalloc(1024, out var ptr));
                        Assert.AreNotEqual(nint.Zero, ptr);

                        // Simulate some work
                        for (var j = 0; j < 1024; j++)
                        {
                            Marshal.WriteByte(ptr, j, (byte)(j % 256));
                        }

                        memory.Free(ptr);
                    }
                });
            }

            Task.WaitAll(tasks);
        }
    }
}
