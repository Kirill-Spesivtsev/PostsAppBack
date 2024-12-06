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
	}
}
