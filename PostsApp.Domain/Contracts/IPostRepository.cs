using PostsApp.Domain.Entities;
using System.Data;
using System.Linq.Expressions;

namespace PostsApp.Domain.Abstractions;

/// <summary>
/// Interface for posts repository
/// </summary>
public interface IPostRepository
{
	/// <summary>
	/// Gets all posts from database.
	/// </summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>List of all posts.</returns>
	public Task<List<Post>> GetAllAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets post by provided ID from the database.
	/// </summary>
	/// <param name="id">ID of post.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Post with its data.</returns>
	public Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Adds new post to database.
	/// </summary>
	/// <param name="post">Post model data.</param>
	/// <param name="cancellationToken">Cancellation token.</param>

	public Task AddAsync(Post post, CancellationToken cancellationToken = default);

	/// <summary>
	/// Removes post from database.
	/// </summary>
	/// <param name="id">ID of post.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public Task RemoveAsync(string id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Bulk inserts posts collection into the database.
	/// </summary>
	/// <param name="posts">Posts collection.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public Task BulkAddAsync(ICollection<Post> posts, CancellationToken cancellationToken = default);
}
