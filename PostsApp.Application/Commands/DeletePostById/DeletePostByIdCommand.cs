using MediatR;
using PostsApp.Domain.Entities;

namespace PostsApp.Application;

/// <summary>
/// Mediatr command to delete post by ID if found.
/// </summary>
/// <param name="Id"></param>
public record DeletePostByIdCommand(string Id) : IRequest;