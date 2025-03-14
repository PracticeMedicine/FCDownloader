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
using System.Runtime.Versioning;
using System.Reflection;
using Sentry;
using Microsoft.Extensions.Logging;

namespace AridityTeam.BetaFortressSetup.Util
{
    [SupportedOSPlatform("windows")]
    internal class AridLogger
    {
        private ILogger? logger;

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
                if(ClientDefs.EnableSentryCrashReporter == true)
                {
                    builder.AddSentry(o =>
                    {
                        o.Dsn = ClientDefs.SentryDSNLink;
                        o.AttachStacktrace = true;
                        o.Environment = ClientDefs.SentryEnvironment;
                        o.Release = string.Format("{0}@{1}",
                            thisAssembly?.GetName()?.Name,
                            thisAssembly?.GetName()?.Version?.ToString());
                        o.Debug = ClientDefs.SentryEnableDebugLogging;
                        o.TracesSampleRate = 1.0f;
                        o.ProfilesSampleRate = 1.0f;

                        o.AddProfilingIntegration();
                    });
                }
            });
        }

        /// <summary>
        /// Wrapper for SentrySdk.CaptureCheckIn.
        /// </summary>
        /// <param name="monitorSlug"></param>
        /// <param name="status"></param>
        /// <param name="sentryId"></param>
        /// <param name="duration"></param>
        /// <param name="scope"></param>
        /// <param name="configureMonitorOptions"></param>
        /// <returns></returns>
        public SentryId Sentry_CaptureCheckIn(
            string monitorSlug, 
            CheckInStatus status, 
            SentryId? sentryId = null, 
            TimeSpan? duration = null, 
            Scope? scope = null, 
            Action<SentryMonitorOptions>? configureMonitorOptions = null)
        {
            return SentrySdk.CaptureCheckIn(monitorSlug, status, sentryId, duration, scope, configureMonitorOptions);
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
