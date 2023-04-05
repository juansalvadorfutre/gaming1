using MediatR;

namespace Application.Handlers.Gaming.Join
{
    public record JoinGameRequest : IRequest<JoinGameResponse>;
}
