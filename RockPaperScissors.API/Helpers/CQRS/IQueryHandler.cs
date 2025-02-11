using MediatR;

namespace api.Helpers.CQRS;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> {}
