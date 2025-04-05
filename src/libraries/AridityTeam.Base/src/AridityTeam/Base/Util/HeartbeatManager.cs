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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AridityTeam.Base.Util
{
    /// <summary>
    /// Manages heartbeat/loop functions.
    /// </summary>
    public class HeartbeatManager : IDisposable
    {
        private readonly ObservableConcurrentBag<HeartbeatInstance?>? _allInstances;
        private readonly ObservableConcurrentBag<HeartbeatInstance?>? _allRunningInstances;
        private readonly Logger? _logger;
        private static HeartbeatManager? _mgrInstance;
        private static readonly Lock Lock = new();

        /// <summary>
        /// Gets the existing instance of HeartbeatManager.
        /// </summary>
        public static HeartbeatManager Instance
        {
            get
            {
                lock(Lock)
                {
                    return _mgrInstance ??= new HeartbeatManager();
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public HeartbeatManager()
        {
            _logger ??= new Logger();

            _allInstances = [];
            _allRunningInstances = [];

            _allInstances.ItemAdded += Instances_OnItemAdded;
            _allRunningInstances.ItemAdded += RunningInstances_OnItemAdded;
            _allRunningInstances.ItemRemoved += RunningInstances_OnItemRemoved;
        }

        /// <summary>
        /// An event.
        /// </summary>
        /// <param name="obj"></param>
        private void RunningInstances_OnItemRemoved(HeartbeatInstance? obj)
        {
            _logger?.Log(LogSeverity.LogInfo, $"Instance {obj?.InstanceName} is now canceled.");
        }

        /// <summary>
        /// An event.
        /// </summary>
        /// <param name="instance"></param>
        private void RunningInstances_OnItemAdded(HeartbeatInstance? instance)
        {
            _logger?.Log(LogSeverity.LogInfo, $"The heartbeat instance '{instance?.InstanceName}' is now running...");
        }

        /// <summary>
        /// An event.
        /// </summary>
        /// <param name="instance"></param>
        private void Instances_OnItemAdded(HeartbeatInstance? instance)
        {
            try
            {
                _logger?.Log(LogSeverity.LogInfo, $"Instance has been added:\n{instance}");

                lock(Lock)
                {
                    if (_allRunningInstances == null || instance == null ||
                        _allRunningInstances.Contains(instance)) return;
                    var cts = new CancellationTokenSource();
                    instance.CancellationToken = cts.Token;
                    instance.CancellationTokenSource = cts;

                    instance.RunningTask = Task.Run(async () =>
                    {
                        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(instance.HeartbeatTime));
                        while (await timer.WaitForNextTickAsync(instance.CancellationToken))
                        {
                            instance.ActionToRun.Invoke();
                        }
                    }, instance.CancellationToken);

                    _allRunningInstances?.Add(instance);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occured on the heartbeat instance.", ex);
            }
        }

        /// <summary>
        /// Cancels/stops all heartbeat instances.
        /// </summary>
        public void CancelAllInstances(bool disposing = false)
        {
            if (_allRunningInstances == null) return;
            foreach (var instance in _allRunningInstances)
            {
                if (instance == null) continue;
                _logger?.Log(LogSeverity.LogInfo, $"Canceling {instance.InstanceName}...");
                instance.CancellationTokenSource?.Cancel();

                if (disposing != true) continue;
                _logger?.Log(LogSeverity.LogInfo, $"Disposing instance ({instance.InstanceName})...");
                instance.CancellationTokenSource?.Dispose();
                instance.RunningTask?.Dispose();
            }
        }

        /// <summary>
        /// Checks if a heartbeat instance is running.
        /// </summary>
        /// <param name="instanceName">Heartbeat instance name</param>
        /// <returns>Returns true if it is running.</returns>
        public bool IsHeartbeatInstanceRunning(string instanceName)
        {
            return _allRunningInstances != null && _allRunningInstances.Any(instance => instance?.InstanceName == instanceName 
                && !instance.CancellationToken.IsCancellationRequested);
        }
        /// <summary>
        /// Checks if an existing heartbeat instance is running.
        /// </summary>
        /// <param name="selectedInstance">Existing Heartbeat instance</param>
        /// <returns>Returns true if it is running.</returns>
        public bool IsHeartbeatInstanceRunning(HeartbeatInstance selectedInstance)
        {
            return _allRunningInstances != null && _allRunningInstances.Select(instance => instance != null && instance.Equals(selectedInstance) 
                && !instance.CancellationToken.IsCancellationRequested).FirstOrDefault();
        }

        /// <summary>
        /// Adds an instance to the heartbeat manager.
        /// </summary>
        /// <param name="newInstance">Heartbeat instance to add</param>
        public void AddInstance(HeartbeatInstance newInstance)
        {
            if (_allInstances != null)
            {
                _allInstances.Add(newInstance);
            }
        }

        // Disposes shit
        public void Dispose()
        {
            _logger?.Log(LogSeverity.LogInfo, "Disposing...");
            CancelAllInstances(true);
        }
    }
}
