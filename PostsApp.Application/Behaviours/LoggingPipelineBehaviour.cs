using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostsApp.Application;

/// <summary>
/// Pipeline behaviour for logging each HTTP request.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class LoggingPipelineBehaviour<TRequest, TResponse>(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
		: IPipelineBehavior<TRequest, TResponse>
			where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger = logger;

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;
		var serializedRequest = JsonSerializer.Serialize(request);

		_logger.LogInformation(
			"Processing request {RequestName}, {@RequestParameters}",
			requestName,
			serializedRequest);

		TResponse result = await next();

		_logger.LogInformation("Completed request {RequestName}", requestName);

		return result;
	}
}