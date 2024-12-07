

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PostsApp.Application;

/// <summary>
/// Pipeline behaviour to validate HTTP request parameters according to defined FluentValidation rules.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;
	private readonly ILogger<ValidationPipelineBehaviour<TRequest, TResponse>> _logger;

	public ValidationPipelineBehaviour(
		IEnumerable<IValidator<TRequest>> validators,
		ILogger<ValidationPipelineBehaviour<TRequest, TResponse>> logger)
	{
		_validators = validators;
		_logger = logger;
	}

	public Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (!_validators.Any())
			return next();

		var validationErrors = _validators
			.Select(validator => validator.Validate(request))
			.SelectMany(result => result.Errors)
			.Where(failure => failure != null)
			.Distinct()
			.ToList();

		if (validationErrors.Any())
		{
			_logger.LogInformation("Validation failed for {RequestType}: {@ValidationErrors}", typeof(TRequest).Name, validationErrors.Select(e=> e.ErrorMessage));
			throw new ValidationException(validationErrors);
		}

		return next();
	}
}
