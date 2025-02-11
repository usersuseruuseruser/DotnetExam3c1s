﻿using MediatR;

namespace api.Helpers.CQRS;

public interface ICommand : IRequest {}

public interface ICommand<out TResponse> : IRequest<TResponse> {}
