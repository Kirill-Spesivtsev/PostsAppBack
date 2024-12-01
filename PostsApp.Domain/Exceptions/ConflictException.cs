using System.Net;


namespace PostsApp.Domain.Extensions;

public class ConflictException : HttpResponseException
{
    public ConflictException(string message) : base(message)
    {
        Status = (int)HttpStatusCode.Conflict;
        Title = "Conflict";
        Type = @"https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
    }

    public ConflictException(string entityName, string id) : this($"{entityName} with ID {id} already exists"){}
}

