namespace PostsApp.Domain.Extensions;

/// <summary>
/// General abstract HTTP response exception.
/// </summary>
public abstract class HttpResponseException : Exception 
{ 
    protected HttpResponseException(string message) : base (message){ }

    public int Status { get; init; }

    public string Title { get; init; } = default!;

    public string Type { get; init; } = default!;
}
