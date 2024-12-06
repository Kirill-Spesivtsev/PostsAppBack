using MediatR;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;

internal class AddNewPostCommandHandler : IRequestHandler<AddNewPostCommand, Post>
{
	private readonly IPostRepository _postRepository;
	private readonly ILogger<AddNewPostCommandHandler> _logger;
	private readonly IExternalApiService _externalApiService;

	public AddNewPostCommandHandler(IPostRepository postRepository, ILogger<AddNewPostCommandHandler> logger, IExternalApiService externalApiService)
	{
		_postRepository = postRepository;
		_logger = logger;
		_externalApiService = externalApiService;
	}

	public async Task<Post> Handle(AddNewPostCommand request, CancellationToken cancellationToken)
	{
		await _postRepository.AddAsync(request.Post, cancellationToken);
		_logger.LogInformation($"Post was created with the ID {request.Post.Id}");

		return request.Post;
	}
}