using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;

/// <summary>
/// Mediatr query handler to get all posts.
/// </summary>
public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, List<Post>>
{
	private readonly IPostRepository _postRepository;
	private readonly IMemoryCache _cache;
	private readonly ILogger<GetAllPostsQueryHandler> _logger;
	private readonly IExternalApiService _externalApiService;

	public GetAllPostsQueryHandler(
		IPostRepository postRepository,
		IMemoryCache cache,
		ILogger<GetAllPostsQueryHandler> logger, 
		IExternalApiService externalApiService)
	{
		_postRepository = postRepository;
		_cache = cache;
		_logger = logger;
		_externalApiService = externalApiService;
	}

	public async Task<List<Post>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
	{
		var entryKey = "Posts_All";

		var cachePosts = await _cache.GetOrCreateAsync(entryKey, async entry => 
		{
			entry.SetSize(50);
			entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
			entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));

			var posts = await _postRepository.GetAllAsync(cancellationToken);
			if (posts.Count == 0)
			{
				var externalPosts = await _externalApiService.FetchExternalData();
				if (externalPosts.Count > 0)
				{
					await _postRepository.BulkAddAsync(externalPosts);
					_logger.LogInformation("Empty database was filled");
				}

				return externalPosts;
			}
			return posts;
		});

		return cachePosts ?? [];
	}
}