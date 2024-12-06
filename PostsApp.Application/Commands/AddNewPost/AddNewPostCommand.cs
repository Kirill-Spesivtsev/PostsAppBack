using MediatR;
using PostsApp.Domain.Entities;

namespace PostsApp.Application;

public record AddNewPostCommand(Post Post) : IRequest<Post>;