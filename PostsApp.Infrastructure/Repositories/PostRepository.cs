using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using System.Data;

public class PostRepository : IPostRepository
{
	private readonly string _connectionString;

	public PostRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	public async Task BulkAddAsync(ICollection<Post> posts, CancellationToken cancellationToken = default)
	{
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken);

		var checkCommand = new SqliteCommand("SELECT COUNT(*) FROM Posts", connection);

		var count = (long) await checkCommand.ExecuteScalarAsync(cancellationToken);
		if (count == 0)
		{
			using var transaction = connection.BeginTransaction();

			foreach (var post in posts)
			{
				var command = new SqliteCommand(
					"INSERT INTO Posts (Id, Title, ArticleLink, PublicationDate, Creator, Content, MediaUrl) VALUES (@Id, @Title, @ArticleLink, @PublicationDate, @Creator, @Content, @MediaUrl)",
					connection, transaction);

				command.Parameters.AddWithValue("@Id", post.Id);
				command.Parameters.AddWithValue("@Title", post.Title);
				command.Parameters.AddWithValue("@ArticleLink", post.ArticleLink ?? (object)DBNull.Value);
				command.Parameters.AddWithValue("@PublicationDate", post.PublicationDate ?? (object)DBNull.Value);
				command.Parameters.AddWithValue("@Creator", post.Creator ?? (object)DBNull.Value);
				command.Parameters.AddWithValue("@Content", post.Content);
				command.Parameters.AddWithValue("@MediaUrl", post.MediaUrl ?? (object)DBNull.Value);

				await command.ExecuteNonQueryAsync(cancellationToken);
			}

			transaction.Commit();
		}
	}
}