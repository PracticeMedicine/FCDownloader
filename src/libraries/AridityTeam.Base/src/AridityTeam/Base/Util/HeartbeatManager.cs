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
        private ObservableConcurrentBag<HeartbeatInstance?>? _allInstances;
        private ObservableConcurrentBag<HeartbeatInstance?>? _allRunningInstances;
        private AridLogger? _logger = null;
        private static HeartbeatManager? _mgrInstance = null;
        private static object _lock = new object();

        /// <summary>
        /// Gets the existing instance of HeartbeatManager.
        /// </summary>
        public static HeartbeatManager Instance
        {
            get
            {
                lock(_lock)
                {
                    if (_mgrInstance == null) _mgrInstance = new HeartbeatManager();
                    return _mgrInstance;
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public HeartbeatManager()
        {
            if (_logger == null)
            {
                _logger = new AridLogger(typeof(HeartbeatInstance));
            }

            _allInstances = new ObservableConcurrentBag<HeartbeatInstance?>();
            _allRunningInstances = new ObservableConcurrentBag<HeartbeatInstance?>();

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
            _logger?.Info("Instance {0} is now canceled.", obj?.InstanceName);
        }

        /// <summary>
        /// An event.
        /// </summary>
        /// <param name="instance"></param>
        private void RunningInstances_OnItemAdded(HeartbeatInstance? instance)
        {
            _logger?.Info("The heartbeat instance '{0}' is now running...", instance?.InstanceName);
        }

        /// <summary>
        /// An event.
        /// </summary>
        /// <param name="instance"></param>
        private void Instances_OnItemAdded(HeartbeatInstance? instance)
        {
            try
            {
                _logger?.Info("Instance has been added:\n{0}", instance?.ToString());

                lock(_lock)
                {
                    if (_allRunningInstances != null && instance != null && !_allRunningInstances.Contains(instance))
                    {
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
            if(_allRunningInstances != null)
            {
                foreach (var instance in _allRunningInstances)
                {
                    if (instance != null)
                    {
                        _logger?.Info("Canceling {0}...", instance?.InstanceName);
                        instance?.CancellationTokenSource?.Cancel();

                        if (disposing == true)
                        {
                            _logger?.Info("Disposing instance ({0})...", instance?.InstanceName);
                            instance?.CancellationTokenSource?.Dispose();
                            instance?.RunningTask?.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if an heartbeat instance is running.
        /// </summary>
        /// <param name="instanceName">Heartbeat instance name</param>
        /// <returns>Returns true if it is running.</returns>
        public bool IsHeartbeatInstanceRunning(string instanceName)
        {
            if(_allRunningInstances != null)
            {
                foreach (var instance in _allRunningInstances)
                {
                    if (instance?.InstanceName == instanceName && !instance.CancellationToken.IsCancellationRequested) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if an existing heartbeat instance is running.
        /// </summary>
        /// <param name="selectedInstance">Existing Heartbeat instance</param>
        /// <returns>Returns true if it is running.</returns>
        public bool IsHeartbeatInstanceRunning(HeartbeatInstance selectedInstance)
        {
            if (_allRunningInstances != null)
            {
                foreach (var instance in _allRunningInstances)
                {
                    return instance != null && instance.Equals(selectedInstance) && !instance.CancellationToken.IsCancellationRequested;
                }
            }
            return false;
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
            _logger?.Info("Disposing...");
            CancelAllInstances(true);
        }
    }
}
