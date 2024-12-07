using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PostsApp.Domain.Extensions;
using System.Net;

namespace PostsApp.API.Middlewares;

/// <summary>
/// Exception handler middleware for handling and logging all application exceptions.
/// </summary>
public class GlobalExceptionHandler
{
	private readonly RequestDelegate _next;
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (ValidationException ex)
		{
			var errorsList = ex.Errors
				.GroupBy(f => f.PropertyName)
				.ToDictionary(
					g => g.Key,
					g => g.Select(f => f.ErrorMessage).ToArray());


			var problemDetails = new ProblemDetails
			{
				Title = "Validation Failed",
				Detail = "One or more validation errors occurred.",
				Status = StatusCodes.Status400BadRequest,
				Extensions =
				{
					["errors"] = errorsList
				}
			};

			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsJsonAsync(problemDetails);
		}
		catch (HttpResponseException ex)
		{
			_logger.LogWarning(ex.Message);

			var problemDetails = new ProblemDetails
			{
				Status = ex.Status,
				Type = ex.Type,
				Title = ex.Title,
				Detail = ex.Message
			};

			context.Response.StatusCode = ex.Status;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsJsonAsync(problemDetails);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Exception was thrown");

			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
				Title = "Internal Server Error",
				Detail = ex.Message
			};

			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}
