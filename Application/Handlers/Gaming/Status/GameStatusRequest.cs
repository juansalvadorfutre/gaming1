using MediatR;

namespace Application.Handlers.Gaming.Status
{
    public record GameStatusRequest : IRequest<GameStatusResponse>;
}
