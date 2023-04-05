using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Handlers.Gaming.Join
{
    public class JoinGameHandler : IRequestHandler<JoinGameRequest, JoinGameResponse>
    {
        private readonly IRepository<Game> _gameRepository;
        private readonly IConfiguration _configuration;
        private static readonly Random _random = new Random();

        public JoinGameHandler(IRepository<Game> gameRepository,
                                IConfiguration configuration)
        {
            _gameRepository = gameRepository;
            _configuration = configuration;
        }

        public async Task<JoinGameResponse> Handle(JoinGameRequest request, CancellationToken cancellationToken)
        {
            Game currentGame;
            var min = _configuration.GetSection("HiloGame")["min"];
            var max = _configuration.GetSection("HiloGame")["max"];
            IEnumerable<Game> games = await _gameRepository.GetAllAsync();
            if (!games.Any())
            {
                var mysteryNumber = _random.Next(Convert.ToInt32(min), Convert.ToInt32(max));
                var numberOfPlayers = 1;
                var gameStatus = GameStatus.Running;
                currentGame = new Game(mysteryNumber, numberOfPlayers,gameStatus);
                await _gameRepository.AddAsync(currentGame);
            }
            else 
            {
                currentGame = games.FirstOrDefault();
                currentGame.SetNumberOfPlayers(currentGame.NumberOfPlayers + 1);
                currentGame.SetGameStatus(GameStatus.Running);
                await _gameRepository.UpdateAsync(currentGame);
            }

            var response = new JoinGameResponse
            {
                Min = min,
                Max = max
            };
            return response;
        }
    }
}
