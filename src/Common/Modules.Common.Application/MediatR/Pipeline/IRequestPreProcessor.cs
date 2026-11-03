namespace Modules.Common.Application.MediatR.Pipeline;

public interface IRequestPreProcessor<in TRequest>
{
		Task Process(TRequest request, CancellationToken cancellationToken);
}
