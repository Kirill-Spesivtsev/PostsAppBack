using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PostsApp.Domain.Entities;

/// <summary>
/// Post entity
/// </summary>
public class Post
{

	/// <summary>
	/// Post Id
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	/// Post Title
	/// </summary>
	[JsonPropertyName("title")]
	public string Title { get; set; } = null!;

	/// <summary>
	/// Link to the original post
	/// </summary>
	[JsonPropertyName("article_link")]
	public string? ArticleLink { get; set; }

	/// <summary>
	/// Date and time of creation
	/// </summary>
	[JsonPropertyName("pub_date")]
	public DateTime? PublicationDate { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Creator name
	/// </summary>
	[JsonPropertyName("creator")]
	public string? Creator { get; set; }

	/// <summary>
	/// Main content of the post
	/// </summary>
	[JsonPropertyName("content")]
	public string Content { get; set; } = null!;

	/// <summary>
	/// Link to image if any
	/// </summary>
	[JsonPropertyName("media_url")]
	public string? MediaUrl { get; set; } = null!;
}
