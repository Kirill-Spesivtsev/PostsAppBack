namespace PostsApp.Tests;

using FluentValidation.TestHelper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using PostsApp.API.Controllers;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using PostsApp.Domain.Entities;
using PostsApp.Domain.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class PostsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly PostsController _controller;
	private readonly PostValidator _validator;

	public PostsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new PostsController(_mediatorMock.Object);
		_validator = new PostValidator();
	}

	[Fact]
	public async Task GetAllPosts_ReturnsOk_WithListOfPosts()
	{
		// Arrange
		var posts = new List<Post>
		{
			new Post
			{
				Id = "d39abe75-cf60-4058-b31e-fcfe38fb04be",
				Title = "Some Title 1",
				ArticleLink = "http://example.com",
				PublicationDate = DateTime.UtcNow,
				Creator = "Some creator 1",
				Content = "Some big bunch of letters 1",
				MediaUrl = "http://example.com/image.jpg"
			},
			new Post
			{
				Id = "0e7e28de-88f0-4188-9176-8a9936f9a54c",
				Title = "Some Title 2",
				ArticleLink = "http://example.com",
				PublicationDate = DateTime.UtcNow,
				Creator = "Some creator 2",
				Content = "Some big bunch of letters 2",
				MediaUrl = "http://example.com/image.jpg"
			},
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPostsQuery>(), It.IsAny<CancellationToken>()))
					 .ReturnsAsync(posts);

		// Act
		var result = await _controller.GetAllPosts(CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<List<Post>>(okResult.Value);
		Assert.Equal(posts.Count, returnValue.Count);
	}

	[Fact]
	public async Task GetAllPosts_ReturnsOk_WithNoExistingPosts()
	{
		// Arrange
		var posts = new List<Post>();
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPostsQuery>(), It.IsAny<CancellationToken>()))
					 .ReturnsAsync(posts);

		// Act
		var result = await _controller.GetAllPosts(CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<List<Post>>(okResult.Value);
		Assert.Empty(returnValue);
	}

	[Fact]
	public async Task GetPostById_ReturnsOk_WithPostData_WhenPostExists()
	{
		// Arrange
		var postId = "85e5ce75-0f21-4199-9742-c80ac17a5d21";
		var post = new Post
		{
			Id = postId,
			Title = "Some Post",
			ArticleLink = "http://example.com",
			PublicationDate = DateTime.UtcNow,
			Creator = "Some Creator",
			Content = "Some Content",
			MediaUrl = "http://example.com/image.jpg"
		};
		_mediatorMock.Setup(m => m.Send(It.IsAny<GetPostByIdQuery>(), It.IsAny<CancellationToken>()))
					 .ReturnsAsync(post);

		// Act
		var result = await _controller.GetPostById(postId, CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<Post>(okResult.Value);
		Assert.Equal(postId, returnValue.Id);
		Assert.Equal(post.Title, returnValue.Title);
		Assert.Equal(post.ArticleLink, returnValue.ArticleLink);
		Assert.Equal(post.Creator, returnValue.Creator);
		Assert.Equal(post.Content, returnValue.Content);
		Assert.Equal(post.MediaUrl, returnValue.MediaUrl);
	}

	[Fact]
	public async Task GetPostById_ReturnsNotFound_WhenPostDoesNotExist()
	{
		// Arrange
		var postId = "b7544052-55bb-4f1c-8d50-fb44e46df4a6";

		var posts = new List<Post>
		{
			new Post
			{
				Id = "9675e624-e2cb-46c8-a319-9b0f5402d724",
				Title = "Some Title 1",
			}, 
		};

		_mediatorMock.Setup(m => m.Send(It.IsAny<GetPostByIdQuery>(), It.IsAny<CancellationToken>()))
					 .ThrowsAsync(new NotFoundException(nameof(Post), postId));

		// Act and Assert
		await Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetPostById(postId, CancellationToken.None));
	}

	[Fact]
	public async Task DeletePostById_ReturnsOk_WhenPostIsDeleted()
	{
		// Arrange
		var postId = "7acc6879-9b7b-401e-ab45-74702c868aeb";
		_mediatorMock.Setup(m => m.Send(It.IsAny<DeletePostByIdCommand>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _controller.DeletePostById(postId, CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkResult>(result);
		Assert.Equal(200, okResult.StatusCode);
	}

	[Fact]
	public async Task DeletePostById_ThrowsNotFoundException_WhenPostDoesNotExist()
	{
		// Arrange
		var postId = "9d86ad07-ae17-49e5-88ae-75fe6d67fa6f";
		_mediatorMock.Setup(m => m.Send(It.IsAny<DeletePostByIdCommand>(), It.IsAny<CancellationToken>()))
					 .ThrowsAsync(new NotFoundException(nameof(Post), postId));

		// Act & Assert
		await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeletePostById(postId, CancellationToken.None));
	}

	[Fact]
	public async Task AddPost_ReturnsCreated_WhenPostIsCreated()
	{
		// Arrange
		var newPost = new AddNewPostCommand 
		{ 
			Title = "Test Post",
			Content = "Test Content" 
		};
		var createdPost = new Post 
		{ 
			Id = "test-id", 
			Title = "Test Post", 
			Content = "Test Content" 
		};
		_mediatorMock.Setup(m => m.Send(It.IsAny<AddNewPostCommand>(), It.IsAny<CancellationToken>()))
					 .ReturnsAsync(createdPost);

		// Act
		var result = await _controller.AddPost(newPost, CancellationToken.None);

		// Assert
		var createdResult = Assert.IsType<CreatedAtActionResult>(result);
		Assert.Equal(201, createdResult.StatusCode);
		Assert.Equal(createdPost, createdResult.Value);
	}

	[Fact]
	public void AddNewPost_HasValidationError_WhenIdIsEmpty()
	{
		var post = new AddNewPostCommand { Id = string.Empty };
		var result = _validator.TestValidate(post);
		result.ShouldHaveValidationErrorFor(p => p.Id);
	}

	[Fact]
	public void AddNewPost_HasValidationError_WhenTitleIsEmpty()
	{
		var post = new AddNewPostCommand { Title = string.Empty };
		var result = _validator.TestValidate(post);
		result.ShouldHaveValidationErrorFor(p => p.Title);
	}

	[Fact]
	public void AddNewPost_HasValidationError__WhenTitleLengthIsTooSmall()
	{
		var post = new AddNewPostCommand { Title = "aa" };
		var result = _validator.TestValidate(post);
		result.ShouldHaveValidationErrorFor(p => p.Title);
	}

	[Fact]
	public void AddNewPost_HasValidationError_WhenArticleLinkIsInvalidUrl()
	{
		var post = new AddNewPostCommand { ArticleLink = "some invalid url" };
		var result = _validator.TestValidate(post);
		result.ShouldHaveValidationErrorFor(p => p.ArticleLink);
	}

	[Fact]
	public void AddNewPost_HasValidationError_WhenContentIsEmpty()
	{
		var post = new AddNewPostCommand { Content = string.Empty };
		var result = _validator.TestValidate(post);
		result.ShouldHaveValidationErrorFor(p => p.Content);
	}

	[Fact]
	public void AddNewPost_HasValidationError_WhenMediaUrlIsInvalidUrl()
	{
		var post = new AddNewPostCommand { MediaUrl = "some invalid url" };
		var result = _validator.TestValidate(post);
		result.ShouldHaveValidationErrorFor(p => p.MediaUrl);
	}

	[Fact]
	public void AddNewPost_DoesNotHaveValidationErrors_WhenPostIsValid()
	{
		var post = new AddNewPostCommand
		{
			Id = "eabe5a7f-2843-461f-906f-31c22f2a9713",
			Title = "Some Title",
			ArticleLink = "http://example.com",
			PublicationDate = DateTime.UtcNow,
			Creator = "Some Creator",
			Content = "Some big bunch of letters",
			MediaUrl = "http://example.com/image.jpg"
		};
		var result = _validator.TestValidate(post);
		result.ShouldNotHaveAnyValidationErrors();
	}

}