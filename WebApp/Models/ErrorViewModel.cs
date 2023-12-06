namespace WebApp.Models;

/// <summary>
/// Represents a model for handling errors in the application.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the current request.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether to show the request ID.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
