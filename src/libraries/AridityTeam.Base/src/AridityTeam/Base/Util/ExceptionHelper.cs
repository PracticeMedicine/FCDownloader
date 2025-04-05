using System;

namespace AridityTeam.Base.Util;

public static class ExceptionHelper
{
    private static void LogException(Exception ex)
    {
        using var log = new Logger();
        // Here you can implement your logging logic (e.g., log to a file, database, etc.)
        log.Log(LogSeverity.LogError, $"Exception: {ex.Message}");
        log.Log(LogSeverity.LogError, $"Stack Trace: {ex.StackTrace}");
    }

    public static Result<T> HandleException<T>(Func<Result<T>> func)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            LogException(ex);
            return Result<T>.Failure("An error occurred while processing your request.");
        }
    }
}