﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostsApp.Application;
using PostsApp.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;

namespace PostsApp.Application;

public class ExternalApiService(IConfiguration configuration, HttpClient httpClient, ILogger<ExternalApiService> logger) : IExternalApiService
{
	private readonly IConfiguration _configuration = configuration;
	private readonly HttpClient _httpClient = httpClient;
	private readonly ILogger<ExternalApiService> _logger = logger;

	/// <summary>
	/// Fetches list of posts from the external API URL, provided in configuration.
	/// </summary>
	/// <returns>Fetched data in the form of posts list.</returns>
	public async Task<List<Post>> FetchExternalData()
	{
		try
		{
			string apiUrl = _configuration["DataFetchApiUrl"]!;
			string json = await _httpClient.GetStringAsync(apiUrl);

			using JsonDocument doc = JsonDocument.Parse(json);
			JsonElement root = doc.RootElement;
			JsonElement dataArray = root.GetProperty("data");

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			List<Post> posts = JsonSerializer.Deserialize<List<Post>>(dataArray.GetRawText(), options)!;

			return posts;
		}
		catch (Exception) 
		{
			_logger.LogError("Failed to fetch data from the external API.");
			return [];
		}
	}
}
