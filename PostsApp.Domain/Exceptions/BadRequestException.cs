using System.Net;

namespace PostsApp.Domain.Extensions;

/// <summary>
/// Exception for Bad Request (401) HTTP response.
/// </summary>
public class BadRequestException : HttpResponseException
{
    public BadRequestException(string message) : base(message)
    {
        Status = (int)HttpStatusCode.BadRequest;
        Title = "Bad Request";
        Type = @"https://tools.ietf.org/html/rfc7231#section-6.5.1";
    }

}
