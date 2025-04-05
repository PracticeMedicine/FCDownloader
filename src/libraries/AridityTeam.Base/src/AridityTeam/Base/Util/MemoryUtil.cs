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
using System.Diagnostics;
using AridityTeam.Base.Internal;

namespace AridityTeam.Base.Util
{
    public static class MemoryUtil
    {
        private static readonly Logger Logger = new();

        public static void CheckForMemoryLeaks(double maxMemUsage)
        {
            var instance = new HeartbeatInstance()
            {
                InstanceName = "Aridity Base Memory Checker",
                HeartbeatTime = 3500,
                ActionToRun = () =>
                {
                    Logger.Log(LogSeverity.LogInfo, $"Current memory usage: {maxMemUsage}mb");
                    if (GetMemoryUsageInMb() > maxMemUsage)
                    {
                        throw new PerformanceException("Exceeded max memory limit.");
                    }
                },
            };

            HeartbeatManager.Instance.AddInstance(instance);
        }

        private static double GetMemoryUsageInMb()
        {
            // Get the current process
            var currentProcess = Process.GetCurrentProcess();

            // Get the memory usage in bytes
            var memoryUsageInBytes = currentProcess.WorkingSet64; // or use PrivateMemorySize64

            // Convert bytes to megabytes
            var memoryUsageInMegabytes = memoryUsageInBytes / (1024.0 * 1024.0);

            return memoryUsageInMegabytes;
        }
    }
}
