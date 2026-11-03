using Modules.Common.Domain.Results;

namespace Modules.Common.Application.Exceptions;
/// <summary>
/// later rename it to match your specific project
/// </summary>
public sealed class MyModularException : Exception
{

		public MyModularException(string requestName, Error? error, Exception? innerException = default)
			: base("Application exception", innerException)
		{
				RequestName = requestName;
				Error = error;
		}

		public MyModularException() : base("Application exception")
		{
		}

		public MyModularException(string? message) : base(message)
		{

		}

		public MyModularException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		public string? RequestName { get; }

		public Error? Error { get; }
}
