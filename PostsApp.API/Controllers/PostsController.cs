using Microsoft.AspNetCore.Mvc;
using PostsApp.Domain.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mime;
using MediatR;
using PostsApp.Application;
using PostsApp.Domain.Entities;


namespace PostsApp.API.Controllers
{
	[ApiController]
	[Route("api/posts")]
	public class PostsController(IMediator mediator, ILogger<PostsController> logger) : ControllerBase
	{
		private readonly ILogger<PostsController> _logger = logger;
		private readonly IMediator _mediator = mediator;

		[HttpGet]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
		public async Task<IActionResult> GetAllPosts(CancellationToken cancellationToken)
		{
			var query = new GetAllPostsQuery();

			var allPosts = await _mediator.Send(query, cancellationToken);

			return Ok(allPosts);
		}

		[HttpGet("{id}")]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetPostById(string id, CancellationToken cancellationToken)
		{
			var query = new GetPostByIdQuery(id);

			var post = await _mediator.Send(query, cancellationToken);

			return Ok(post);
		}

		[HttpPost]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Post))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AddPost(AddNewPostCommand request, CancellationToken cancellationToken)
		{
			var createdPost = await _mediator.Send(request, cancellationToken);

			return CreatedAtAction(nameof(AddPost), new { createdPost.Id }, createdPost);
		}

	}
}
