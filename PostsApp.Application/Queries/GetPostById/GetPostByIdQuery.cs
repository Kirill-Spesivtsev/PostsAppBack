using MediatR;
using Microsoft.Extensions.Logging;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;

namespace PostsApp.Application;

/// <summary>
/// Mediatr query to get post by provided ID.
/// </summary>
/// <param name="Id">Post ID</param>
public record GetPostByIdQuery(string Id) : IRequest<Post>;