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
using System.Diagnostics;

namespace AridityTeam.Base.Util
{
    public static class MemoryUtil
    {
        private static AridLogger _logger = new AridLogger(typeof(MemoryUtil));

        public static void CheckForMemoryLeaks(double maxMemUsage)
        {
            HeartbeatInstance instance = new HeartbeatInstance()
            {
                InstanceName = "Aridity Base Memory Checker",
                HeartbeatTime = 3500,
                ActionToRun = () =>
                {
                    _logger.Debug("Current memory usage: {0}mb", maxMemUsage);
                    if (GetMemoryUsageInMB() > maxMemUsage)
                    {
                        throw new PerformanceException("Exceeded max memory limit.");
                    }
                },
            };

            HeartbeatManager.Instance.AddInstance(instance);
        }

        public static double GetMemoryUsageInMB()
        {
            // Get the current process
            Process currentProcess = Process.GetCurrentProcess();

            // Get the memory usage in bytes
            long memoryUsageInBytes = currentProcess.WorkingSet64; // or use PrivateMemorySize64

            // Convert bytes to megabytes
            double memoryUsageInMegabytes = memoryUsageInBytes / (1024.0 * 1024.0);

            return memoryUsageInMegabytes;
        }
    }
}
