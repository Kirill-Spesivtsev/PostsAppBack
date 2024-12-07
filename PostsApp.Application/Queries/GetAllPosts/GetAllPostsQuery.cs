using MediatR;
using PostsApp.Domain.Entities;

namespace PostsApp.Application;

/// <summary>
/// Mediatr query to get all posts.
/// </summary>
public record GetAllPostsQuery() : IRequest<List<Post>>;