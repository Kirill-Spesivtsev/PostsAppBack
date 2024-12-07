using MediatR;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;

/// <summary>
/// Mediatr query handler to get post by provided ID.
/// </summary>
internal class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Post>
{
	private readonly IPostRepository _postRepository;
	private readonly ILogger<GetPostByIdQueryHandler> _logger;

	public GetPostByIdQueryHandler(IPostRepository postRepository, ILogger<GetPostByIdQueryHandler> logger)
	{
		_postRepository = postRepository;
		_logger = logger;
	}

	public async Task<Post> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
	{
		var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

		if (post is null)
		{
			throw new NotFoundException(nameof(Post), request.Id);
		}

		return post;
	}
}