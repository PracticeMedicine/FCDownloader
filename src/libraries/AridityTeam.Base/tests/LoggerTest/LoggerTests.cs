using System;
using System.IO;
//using Moq;
using AridityTeam.Base.Util;
using Sentry;

namespace AridityTeam.Base.Tests.LoggerTest;

public class LoggerTests
{
    [Fact]
    public void Instance_ShouldReturnSingletonInstance()
    {
        var logger1 = Logger.Instance;
        var logger2 = Logger.Instance;

        Assert.AreEqual(logger1, logger2);
    }

    [Fact]
    public void InitLogging_ShouldInitializeSentry_WhenEnabled()
    {
        var logger = Logger.Instance;
        var settings = new LoggingSettings
        {
            EnableSentry = true,
            SentryOptions = new SentryOptions() { Dsn = "" }// Assume this is a valid SentryOptions object
        };

        var result = logger.InitLogging(settings);

        Assert.IsTrue(result);
        // You may want to verify that SentrySdk.Init was called, if you can mock it.
    }

    [Fact]
    public void Log_ShouldLogMessage_WhenCalled()
    {
        var logger = Logger.Instance;
        var settings = new LoggingSettings
        {
            EnableSentry = false
        };

        logger.InitLogging(settings);

        using var sw = new StringWriter();
        Console.SetOut(sw);
        logger.Log(LogSeverity.LogInfo, "Test log message");

        var result = sw.ToString().Trim();
        Assert.Contains("Test log message", result);
    }

    [Fact]
    public void Dispose_ShouldDisposeSentry_WhenCalled()
    {
        var logger = Logger.Instance;
        var settings = new LoggingSettings
        {
            EnableSentry = true,
            SentryOptions = new SentryOptions() { Dsn = "" } // Assume this is a valid SentryOptions object
        };

        logger.InitLogging(settings);
        logger.Dispose();

        // Verify that _sentry is disposed
        Assert.IsNull(logger.GetSentryInit());
    }
}