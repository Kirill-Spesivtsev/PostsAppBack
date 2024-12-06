using MediatR;
using PostsApp.Domain.Entities;

namespace PostsApp.Application;

public record GetAllPostsQuery() : IRequest<List<Post>>;