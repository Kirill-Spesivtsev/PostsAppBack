using PostsApp.Domain.Entities;
using System.Data;
using System.Linq.Expressions;

namespace PostsApp.Domain.Abstractions;

public interface IPostRepository
{
	public Task<List<Post>> GetAllAsync(CancellationToken cancellationToken = default);

	public Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

	public Task AddAsync(Post post, CancellationToken cancellationToken = default);

	public Task RemoveAsync(string id, CancellationToken cancellationToken = default);

	public Task BulkAddAsync(ICollection<Post> posts, CancellationToken cancellationToken = default);
}
