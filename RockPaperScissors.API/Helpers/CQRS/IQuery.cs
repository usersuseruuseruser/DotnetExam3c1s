using MediatR;

namespace api.Helpers.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse> {}
