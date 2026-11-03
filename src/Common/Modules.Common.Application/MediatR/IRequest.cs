namespace Modules.Common.Application.MediatR;

#pragma warning disable S2326
public interface IRequest<TResponse>;
#pragma warning restore S2326

public interface IRequestHandler<in TRequest, TResponse>
		where TRequest : IRequest<TResponse>
{
		Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}


