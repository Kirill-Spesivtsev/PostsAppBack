using PostsApp.Domain.Entities;

namespace PostsApp.Application;

/// <summary>
/// Interface for the ExternalApiService.
/// </summary>
public interface IExternalApiService
{
	/// <summary>
	/// Fetches list of posts from external API
	/// </summary>
	/// <returns>List of posts</returns>
	public Task<List<Post>> FetchExternalData();
}
