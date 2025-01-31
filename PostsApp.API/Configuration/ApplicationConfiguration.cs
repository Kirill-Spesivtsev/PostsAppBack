﻿using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using System.Reflection;
using System.Reflection.Metadata;

namespace PostsApp.API.Configuration;

public static class ApplicationConfiguration
{
	/// <summary>
	/// Extenstion method for application services configuration.
	/// </summary>
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
	{
		var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
		services.AddCors(options => 
		{ 
			options.AddPolicy("DefaultCorsPolicy", builder => builder.WithOrigins(allowedOrigins!).AllowAnyHeader().AllowAnyMethod()); 
		}); 

		builder.Services.AddProblemDetails();

		services.AddMediatR(configuration =>
			configuration.RegisterServicesFromAssemblyContaining<ExternalApiService>());

		services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
		services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));

		services.AddValidatorsFromAssemblyContaining<ExternalApiService>( includeInternalTypes: true);

		builder.Services.AddHttpClient();

		builder.Services.AddMemoryCache(options =>
		{
			options.SizeLimit = 500;
		});

		services.AddAutoMapper(typeof(ExternalApiService).Assembly);

		builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Posts API v1",
					Description = "API to manage Posts",
					License = new OpenApiLicense() { Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT") }
				});
				var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, fileName));
			}
		);

		builder.Services.AddScoped<IPostRepository, PostRepository>(sp => 
			new PostRepository(builder.Configuration.GetConnectionString("DefaultConnection")!)
		);

		builder.Services.AddScoped<IExternalApiService, ExternalApiService>();

		return services;
	}
}