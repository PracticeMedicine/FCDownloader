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
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace AridityTeam.ElevatorProcess
{
    internal class AridLogger
    {
        private ILogger? logger = null;

        public AridLogger(string category)
        {
            logger = SetupLoggerFactory().CreateLogger(category);
        }

        public AridLogger(Type category)
        {
            logger = SetupLoggerFactory().CreateLogger(category);
        }

        private ILoggerFactory SetupLoggerFactory()
        {
            var thisAssembly = Assembly.GetEntryAssembly();
            return LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        public void Info(string content)
        {
            logger?.LogInformation(content);
        }

        public void Info(string content, params object?[] args)
        {
            logger?.LogInformation(content, args);
        }

        public void Warn(string content)
        {
            logger?.LogWarning(content);
        }

        public void Warn(string content, params object?[] args)
        {
            logger?.LogWarning(content, args);
        }

        public void Err(string content)
        {
            logger?.LogError(content);
        }

        public void Err(string content, params object?[] args)
        {
            logger?.LogError(content, args);
        }

        public void Debug(string content)
        {
            logger?.LogDebug(content);
        }

        public void Debug(string content, params object?[] args)
        {
            logger?.LogDebug(content, args);
        }

        public void Trace(string content)
        {
            logger?.LogTrace(content);
        }

        public void Trace(string content, params object?[] args)
        {
            logger?.LogTrace(content, args);
        }

        public void Crit(string content)
        {
            logger?.LogCritical(content);
        }

        public void Crit(string content, params object?[] args)
        {
            logger?.LogCritical(content, args);
        }
    }
}
