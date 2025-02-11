using api.Domain;

namespace api.Contracts;

public record UserMove(Guid UserId, string Username, Figure Figure);