using Sentry;

namespace AridityTeam.Base.Util;

/// <summary>
/// Represents the settings for logging.
/// </summary>
public class LoggingSettings
{
    /// <summary>
    /// Gets or sets the destination for logging.
    /// </summary>
    public LoggingDestination Destination { get; set; } = LoggingDestination.LogDefault;
    
    /// <summary>
    /// Condition.
    /// Whether to enable the Sentry SDK.
    /// </summary>
    public bool EnableSentry { get; set; } = false;
    
    /// <summary>
    /// Sets the Sentry SDK options.
    /// </summary>
    public SentryOptions? SentryOptions { get; set; } = new();
}