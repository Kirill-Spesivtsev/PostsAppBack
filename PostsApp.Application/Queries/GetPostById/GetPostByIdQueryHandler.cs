using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;
using System.Text.Json;

/// <summary>
/// Mediatr query handler to get post by provided ID.
/// </summary>
internal class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Post>
{
	private readonly IPostRepository _postRepository;
	private readonly IMemoryCache _cache;
	private readonly ILogger<GetPostByIdQueryHandler> _logger;

	public GetPostByIdQueryHandler(IPostRepository postRepository, IMemoryCache cache, ILogger<GetPostByIdQueryHandler> logger)
	{
		_postRepository = postRepository;
		_cache = cache;
		_logger = logger;
	}

	public async Task<Post> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
	{
		var key = $"Post_{request.Id}";

		var cahePost = await _cache.GetOrCreateAsync(key, async entry =>
		{
			entry.SetSize(1);
			entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
			entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));

			var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

			if (post is null)
			{
				throw new NotFoundException(nameof(Post), request.Id);
			}

			return post;
		});
		
		return cahePost!;
	}
}