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
using System.Threading;
using System.Threading.Tasks;

namespace AridityTeam.Base.Util
{
    /// <summary>
    /// Heartbeat Instance configuration
    /// </summary>
    public class HeartbeatInstance
    {
        /// <summary>
        /// Name of the heartbeat instance.
        /// (The thread name will be the same as the instance name.)
        /// </summary>
        public string? InstanceName { get; set; }

        /// <summary>
        /// Time till the next heartbeat.
        /// </summary>
        public required int HeartbeatTime { get; set; }

        /// <summary>
        /// Self-explanatory.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Self-explanatory.
        /// </summary>
        public CancellationTokenSource? CancellationTokenSource { get; set; }

        /// <summary>
        /// Gets the current thread in which the instance is running on.
        /// </summary>
        public Task? RunningTask { get; set; }

        /// <summary>
        /// Action to run.
        /// </summary>
        public required Action ActionToRun { get; set; }

        public override string ToString()
        {
            return $"""
                Heartbeat Instance info:
                    InstanceName: {InstanceName}
                    HeartbeatTime: {HeartbeatTime}
                """;
        }
    }
}
