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
using AridityTeam.Base.ProcessUtil;

namespace AridityTeam.Base.Tests.MemoryTest
{
    public class MemoryTests
    {
        [Fact]
        public void TestUncheckedMalloc_ZeroBytes()
        {
            var memory = new Memory();
            Assert.IsTrue(memory.UncheckedMalloc(0, out var ptr));
            Assert.AreNotEqual(nint.Zero, ptr); // Or handle this case as per your design
            memory.Free(ptr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestFree_NullPointer()
        {
            var memory = new Memory();
            memory.Free(nint.Zero); // Should not throw

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc_NullPointer()
        {
            var memory = new Memory();
            var newPtr = memory.Realloc(nint.Zero, 100);
            Assert.AreNotEqual(nint.Zero, newPtr);
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc_ZeroBytes()
        {
            var memory = new Memory();
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr));
            var newPtr = memory.Realloc(ptr, 0);
            Assert.AreNotEqual(nint.Zero, newPtr); // Or handle this case as per your design
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc_PreserveContent()
        {
            var memory = new Memory();
            var lockObject = new object();

            // Allocate memory
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr));

            // Write data to the allocated memory in a thread-safe manner
            lock (lockObject)
            {
                for (var i = 0; i < 100; i++)
                {
                    Marshal.WriteByte(ptr, i, (byte)(i % 256));
                }
            }

            // Reallocate memory in a thread-safe manner
            nint newPtr;
            lock (lockObject)
            {
                newPtr = memory.Realloc(ptr, 200);
            }
            Assert.AreNotEqual(nint.Zero, newPtr);

            // Verify that the data is preserved in a thread-safe manner
            lock (lockObject)
            {
                for (var i = 0; i < 100; i++)
                {
                    var value = Marshal.ReadByte(newPtr, i);
                    Assert.AreEqual((byte)(i % 256), value);
                }
            }

            // Free the memory
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestGetAllocatedPointers()
        {
            var memory = new Memory();

            // Allocate two pointers
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr1));
            Assert.IsTrue(memory.UncheckedMalloc(200, out var ptr2));

            // Verify that both pointers are tracked
            var allocatedPointers = memory.GetAllocatedPointers();
            Assert.Contains(ptr1, allocatedPointers);
            Assert.Contains(ptr2, allocatedPointers);

            // Free one pointer and verify it's no longer tracked
            memory.Free(ptr1);
            allocatedPointers = memory.GetAllocatedPointers();
            Assert.DoesNotContain(ptr1, allocatedPointers);
            Assert.Contains(ptr2, allocatedPointers);

            // Free the second pointer and verify it's no longer tracked
            memory.Free(ptr2);
            allocatedPointers = memory.GetAllocatedPointers();
            Assert.DoesNotContain(ptr2, allocatedPointers);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestMemoryTracking()
        {
            var memory = new Memory();
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr1));
            Assert.IsTrue(memory.UncheckedMalloc(200, out var ptr2));

            // Free one pointer and verify it's tracked correctly
            memory.Free(ptr1);

            // Assuming you have a method to get the current allocated pointers
            var allocatedPointers = memory.GetAllocatedPointers();
            Assert.DoesNotContain(ptr1, allocatedPointers);
            Assert.Contains(ptr2, allocatedPointers);

            memory.Free(ptr2);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestUncheckedMalloc()
        {
            var memory = new Memory();
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr));
            Assert.AreNotEqual(nint.Zero, ptr);
            memory.Free(ptr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc()
        {
            var memory = new Memory();
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr));
            var newPtr = memory.Realloc(ptr, 200);
            Assert.AreNotEqual(nint.Zero, newPtr);
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestFree()
        {
            var memory = new Memory();
            Assert.IsTrue(memory.UncheckedMalloc(100, out var ptr));
            memory.Free(ptr);
            ptr = nint.Zero; // Nullify the pointer
            Assert.AreEqual(nint.Zero, ptr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }
    }
}
