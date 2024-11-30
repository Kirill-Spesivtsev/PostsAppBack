using Microsoft.AspNetCore.Mvc;

namespace PostsApp.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PostsController : ControllerBase
	{

		private readonly ILogger<PostsController> _logger;

		public PostsController(ILogger<PostsController> logger)
		{
			_logger = logger;
		}

	}
}
