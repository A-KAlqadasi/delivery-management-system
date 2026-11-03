using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Modules.Common.Domain.Results;

namespace Modules.Common.Api.Extensions;

public static class ProblemExtensions
{
	public static IResult ToProblem(this List<Error> errors)
	{
		if (errors.Count == 0)
		{
			return Results.Problem();
		}

		if (errors.All(error => error.Type == ErrorKind.Validation))
		{
			return ValidationProblem(errors);
		}

		return Problem(errors[0]);
	}

	private static IResult Problem(Error error)
	{
		int statusCode = error.Type switch
		{
			ErrorKind.Conflict => StatusCodes.Status409Conflict,
			ErrorKind.Validation => StatusCodes.Status400BadRequest,
			ErrorKind.NotFound => StatusCodes.Status404NotFound,
			ErrorKind.Unauthorized => StatusCodes.Status403Forbidden,
			_ => StatusCodes.Status500InternalServerError,
		};

		return Results.Problem(statusCode: statusCode, title: error.Description);
	}

	private static IResult ValidationProblem(List<Error> errors)
	{
		Dictionary<string, string[]> errorsDict = errors.ToDictionary(e => e.Code, e => new[] { e.Description }, StringComparer.Ordinal);

		ValidationProblemDetails problemDetails = new ValidationProblemDetails(errorsDict)
		{
			Status = StatusCodes.Status400BadRequest
		};

		return Results.Json(problemDetails, statusCode: StatusCodes.Status400BadRequest);
	}
}
