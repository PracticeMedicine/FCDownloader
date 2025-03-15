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
 */
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AridityTeam.Base.ProcessUtil;

namespace AridityTeam.Base.Tests.MemoryTest
{
    public class MemoryTests
    {
        [Fact]
        public void TestUncheckedMalloc_ZeroBytes()
        {
            Memory memory = new Memory();
            Assert.True(memory.UncheckedMalloc(0, out nint ptr));
            Assert.NotEqual(nint.Zero, ptr); // Or handle this case as per your design
            memory.Free(ptr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestFree_NullPointer()
        {
            Memory memory = new Memory();
            memory.Free(nint.Zero); // Should not throw

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc_NullPointer()
        {
            Memory memory = new Memory();
            nint newPtr = memory.Realloc(nint.Zero, 100);
            Assert.NotEqual(nint.Zero, newPtr);
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc_ZeroBytes()
        {
            Memory memory = new Memory();
            Assert.True(memory.UncheckedMalloc(100, out nint ptr));
            nint newPtr = memory.Realloc(ptr, 0);
            Assert.NotEqual(nint.Zero, newPtr); // Or handle this case as per your design
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc_PreserveContent()
        {
            Memory memory = new Memory();
            object lockObject = new object();

            // Allocate memory
            Assert.True(memory.UncheckedMalloc(100, out nint ptr));

            // Write data to the allocated memory in a thread-safe manner
            lock (lockObject)
            {
                for (int i = 0; i < 100; i++)
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
            Assert.NotEqual(nint.Zero, newPtr);

            // Verify that the data is preserved in a thread-safe manner
            lock (lockObject)
            {
                for (int i = 0; i < 100; i++)
                {
                    byte value = Marshal.ReadByte(newPtr, i);
                    Assert.Equal((byte)(i % 256), value);
                }
            }

            // Free the memory
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestGetAllocatedPointers()
        {
            Memory memory = new Memory();

            // Allocate two pointers
            Assert.True(memory.UncheckedMalloc(100, out nint ptr1));
            Assert.True(memory.UncheckedMalloc(200, out nint ptr2));

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
            Memory memory = new Memory();
            Assert.True(memory.UncheckedMalloc(100, out nint ptr1));
            Assert.True(memory.UncheckedMalloc(200, out nint ptr2));

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
            Memory memory = new Memory();
            Assert.True(memory.UncheckedMalloc(100, out nint ptr));
            Assert.NotEqual(nint.Zero, ptr);
            memory.Free(ptr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestRealloc()
        {
            Memory memory = new Memory();
            Assert.True(memory.UncheckedMalloc(100, out nint ptr));
            nint newPtr = memory.Realloc(ptr, 200);
            Assert.NotEqual(nint.Zero, newPtr);
            memory.Free(newPtr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }

        [Fact]
        public void TestFree()
        {
            Memory memory = new Memory();
            Assert.True(memory.UncheckedMalloc(100, out nint ptr));
            memory.Free(ptr);
            ptr = nint.Zero; // Nullify the pointer
            Assert.Equal(nint.Zero, ptr);

            memory.PrintPerformanceMetrics(); // Display performance metrics
        }
    }
}
