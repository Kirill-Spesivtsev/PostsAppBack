using PostsApp.Application;
using PostsApp.Domain.Abstractions;
using System.Reflection;
using System.Reflection.Metadata;

namespace PostsApp.API.Configuration;

public static class ApplicationConfiguration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
	{
		builder.Services.AddProblemDetails();

		services.AddMediatR(configuration =>
			configuration.RegisterServicesFromAssemblyContaining<ExternalApiService>());

		builder.Services.AddHttpClient<ExternalApiService>();

		builder.Services.AddScoped<IPostRepository, PostRepository>(sp =>
		{
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			return new PostRepository(connectionString!);
		});
		builder.Services.AddScoped<IExternalApiService, ExternalApiService>();

		return services;
	}
}