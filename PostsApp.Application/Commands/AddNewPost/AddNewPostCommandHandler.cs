using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;

/// <summary>
/// Mediatr command handler to add a new post.
/// </summary>
public class AddNewPostCommandHandler : IRequestHandler<AddNewPostCommand, Post>
{
	private readonly IPostRepository _postRepository;
	private readonly ILogger<AddNewPostCommandHandler> _logger;
	private readonly IMemoryCache _memoryCache;
	private readonly IMapper _mapper;

	public AddNewPostCommandHandler(
		IPostRepository postRepository,
		IMemoryCache memoryCache,
		IMapper mapper,
		ILogger<AddNewPostCommandHandler> logger)
	{
		_postRepository = postRepository;
		_logger = logger;
		_mapper = mapper;
		_memoryCache = memoryCache;
	}

	public async Task<Post> Handle(AddNewPostCommand request, CancellationToken cancellationToken)
	{
		Post post = _mapper.Map<Post>(request);

		await _postRepository.AddAsync(post, cancellationToken);
		_logger.LogInformation($"Post was created with the ID {post.Id}");

		var cacheOptions = new MemoryCacheEntryOptions
		{
			SlidingExpiration = TimeSpan.FromMinutes(5),
			AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
		};
		_memoryCache.Set($"Post_{post.Id}", post,cacheOptions.SetSize(1));
		if (_memoryCache.TryGetValue("Posts_All", out List<Post>? posts))
		{
			posts?.Add(post);
			_memoryCache.Set("Posts_All", posts, cacheOptions.SetSize(50));
		}

		return post;
	}
}