namespace PostsApp.Application;

using MediatR;
using Microsoft.Extensions.Logging;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;

/// <summary>
/// Mediatr command handler to delete post by ID if found.
/// </summary>
internal class DeletePostByIdCommandHandler : IRequestHandler<DeletePostByIdCommand>
{
	private readonly IPostRepository _postRepository;
	private readonly ILogger<DeletePostByIdCommandHandler> _logger;

	public DeletePostByIdCommandHandler(IPostRepository postRepository, ILogger<DeletePostByIdCommandHandler> logger)
	{
		_postRepository = postRepository;
		_logger = logger;
	}

	public async Task Handle(DeletePostByIdCommand request, CancellationToken cancellationToken)
	{
		var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

		if (post is null)
		{
			throw new NotFoundException(nameof(Post), request.Id);
		}

		await _postRepository.RemoveAsync(post.Id, cancellationToken);
	}
}