using MediatR;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;

/// <summary>
/// Mediatr query handler to get all posts.
/// </summary>
internal class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, List<Post>>
{
	private readonly IPostRepository _postRepository;
	private readonly ILogger<GetAllPostsQueryHandler> _logger;
	private readonly IExternalApiService _externalApiService;

	public GetAllPostsQueryHandler(IPostRepository postRepository, ILogger<GetAllPostsQueryHandler> logger, IExternalApiService externalApiService)
	{
		_postRepository = postRepository;
		_logger = logger;
		_externalApiService = externalApiService;
	}

	public async Task<List<Post>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
	{
		var posts = await _postRepository.GetAllAsync(cancellationToken);
		if (posts.Count == 0)
		{
			var externalPosts = await _externalApiService.FetchExternalData();
			await _postRepository.BulkAddAsync(externalPosts);
			_logger.LogInformation("Empty database was filled");

			return externalPosts;
		}

		return posts;
	}
}