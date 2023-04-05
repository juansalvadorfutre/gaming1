using MediatR;

namespace Application.Handlers.Gaming.Play
{
    public record PlayGameRequest : IRequest<PlayGameResponse>
    {
        public int? Bet { get; init; }
    }
}
