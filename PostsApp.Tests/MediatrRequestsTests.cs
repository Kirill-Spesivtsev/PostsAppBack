using AutoMapper;
using global::PostsApp.Application;
using global::PostsApp.Domain.Abstractions;
using global::PostsApp.Domain.Entities;
using global::PostsApp.Domain.Extensions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MemoryCache.Testing.Moq;
using System.Reflection.Metadata;

namespace PostsApp.Tests
{
	public class MediatrRequestsTests
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly IMemoryCache _memoryCache;
		private readonly Mock<IPostRepository> _postRepositoryMock;
		private readonly Mock<IExternalApiService> _externalApiServiceMock;
		private readonly GetAllPostsQueryHandler _handlerGetAllPostsMock;
		private readonly GetPostByIdQueryHandler _handlerGetPostByIdMock;
		private readonly AddNewPostCommandHandler _handlerAddNewPost;
		private readonly DeletePostByIdCommandHandler _handlerDeletePostById;
		private readonly Mock<IMapper> _mapperMock;

		public MediatrRequestsTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_postRepositoryMock = new Mock<IPostRepository>();
			_externalApiServiceMock = new Mock<IExternalApiService>();
			_memoryCache = Create.MockedMemoryCache();
			_mapperMock = new Mock<IMapper>();
			

			var loggerMockGetAll = new Mock<ILogger<GetAllPostsQueryHandler>>();
			var loggerMockGetById = new Mock<ILogger<GetPostByIdQueryHandler>>();
			var loggerMockAddNewPost = new Mock<ILogger<AddNewPostCommandHandler>>();
			var loggerMockDeletePostById = new Mock<ILogger<DeletePostByIdCommandHandler>>();

