using System;
using System.Runtime.CompilerServices;

namespace AridityTeam.Base.Util;

public interface ILogger : IDisposable
{
    /// <summary>
    /// Gets the minimal log level.
    /// </summary>
    /// <returns></returns>
    int GetMinLogLevel();
    
    /// <summary>
    /// Gets the level of the verbose log.
    /// </summary>
    /// <returns></returns>
    int GetVLogLevel(string idk);

    /// <summary>
    /// <para>Initializes the logger, this only sets the logging destination.</para>
    /// <para>If InitLogging isn't called, the default destination will be LogToAll.</para>
    /// </summary>
    /// <param name="settings">Settings for the logger.</param>
    /// <returns>Returns true on success</returns>
    bool InitLogging(LoggingSettings settings);
    
    /// <summary>
    /// <para>Okay this is the fun part of this class.</para>
    /// <para>
    /// This logs a specific message and verbosity into the console and to the specified destination. Some stuff might not work as expected like LOG_TO_FILE but for now I'll leave it like this.
    /// Some stuff from mini_chromium (Chromium Base) that could possibly be ported will be planned.
    /// </para>
    /// <example>
    /// This shows you on how can you use it.
    /// <code>
    /// using AridityTeam.Base.Util;
    ///
    /// LoggingSettings settings = new LoggingSettings();
    /// settings.Destination = LoggingDestination.LogDefault;
    /// Logger _logger = new Logger();
    /// if(!_logger.InitLogging(settings))
    ///     throw new Exception("Failed to init logging settings");
    /// 
    /// _logger.Log(LogSeverity.LogInfo, "Hello world!");
    /// _logger.Log(LogSeverity.LogInfo, "This is the implementation or most likely a port of the Chromium logger in .NET!");
    /// _logger.Log(LogSeverity.LogInfo, "This is an info message.");
    /// _logger.Log(LogSeverity.LogWarning, "This is a warning message.");
    /// _logger.Log(LogSeverity.LogError, "This is an error message.");
    /// _logger.Log(LogSeverity.LogFatal, "This is a fatal message. The program will exit immediately in code 1.");
    /// </code>
    /// </example>
    /// <para>
    /// The output is formatted as per the following example as on every platform. (Except for ASP.NET/Browser.)
    /// <code>
    /// [2025325,144613.272:INFO Program.cs:90] Succeeded
    /// </code>
    /// The colon separated fields inside the brackets are as follows:
    /// <list type="bullet">
    /// <item>An optional Logfile prefix (not included in this example)</item>
    /// <item>Process ID</item>
    /// <item>Thread ID</item>
    /// <item>The date/time of the log message, in MMDD/HHMMSS.Milliseconds format</item>
    /// <item>The log level</item>
    /// <item>The filename and line number where the log was instantiated</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="severity">Log severity</param>
    /// <param name="message">Message to output</param>
    /// <param name="filePath">File path to the caller</param>
    /// <param name="lineNumber">Line number of the caller</param>
    void Log(LogSeverity severity, string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    /// Kind of the same as <see cref="ILogger"/>.Info but for verbose output.
    /// </summary>
    /// <param name="message">Message to output</param>
    /// <param name="verbosity">The verbosity of the log message</param>
    /// <param name="filePath">File path to the caller</param>
    /// <param name="lineNumber">Line number of the caller</param>
    void VLog(string message, int verbosity,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);
    
    /// <summary>
    /// Gets the Logging destination.
    /// </summary>
    /// <returns>Returns the logging destination specified.</returns>
    LoggingDestination GetDestination();
    
    /// <summary>
    /// You know it.
    /// </summary>
    /// <param name="handler"></param>
    void SetLogMessageHandler(LogMessageHandlerFunction handler);
    
    /// <summary>
    /// You know it.
    /// </summary>
    /// <returns></returns>
    LogMessageHandlerFunction? GetLogMessageHandler();
}