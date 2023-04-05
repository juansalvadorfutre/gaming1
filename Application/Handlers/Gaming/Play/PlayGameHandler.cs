using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Handlers.Gaming.Play
{
    public class PlayGameHandler : IRequestHandler<PlayGameRequest, PlayGameResponse>
    {
        private readonly IRepository<Game> _gameRepository;
        private readonly IConfiguration _configuration;
        private static readonly Random _random = new Random();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PlayGameHandler(IRepository<Game> gameRepository,
                                IConfiguration configuration)
        {
            _gameRepository = gameRepository;
            _configuration = configuration;
        }

        public async Task<PlayGameResponse> Handle(PlayGameRequest request, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                IEnumerable<Game> games = await _gameRepository.GetAllAsync();
                var currentGame = games?.FirstOrDefault();

                PlayGameResponse response = new PlayGameResponse();
                if (currentGame.GameStatus == GameStatus.GameOver)
                {
                    response.SetGameStatus(GameStatus.GameOver);
                    return response;
                }
                
                if (request.Bet > currentGame?.MysteryNumber)
                {
                    response.SetBetResult(BetResult.Hi);
                    response.SetGameStatus(GameStatus.Running);
                }
                else if (request.Bet < currentGame?.MysteryNumber)
                {
                    response.SetBetResult(BetResult.Lo);
                    response.SetGameStatus(GameStatus.Running);
                }
                else
                {
                    response.SetBetResult(BetResult.Success);
                    response.SetGameStatus(GameStatus.Running);                    
                    await ResetGameAsync(currentGame);
                }

                return response;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task ResetGameAsync(Game currentGame)
        {
            var min = Convert.ToInt32(_configuration.GetSection("HiloGame")["min"]);
            var max = Convert.ToInt32(_configuration.GetSection("HiloGame")["max"]);
            var mysteryNumber = _random.Next(min, max);

            currentGame.SetNumberOfPlayers(0);
            currentGame.SetMysteryNumber(mysteryNumber);
            currentGame.SetGameStatus(GameStatus.GameOver);
            
            await _gameRepository.UpdateAsync(currentGame);
        }
    }
}
