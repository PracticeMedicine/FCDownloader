namespace AridityTeam.Base.Util;

public static class LogSeverityExtensions
{
    public static Sentry.SentryLevel ToSentryLevel(this LogSeverity severity) 
    {
        switch (severity)
        {
            case LogSeverity.LogInfo:
                return Sentry.SentryLevel.Info;
            case LogSeverity.LogWarning:
                return Sentry.SentryLevel.Warning;
            case LogSeverity.LogError:
                return Sentry.SentryLevel.Error;
            case LogSeverity.LogFatal:
                return Sentry.SentryLevel.Fatal;
            default:
                return Sentry.SentryLevel.Debug;
        }
    }
}