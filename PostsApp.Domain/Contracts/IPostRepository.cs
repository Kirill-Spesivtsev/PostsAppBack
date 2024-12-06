using PostsApp.Domain.Entities;
using System.Data;
using System.Linq.Expressions;

namespace PostsApp.Domain.Abstractions;

public interface IPostRepository
{
	public Task BulkAddAsync(ICollection<Post> posts, CancellationToken cancellationToken = default);
}
