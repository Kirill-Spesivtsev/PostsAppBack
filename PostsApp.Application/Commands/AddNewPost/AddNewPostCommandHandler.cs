using AutoMapper;
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
	private readonly IMapper _mapper;

	public AddNewPostCommandHandler(
		IPostRepository postRepository, 
		ILogger<AddNewPostCommandHandler> logger, 
		IExternalApiService externalApiService,
		IMapper mapper)
	{
		_postRepository = postRepository;
		_logger = logger;
		_externalApiService = externalApiService;
		_mapper = mapper;
	}

	public async Task<Post> Handle(AddNewPostCommand request, CancellationToken cancellationToken)
	{
		Post post = _mapper.Map<Post>(request);

		await _postRepository.AddAsync(post, cancellationToken);
		_logger.LogInformation($"Post was created with the ID {post.Id}");

		return post;
	}
}