namespace PostsApp.Application;

using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;

/// <summary>
/// Mediatr command handler to delete post by ID if found.
/// </summary>
public class DeletePostByIdCommandHandler : IRequestHandler<DeletePostByIdCommand>
{
	private readonly IPostRepository _postRepository;
	private readonly ILogger<DeletePostByIdCommandHandler> _logger;
	private readonly IMemoryCache _memoryCache;

	public DeletePostByIdCommandHandler(
		IPostRepository postRepository, 
		IMemoryCache memoryCache,
		ILogger<DeletePostByIdCommandHandler> logger)
	{
		_postRepository = postRepository;
		_memoryCache = memoryCache;
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

		var cacheOptions = new MemoryCacheEntryOptions
		{
			Size = 50,
			SlidingExpiration = TimeSpan.FromMinutes(5),
			AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
		};
		_memoryCache.Remove($"Post_{post.Id}");
		if (_memoryCache.TryGetValue("Posts_All", out List<Post>? posts))
		{
			posts = posts?.Where(item => item.Id != post.Id).ToList();
			_memoryCache.Set("Posts_All", posts, cacheOptions);
		}

	}
}