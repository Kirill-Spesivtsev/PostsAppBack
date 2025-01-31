﻿using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using System.Data;

/// <summary>
/// Repository for posts.
/// </summary>
public class PostRepository : IPostRepository
{
	private readonly string _connectionString;

	public PostRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <summary>
	/// Gets all posts from database.
	/// </summary>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>List of all posts.</returns>
	public async Task<List<Post>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var posts = new List<Post>();

		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken);

		var command = new SqliteCommand("SELECT * FROM Posts", connection);

		using var reader = await command.ExecuteReaderAsync(cancellationToken);
		while (await reader.ReadAsync(cancellationToken))
		{
			posts.Add(new Post
			{
				Id = reader["Id"].ToString()!,
				Title = reader["Title"].ToString()!,
				ArticleLink = reader["ArticleLink"] as string,
				PublicationDate = reader["PublicationDate"] as DateTime?,
				Creator = reader["Creator"] as string,
				Content = reader["Content"].ToString()!,
				MediaUrl = reader["MediaUrl"] as string
			});
		}
			
		return posts;
	}

	/// <summary>
	/// Gets post by provided ID from the database.
	/// </summary>
	/// <param name="id">ID of post.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Post with its data.</returns>
	public async Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken);

		var command = new SqliteCommand("SELECT * FROM Posts WHERE Id = @Id", connection);
		command.Parameters.AddWithValue("@Id", id);

		using var reader = await command.ExecuteReaderAsync(cancellationToken);
		if (await reader.ReadAsync(cancellationToken))
		{
			return new Post
			{
				Id = reader["Id"].ToString()!,
				Title = reader["Title"].ToString()!,
				ArticleLink = reader["ArticleLink"] as string,
				PublicationDate = reader["PublicationDate"] as DateTime?,
				Creator = reader["Creator"] as string,
				Content = reader["Content"].ToString()!,
				MediaUrl = reader["MediaUrl"] as string
			};
		}
			
		return null;
	}

	/// <summary>
	/// Adds new post to database.
	/// </summary>
	/// <param name="post">Post model data.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
	{
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken);

		var command = new SqliteCommand(
			"INSERT INTO Posts (Id, Title, ArticleLink, PublicationDate, Creator, Content, MediaUrl) VALUES (@Id, @Title, @ArticleLink, @PublicationDate, @Creator, @Content, @MediaUrl)",
			connection);

		command.Parameters.AddWithValue("@Id", post.Id);
		command.Parameters.AddWithValue("@Title", post.Title);
		command.Parameters.AddWithValue("@ArticleLink", post.ArticleLink ?? (object)DBNull.Value);
		command.Parameters.AddWithValue("@PublicationDate", post.PublicationDate ?? (object)DBNull.Value);
		command.Parameters.AddWithValue("@Creator", post.Creator ?? (object)DBNull.Value);
		command.Parameters.AddWithValue("@Content", post.Content);
		command.Parameters.AddWithValue("@MediaUrl", post.MediaUrl ?? (object)DBNull.Value);

		await command.ExecuteNonQueryAsync(cancellationToken);
		
	}

	/// <summary>
	/// Removes post from database.
	/// </summary>
	/// <param name="id">ID of post.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
	{
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken);

		var command = new SqliteCommand("DELETE FROM Posts WHERE Id = @Id", connection);
		command.Parameters.AddWithValue("@Id", id);

		await command.ExecuteNonQueryAsync(cancellationToken);
	}

	/// <summary>
	/// Bulk inserts posts collection into the database.
	/// </summary>
	/// <param name="posts">Posts collection.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task BulkAddAsync(ICollection<Post> posts, CancellationToken cancellationToken = default)
	{
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken);

		var checkCommand = new SqliteCommand("SELECT COUNT(*) FROM Posts", connection);

		var count = (long) (await checkCommand.ExecuteScalarAsync(cancellationToken))!;
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