using PostsApp.Domain.Entities;

namespace PostsApp.Application;

public interface IExternalApiService
{
	public Task<List<Post>> FetchExternalData();
}
