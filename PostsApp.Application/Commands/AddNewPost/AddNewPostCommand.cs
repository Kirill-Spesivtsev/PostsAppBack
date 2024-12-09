using MediatR;
using PostsApp.Domain.Entities;
using System.Text.Json.Serialization;

namespace PostsApp.Application;

/// <summary>
/// Mediatr command to add a new post.
/// </summary>
public record AddNewPostCommand: IRequest<Post>
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = null!;

	[JsonPropertyName("title")]
	public string Title { get; set; } = null!;

	[JsonPropertyName("article_link")]
	public string? ArticleLink { get; set; }

	[JsonPropertyName("pub_date")]
	public DateTime? PublicationDate { get; set; }

	[JsonPropertyName("creator")]
	public string? Creator { get; set; }

	[JsonPropertyName("content")]
	public string Content { get; set; } = null!;

	[JsonPropertyName("media_url")]
	public string? MediaUrl { get; set; } = null!;
}