			_handlerGetAllPostsMock = new GetAllPostsQueryHandler(
				_postRepositoryMock.Object,
				_memoryCache,
				loggerMockGetAll.Object,
				_externalApiServiceMock.Object
			);
			_handlerGetPostByIdMock = new GetPostByIdQueryHandler(
				_postRepositoryMock.Object,
				_memoryCache,
				loggerMockGetById.Object
			);
			_handlerAddNewPost = new AddNewPostCommandHandler(
				_postRepositoryMock.Object,
				_memoryCache,
				_mapperMock.Object,
				loggerMockAddNewPost.Object
			);
			_handlerDeletePostById = new DeletePostByIdCommandHandler(
				_postRepositoryMock.Object,
				_memoryCache, 
				loggerMockDeletePostById.Object
			);
		}

		[Fact]
		public async Task HandleGetAllPosts_ReturnsPostsFromCache_WhenCacheIsNotEmpty()
		{
			// Arrange
			var cachedPosts = new List<Post>
			{
				new Post
				{
					Id = "22fdccbd-bbb0-4ef7-a211-85febe0addf9",
					Title = "Cached Post"
				}
			};

			_memoryCache.Set("Posts_All", cachedPosts);

			var query = new GetAllPostsQuery();

			// Act
			var result = await _handlerGetAllPostsMock.Handle(query, CancellationToken.None);
			var cacheEntries = _memoryCache.Get<IEnumerable<Post>>("Posts_All");

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(cachedPosts[0].Title, result[0].Title);

			Assert.NotNull(cacheEntries);
			Assert.Single(cacheEntries);
			Assert.Equal(cachedPosts[0].Title, cacheEntries.First().Title);
		}

		[Fact]
		public async Task HandleGetAllPosts_ReturnsPostsFromRepository_WhenCacheIsEmpty()
		{
			// Arrange
			var posts = new List<Post>
			{
				new Post
				{
					Id = "01899cf2-ed75-4933-bc68-021d73abeb20",
					Title = "Some Post"
				}
			};

			_memoryCache.Remove("Posts_All");

			_postRepositoryMock
				.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(posts);

			// Act
			var result = await _handlerGetAllPostsMock.Handle(new GetAllPostsQuery(), CancellationToken.None);
			var cacheEntries = _memoryCache.Get<IEnumerable<Post>>("Posts_All");

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(posts[0].Title, result[0].Title);

			Assert.NotNull(cacheEntries);
			Assert.Single(cacheEntries);
			Assert.Equal(posts[0].Title, cacheEntries.First().Title);
		}

		[Fact]
		public async Task HandleGetAllPosts_FetchesFromExternalApi_WhenRepositoryIsEmpty()
		{
			// Arrange
			var externalPosts = new List<Post>
			{
				new Post 
				{ 
					Id = "851a364e-a9f3-4e8f-8e3f-38eac2933338", 
					Title = "External Post" 
				}
			};

			_memoryCache.Remove("Posts_All");

			_postRepositoryMock
				.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Post>());

			_externalApiServiceMock
				.Setup(api => api.FetchExternalData())
				.ReturnsAsync(externalPosts);

			_postRepositoryMock
				.Setup(repo => repo.BulkAddAsync(It.IsAny<List<Post>>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _handlerGetAllPostsMock.Handle(new GetAllPostsQuery(), CancellationToken.None);
			var cacheEntries = _memoryCache.Get<IEnumerable<Post>>("Posts_All");

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(externalPosts[0].Title, result[0].Title);

			Assert.NotNull(cacheEntries);
			Assert.Single(cacheEntries);
			Assert.Equal(externalPosts[0].Title, cacheEntries.First().Title);
		}

		[Fact]
		public async Task HandleGetPostById_ReturnsPostFromCache_WhenPostExistsInCache()
		{
			// Arrange
			var postId = "2abbbf5c-8a23-499b-8a06-729a6efc6975";
			var post = new Post
			{
				Id = postId,
				Title = "Cached Post"
			};

			_memoryCache.Set($"Post_{postId}", post);

			// Act
			var result = await _handlerGetPostByIdMock.Handle(new GetPostByIdQuery(postId), CancellationToken.None);
			var cachedPost = _memoryCache.Get<Post>($"Post_{postId}");

			// Assert
			Assert.NotNull(result);
			Assert.Equal(postId, result.Id);
			Assert.Equal(post.Title, result.Title);

			Assert.NotNull(cachedPost);
			Assert.Equal(postId, cachedPost.Id);
			Assert.Equal(post.Title, cachedPost.Title);
		}

		[Fact]
		public async Task HandleGetPostById_ReturnsPostFromRepository_WhenCacheIsEmpty()
		{
			// Arrange
			var postId = "bb291b20-a548-4382-9720-8a42eae38dfe";
			var post = new Post 
			{ 
				Id = postId, 
				Title = "Some Post" 
			};

			_memoryCache.Remove($"Post_{postId}");

			_postRepositoryMock
				.Setup(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(post);

			// Act
			var result = await _handlerGetPostByIdMock.Handle(new GetPostByIdQuery(postId), CancellationToken.None);
			var cachePost = _memoryCache.Get<Post>($"Post_{postId}");

			// Assert
			Assert.NotNull(result);
			Assert.Equal(post.Id, result.Id);
			Assert.Equal(post.Title, result.Title);

			Assert.NotNull(cachePost);
			Assert.Equal(postId, cachePost.Id);
			Assert.Equal(post.Title, cachePost.Title);
		}

		[Fact]
		public async Task HandleGetPostById_ThrowsNotFoundException_WhenPostDoesNotExist()
		{
			// Arrange
			var postId = "819f84bb-6965-4206-b080-fab9a1dcf88f";

			_memoryCache.Remove($"Post_{postId}");

			_postRepositoryMock
				.Setup(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Post)null!);

			// Act and Assert
			var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
				await _handlerGetPostByIdMock.Handle(new GetPostByIdQuery(postId), CancellationToken.None));

			_postRepositoryMock.Verify(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task HandleAddNewPost_AddsNewPostToDatabaseCorrectly_WhenValidPostModelProvided()
		{
			// Arrange
			var postModel = new AddNewPostCommand
			{
				Id = "0a2580bf-0893-42b2-a92b-f418c753c16d",
				Title = "Some Title",
				ArticleLink = "http://example.com",
				PublicationDate = DateTime.UtcNow,
				Creator = "Some Creator",
				Content = "Some big bunch of letters",
				MediaUrl = "http://example.com/image.jpg"
			};

			var post = new Post
			{
				Id = postModel.Id,
				Title = postModel.Title,
				ArticleLink = postModel.ArticleLink,
				PublicationDate = postModel.PublicationDate,
				Creator = postModel.Creator,
				Content = postModel.Content,
				MediaUrl = postModel.MediaUrl
			};

			_mapperMock.Setup(m => m.Map<Post>(It.IsAny<AddNewPostCommand>())).Returns(post);

			_postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			// Act
			var addedPost = await _handlerAddNewPost.Handle(postModel, CancellationToken.None);

			// Assert
			_postRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);

			Assert.NotNull(addedPost);
			Assert.Equal(addedPost.Id, postModel.Id);
			Assert.Equal(addedPost.Title, postModel.Title);
			Assert.Equal(addedPost.Content, postModel.Content);
			Assert.Equal(addedPost.MediaUrl, postModel.MediaUrl);
			Assert.Equal(addedPost.ArticleLink, postModel.ArticleLink);
			Assert.Equal(addedPost.PublicationDate, postModel.PublicationDate);
			Assert.Equal(addedPost.Creator, postModel.Creator);
		}

		[Fact]
		public async Task HandleAddNewPost_AddsNewPostToCacheCorrectly_WhenValidPostModelProvided()
		{
			// Arrange
			var postId = "2cac4889-b679-485d-bf71-d9f530a3bcb7";

			var postModel = new AddNewPostCommand
			{
				Id = postId,
				Title = "Some Post",
				Content = "Some Content"
			};

			var mappedPost = new Post
			{
				Id = postId,
				Title = postModel.Title,
				Content = postModel.Content
			};

			_mapperMock.Setup(m => m.Map<Post>(It.IsAny<AddNewPostCommand>())).Returns(mappedPost);

			_postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			_memoryCache.Set<List<Post>>("Posts_All", []);

			// Act
			var post = await _handlerAddNewPost.Handle(postModel, CancellationToken.None);
			var cachePost = _memoryCache.Get<Post>($"Post_{postModel.Id}");
			var allCacheEntries = _memoryCache.Get<List<Post>>("Posts_All");

			// Assert
			_postRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
			
			Assert.NotNull(cachePost);
			Assert.Equal(postId, cachePost.Id);
			Assert.Equal(postModel.Title, cachePost.Title);

			Assert.NotNull(allCacheEntries);
			Assert.Contains(allCacheEntries, p => p.Id == postId && p.Title == postModel.Title);
		}

		[Fact]
		public async Task HandleAddNewPost_MapperMapsCorrectly_WhenValidPostModelProvided()
		{
			// Arrange
			var config = new MapperConfiguration(cfg => 
			{ 
				cfg.AddProfile<MappingProfile>(); 
			}); 
			IMapper mapper = config.CreateMapper();

			var postModel = new AddNewPostCommand
			{
				Id = "0a2580bf-0893-42b2-a92b-f418c753c16d",
				Title = "Some Title",
				ArticleLink = "http://example.com",
				PublicationDate = DateTime.UtcNow,
				Creator = "Some Creator",
				Content = "Some big bunch of letters",
				MediaUrl = "http://example.com/image.jpg"
			};

			// Act
			var post = mapper.Map<Post>(postModel);

			Assert.NotNull(post);
			Assert.Equal(post.Id, postModel.Id);
			Assert.Equal(post.Title, postModel.Title);
			Assert.Equal(post.ArticleLink, postModel.ArticleLink);
			Assert.Equal(post.PublicationDate, postModel.PublicationDate);
			Assert.Equal(post.Creator, postModel.Creator);
			Assert.Equal(post.Content, postModel.Content);
			Assert.Equal(post.MediaUrl, postModel.MediaUrl);
		}

		[Fact]
		public async Task HandleAddNewPost_DoesNotCachePostIfPosts_WhenAllDoesNotExist()
		{
			// Arrange
			var postId = "58a5c1ae-56a7-465b-b6b4-9ddeedfd96df";
			var postModel = new AddNewPostCommand
			{
				Title = "Some Post",
				Content = "Some Content"
			};
			var post = new Post
			{
				Id = postId,
				Title = postModel.Title,
				Content = postModel.Content
			};

			_memoryCache.Remove("Posts_All");

			_mapperMock.Setup(m => m.Map<Post>(postModel)).Returns(post);
			_postRepositoryMock.Setup(r => r.AddAsync(post, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			// Act
			var result = await _handlerAddNewPost.Handle(postModel, CancellationToken.None);
			var cachedPost = _memoryCache.Get<Post>($"Post_{postId}");
			var allPosts = _memoryCache.Get<List<Post>>("Posts_All");

			// Assert
			Assert.NotNull(result);
			Assert.Equal(postId, result.Id);

			Assert.NotNull(cachedPost);
			Assert.Equal(postId, cachedPost.Id);

			Assert.Null(allPosts);
		}

		[Fact]
		public async Task HandleAddNewPost_DoesNotCachePost_IfAddWasUnsuccessfull()
		{
			// Arrange
			var postId = "b9a96638-bd13-4f29-b645-788e72d92e13";
			var postModel = new AddNewPostCommand
			{
				Title = "Some Post",
				Content = "Some Content"
			};
			var post = new Post
			{
				Id = postId,
				Title = postModel.Title,
				Content = postModel.Content
			};

			_mapperMock.Setup(m => m.Map<Post>(postModel)).Returns(post);
			_postRepositoryMock.Setup(r => r.AddAsync(post, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Add failed"));

			// Act
			var exception = await Assert.ThrowsAsync<Exception>(() => _handlerAddNewPost.Handle(postModel, CancellationToken.None));
			var cachedPost = _memoryCache.Get<Post>($"Post_{postId}");
			var allPosts = _memoryCache.Get<List<Post>>("Posts_All");

			// Assert
			Assert.Equal("Add failed", exception.Message);
			Assert.Null(cachedPost);
			Assert.Null(allPosts);
		}

		[Fact]
		public async Task HandleDeletePostById_DeletesPostCorrectly_WhenPostExists()
		{
			// Arrange
			var postId = "59758f6d-28da-4504-b25f-b809feaab128";
			var post = new Post 
			{ 
				Id = postId 
			};
			var request = new DeletePostByIdCommand(postId);

			_postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(post);
			_postRepositoryMock.Setup(repo => repo.RemoveAsync(postId, It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			_memoryCache.Set($"Post_{postId}", post);
			_memoryCache.Set("Posts_All", new List<Post> { post });

			// Act
			await _handlerDeletePostById.Handle(request, CancellationToken.None);
			var removedPost = _memoryCache.Get<Post>($"Post_{postId}");
			var postsInCache = _memoryCache.Get<List<Post>>("Posts_All");

			// Assert
			_postRepositoryMock.Verify(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()), Times.Once);
			_postRepositoryMock.Verify(repo => repo.RemoveAsync(postId, It.IsAny<CancellationToken>()), Times.Once);

			Assert.Null(removedPost);
			Assert.DoesNotContain(post, postsInCache!);
		}

		[Fact]
		public async Task HandleDeletePostById_ThrowsNotFoundException_WhenPostDoesNotExist()
		{
			// Arrange
			var postId = "8a06aa8b-fbe6-479c-a6cf-c28065eac21c";
			var request = new DeletePostByIdCommand(postId);

			_postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(null as Post);

			// Act and Assert
			var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handlerDeletePostById.Handle(request, CancellationToken.None));
			Assert.Contains(postId, exception.Message);
		}

		[Fact]
		public async Task HandleDeletePostById_RemovesPostFromCache_WhenPostExists()
		{
			// Arrange
			var postId = "9a9bab79-4317-4275-ae16-8e45e3d39f5d";
			var post = new Post { Id = postId };
			var request = new DeletePostByIdCommand(postId);

			_postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(post);
			_postRepositoryMock.Setup(repo => repo.RemoveAsync(postId, It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);


			// Act
			_memoryCache.Set($"Post_{postId}", post);
			_memoryCache.Set("Posts_All", new List<Post> { post });

			await _handlerDeletePostById.Handle(request, CancellationToken.None);

			var removedPost = _memoryCache.Get<Post>($"Post_{postId}");
			var postsInCache = _memoryCache.Get<List<Post>>("Posts_All");

			// Assert

			Assert.Null(removedPost);
			Assert.DoesNotContain(post, postsInCache!); 
		}

	}
}
