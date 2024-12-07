using FluentValidation;
using MediatR;
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

		services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
		services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));

		services.AddValidatorsFromAssemblyContaining<ExternalApiService>( includeInternalTypes: true);

		builder.Services.AddHttpClient<ExternalApiService>();

		services.AddAutoMapper(typeof(ExternalApiService).Assembly);

		builder.Services.AddScoped<IPostRepository, PostRepository>(sp => 
			new PostRepository(builder.Configuration.GetConnectionString("DefaultConnection")!)
		);

		builder.Services.AddScoped<IExternalApiService, ExternalApiService>();

		return services;
	}
}