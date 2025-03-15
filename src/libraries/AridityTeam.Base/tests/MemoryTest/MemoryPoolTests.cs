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
    public class MemoryPoolTests
    {
        [Fact]
        public void TestMemoryPool_RentAndReturn()
        {
            Memory memory = new Memory(1024); // Initialize with a block size of 1024 bytes

            // Rent a block from the pool
            Assert.True(memory.UncheckedMalloc(1024, out nint ptr1));
            Assert.NotEqual(nint.Zero, ptr1);

            // Return the block to the pool
            memory.Free(ptr1);

            // Rent another block (should reuse the returned block)
            Assert.True(memory.UncheckedMalloc(1024, out nint ptr2));
            Assert.Equal(ptr1, ptr2); // Verify that the same block is reused
            memory.Free(ptr2);
        }

        [Fact]
        public void TestMemoryPool_Clear()
        {
            Memory memory = new Memory(1024); // Initialize with a block size of 1024 bytes

            // Rent a block from the pool
            Assert.True(memory.UncheckedMalloc(1024, out nint ptr));
            Assert.NotEqual(nint.Zero, ptr);

            // Clear the pool
            memory.ClearPool();

            // Verify that the block is no longer in the pool
            Assert.True(memory.UncheckedMalloc(1024, out nint newPtr));
            Assert.NotEqual(ptr, newPtr); // A new block should be allocated
            memory.Free(newPtr);
        }

        [Fact]
        public void TestMemoryPool_LargeAllocation()
        {
            Memory memory = new Memory(1024); // Initialize with a block size of 1024 bytes

            // Allocate a block larger than the pool block size
            Assert.True(memory.UncheckedMalloc(2048, out nint ptr));
            Assert.NotEqual(nint.Zero, ptr);

            // Free the block
            memory.Free(ptr);
        }

        [Fact]
        public void TestMemoryPool_ThreadSafety()
        {
            Memory memory = new Memory(1024); // Initialize with a block size of 1024 bytes
            object lockObject = new object();

            // Simulate multiple threads accessing the pool
            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    lock (lockObject)
                    {
                        Assert.True(memory.UncheckedMalloc(1024, out nint ptr));
                        Assert.NotEqual(nint.Zero, ptr);

                        // Simulate some work
                        for (int j = 0; j < 1024; j++)
                        {
                            Marshal.WriteByte(ptr, j, (byte)(j % 256));
                        }

                        memory.Free(ptr);
                    }
                });
            }

            // Wait for all tasks to complete
            Task.WaitAll(tasks);
        }
    }
}
