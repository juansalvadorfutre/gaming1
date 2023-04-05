using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Gaming.Status
{
    public class GameStatusHandler : IRequestHandler<GameStatusRequest, GameStatusResponse>
    {
        private readonly IRepository<Game> _gameRepository;

        public GameStatusHandler(IRepository<Game> gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<GameStatusResponse> Handle(GameStatusRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Game> games = await _gameRepository.GetAllAsync();
            Game currentGame = games?.FirstOrDefault();
            
            var response = new GameStatusResponse
            {
                Game = currentGame
            };
            return response;
        }
    }
}
