using MediatR;
using PostsApp.Domain.Entities;
using System.Text.Json.Serialization;

namespace PostsApp.Application;

/// <summary>
/// Mediatr command to add a new post.
/// </summary>
public record AddNewPostCommand: IRequest<Post>
{
	public string Id { get; set; } = null!;

	public string Title { get; set; } = null!;

	public string? ArticleLink { get; set; }

	public DateTime? PublicationDate { get; set; }

	public string? Creator { get; set; }

	public string Content { get; set; } = null!;

	public string? MediaUrl { get; set; } = null!;
}