namespace PostsApp.Domain.Extensions;

public abstract class HttpResponseException : Exception 
{ 
    protected HttpResponseException(string message) : base (message){ }

    public int Status { get; init; }

    public string Title { get; init; } = default!;

    public string Type { get; init; } = default!;
}
