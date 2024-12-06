using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PostsApp.Domain.Entities;

public class Post
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[JsonPropertyName("title")]
	public string Title { get; set; } = null!;

	[JsonPropertyName("article_link")]
	public string? ArticleLink { get; set; }

	[JsonPropertyName("pub_date")]
	public DateTime? PublicationDate { get; set; } = DateTime.UtcNow;

	[JsonPropertyName("creator")]
	public string? Creator { get; set; }

	[JsonPropertyName("content")]
	public string Content { get; set; } = null!;

	[JsonPropertyName("media_url")]
	public string? MediaUrl { get; set; } = null!;
}
