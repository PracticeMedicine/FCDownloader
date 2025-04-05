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
using System.Threading;
using System.Runtime.CompilerServices;
using Sentry;

namespace AridityTeam.Base.Util;

/// <summary>
/// Ah, yes. The so-called "Chromium" logger in ".NET".
/// <para>
/// This kind of replicates Chromium's logging format + some of the code were just copied from the logger code of Chromium. (just some C++ keywords were changed)
/// </para>
/// </summary>
public class Logger : ILogger
{
    private LogMessageHandlerFunction? _logMessageHandler;
    private LoggingSettings? _settings;
    private static Logger? _instance;
    private IDisposable? _sentry;
    private static readonly Lock Lock = new();
    private LogMessage? _logMsg;

    public static Logger Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new Logger();
            }
        }
    }
    
    public int GetMinLogLevel()
    {
        return (int)LogSeverity.LogInfo;
    }

    public int GetVLogLevel(string idk)
    {
        return int.MaxValue;
    }

    public bool InitLogging(LoggingSettings settings)
    {
        _settings = settings;

        if (settings.EnableSentry != true) return true;
        if (settings.SentryOptions == null) return false;
            
        _sentry = SentrySdk.Init(settings.SentryOptions);

        return true;
    }

    public void Log(LogSeverity severity, string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int line = 0)
    {
        _logMsg ??= new LogMessage(filePath, line, (int)severity);
        switch (severity)
        {
            case LogSeverity.LogInfo:
            case LogSeverity.LogVerbose:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case LogSeverity.LogWarning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.LogError:
            case LogSeverity.LogErrorReport:
            case LogSeverity.LogFatal:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ResetColor();
                break;
        }
        _logMsg?.GetWriter()?.WriteLine(message);
        Console.ResetColor();
        if (_settings?.EnableSentry == true)
            SentrySdk.CaptureMessage(_logMsg?.GetLastOutput() + message, severity.ToSentryLevel());
        _logMsg?.Dispose();
        _logMsg = null;
    }

    public void VLog(string message, int verbosity,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int line = 0)
    {
        _logMsg ??= new LogMessage(filePath, line, verbosity);
        _logMsg?.GetWriter()?.WriteLine(message);
        _logMsg?.Dispose();
        _logMsg = null;
    }

    public void Dispose()
    {
        _sentry?.Dispose();
        _sentry = null;
        _logMsg?.Dispose();
        _logMsg = null;
    }

    public IDisposable? GetSentryInit() => _sentry;
    
    public LoggingDestination GetDestination()
    {
        if (_settings != null) return _settings.Destination;
        return LoggingDestination.LogDefault;
    }

    public void SetLogMessageHandler(LogMessageHandlerFunction handler) => _logMessageHandler = handler;
    public LogMessageHandlerFunction? GetLogMessageHandler() => _logMessageHandler;
}