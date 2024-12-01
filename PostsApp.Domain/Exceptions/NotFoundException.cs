using System.Net;

namespace PostsApp.Domain.Extensions;

public class NotFoundException : HttpResponseException
{
    public NotFoundException(string message) : base(message)
    {
        Status = (int)HttpStatusCode.NotFound;
        Title = "Not Found";
        Type = @"https://tools.ietf.org/html/rfc7231#section-6.5.4";
    }

    public NotFoundException(string entityName, string id) : this($"{entityName} with ID {id} was not found"){}
}
