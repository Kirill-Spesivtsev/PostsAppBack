using MediatR;
using Microsoft.Extensions.Logging;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;

namespace PostsApp.Application;

public record GetPostByIdQuery(string Id) : IRequest<Post>;