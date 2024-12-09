using FluentValidation;
using PostsApp.Application;
using PostsApp.Domain.Entities;


/// <summary>
/// Fluent validator for Post entity.
/// </summary>
public class PostValidator : AbstractValidator<AddNewPostCommand>
{
	public PostValidator()
	{
		RuleFor(post => post.Id)
			.NotEmpty()
			.WithMessage("Post ID should not be empty.");

		RuleFor(post => post.Title)
			.NotEmpty()
			.WithMessage("Title is required.")
			.MinimumLength(3)
			.WithMessage("Title should be at least 3 characters long.");

		RuleFor(post => post.ArticleLink)
			.Must(BeAValidUrl)
			.When(post => !string.IsNullOrEmpty(post.ArticleLink))
			.WithMessage("Article link should be a valid URL or empty.");

		RuleFor(post => post.Content)
			.NotEmpty()
			.WithMessage("Content is required.")
			.MinimumLength(10)
			.WithMessage("Content should be at least 10 characters long.");

		RuleFor(post => post.MediaUrl)
			.Must(BeAValidUrl)
			.When(post => !string.IsNullOrEmpty(post.MediaUrl))
			.WithMessage("MediaUrl should be a valid URL or empty.");
	}

	private bool BeAValidUrl(string? url)
	{
		return Uri.TryCreate(url, UriKind.Absolute, out _);
	}

	static bool BeAValidGuid(string? guid) 
	{ 
		return Guid.TryParse(guid, out _); 
	}
}
