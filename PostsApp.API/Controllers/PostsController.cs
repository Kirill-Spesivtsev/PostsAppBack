using Microsoft.AspNetCore.Mvc;
using PostsApp.Domain.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mime;
using MediatR;
using PostsApp.Application;
using PostsApp.Domain.Entities;


namespace PostsApp.API.Controllers
{
	/// <summary>
	/// Controller for managing posts
	/// </summary>
	/// <param name="mediator"></param>
	/// <param name="logger"></param>
	[ApiController]
	[Route("api/posts")]
	public class PostsController(IMediator mediator, ILogger<PostsController> logger) : ControllerBase
	{
		private readonly ILogger<PostsController> _logger = logger;
		private readonly IMediator _mediator = mediator;

		/// <summary>
		/// Get all posts.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Ok with the list of all existing posts.</returns>
		[HttpGet]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Post>))]
		public async Task<IActionResult> GetAllPosts(CancellationToken cancellationToken)
		{
			var query = new GetAllPostsQuery();

			var allPosts = await _mediator.Send(query, cancellationToken);

			return Ok(allPosts);
		}


		/// <summary>
		/// Get post by ID.
		/// </summary>
		/// <param name="id">ID of the post to find.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Ok with post data if found, NotFound if no post with provided ID was found.</returns>
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

		/// <summary>
		/// Create new post.
		/// </summary>
		/// <param name="post">Post data.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Created with post details if post data provided is valid, BadRequest if validation errors occured.</returns>
		[HttpPost]
		[Produces(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Post))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AddPost(AddNewPostCommand post, CancellationToken cancellationToken)
		{
			var createdPost = await _mediator.Send(post, cancellationToken);

			return CreatedAtAction(nameof(AddPost), new { createdPost.Id }, createdPost);
		}

	}
}
