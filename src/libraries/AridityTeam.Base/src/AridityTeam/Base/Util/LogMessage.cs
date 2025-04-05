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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AridityTeam.Base.Util;

/// <summary>
/// <para>https://github.com/chromium/mini_chromium/blob/main/base/logging.cc</para>
/// <para>https://github.com/chromium/mini_chromium/blob/main/base/logging.h#L95</para>
/// </summary>
internal class LogMessage : IDisposable
{
    private readonly string[] _logSeverityNames =
    [
        "INFO",
        "WARNING",
        "ERROR",
        "ERROR_REPORT",
        "FATAL"
    ];

    private readonly System.Threading.Lock _lock = new();
    
    private TextWriter? _writer;
    private readonly string _filePath;
    private readonly int _line ;
    private readonly LogMessageHandlerFunction? _handler = Logger.Instance.GetLogMessageHandler();
    private readonly LogSeverity _severity;
    private int? _messageStart;
    private string? _output;
    
    public LogMessage(string filePath, int line, int severity)
    {
        _writer = Console.Out;
        _filePath = filePath;
        _messageStart = 0;
        _output = null;
        _line = line;
        _severity = (LogSeverity)severity;
        Init();
    }
    public LogMessage(string filePath, int line, string result)
    {
        _writer = Console.Out;
        _filePath = filePath;
        _line = line;
        _messageStart = 0;
        _output = null;
        _severity = LogSeverity.LogFatal;
        Init();
        _writer.WriteLine("Check failed: " + result + ".");
    }

    public void Dispose()
    {
        Flush();
        GC.SuppressFinalize(this);
    }

    public void SetWriter(TextWriter? writer)
    {
        _writer = writer;
    }

    public TextWriter? GetWriter()
    {
        return _writer;
    }

    public string GetFilePath()
    {
        return _filePath;
    }

    private void Init()
    {
        lock (_lock)
        {
            switch (_severity)
            {
                case LogSeverity.LogInfo:
                case LogSeverity.LogVerbose:
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.LogWarning:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.LogError:
                case LogSeverity.LogErrorReport:
                case LogSeverity.LogFatal:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            var destination = Logger.Instance.GetDestination();
            var strBuilder = new StringBuilder();
            var fileName = _filePath;

            _writer = new MultiTextWriter(Console.Out);

            if (destination.IsEnabled(LoggingDestination.LogToFile))
            {
                if (_writer is MultiTextWriter writer)
                {
                    writer.AddWriter(new FileTextWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log"), true));
                }
                else 
                {
                    _writer = new FileTextWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log"), true);
                }
            }
            
            if (destination.IsEnabled(LoggingDestination.LogToStderr))
            {
                if (_writer is MultiTextWriter writer)
                {
                    writer.AddWriter(Console.Error);
                }
                else 
                {
                    _writer = Console.Error;
                }
            }

            int lastSlash;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                lastSlash = fileName.LastIndexOf('\\');
            else
                lastSlash = fileName.LastIndexOf('/');

            if (lastSlash != -1)
            {
                fileName = fileName.Substring(lastSlash + 1);
            }

            var pid = Process.GetCurrentProcess().Id;
            var thread = Environment.CurrentManagedThreadId;

            strBuilder.Append("[" +
                           pid +
                           ":" +
                           thread +
                           ":");
            
            var localTime = DateTime.Now;
            
            strBuilder.Append(localTime.Year.ToString() +
                           localTime.Month.ToString() +
                           localTime.Day.ToString() +
                           "," +
                           localTime.Hour.ToString() +
                           localTime.Minute.ToString() +
                           localTime.Second.ToString() +
                           "." + localTime.Millisecond +
                           ":");

            if ((int)_severity >= 0)
            {
                strBuilder.Append(_logSeverityNames[(int)_severity]);
            }
            else
            {
                strBuilder.Append("VERBOSE" + -(int)_severity);
            }

            strBuilder.Append(" " +
                           fileName +
                           ":" +
                           _line +
                           "]");
            
            _output = strBuilder.ToString();
            Debug.WriteLine("The output of the log message: "+_output,"Logger");
            _writer?.Write(_output);

            _messageStart = _writer?.ToString()?.Length;
            Console.ResetColor();
            
            _writer?.Write(' ');
        }
    }

    public string? GetLastOutput() => _output;

    private void Flush()
    {
        lock (_lock)
        {
            var destination = Logger.Instance.GetDestination();

            var strNewLine = _writer?.NewLine;

            if (_handler != null && strNewLine != null &&
                _handler(_severity, _filePath, _line, _messageStart, strNewLine))
                return;

            if (destination.IsEnabled(LoggingDestination.LogToStderr))
            {
                Console.Error.Write("{0}", strNewLine);
                Console.Error.Flush();
            }

            if (destination.IsEnabled(LoggingDestination.LogToSystemDebug))
            {
                Debug.WriteLine(strNewLine); 
            }
            
            _writer?.Close();

            if (_severity == LogSeverity.LogFatal)
                BaseFunctions.ImmediateCrash("Fatal error detected!");// "base::ImmediateCrash" they say.
        }
    }
}