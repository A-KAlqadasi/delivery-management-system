namespace Modules.Common.Domain.Results;

public enum ErrorKind
{
	Failure,
	Unexpected,
	Validation,
	Conflict,
	NotFound,
	Unauthorized,
	Forbidden,
}
