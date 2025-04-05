namespace AridityTeam.Base.Util;

public enum LogSeverity : int
{
    LogVerbose = -1,
    LogInfo,
    LogWarning,
    LogError,
    LogErrorReport,
    LogFatal,
    LogNumSeverities = 5,

#if !DEBUG
    LogDFatal = LogError,
#else
    LogDFatal = LogFatal,    
#endif
}