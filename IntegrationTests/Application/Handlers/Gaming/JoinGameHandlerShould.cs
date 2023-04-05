using Application.Handlers.Gaming.Join;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using IntegrationTests.Application.Common;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Application.Handlers.Gaming
{
    public class JoinGameHandlerShould : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Game> _gameRepository;
        private readonly DatabaseFixture _databaseFixture;


        public JoinGameHandlerShould()
        {
            _databaseFixture = new DatabaseFixture();
            _mediator = _databaseFixture.Mediator;
            _gameRepository = _databaseFixture.GameRepository;
        }

        public void Dispose()
        {
            _databaseFixture.Dispose();
        }

        [Fact]
        public async Task JoinGameHandler_Should_Create_New_Game_When_No_Games_Exist()
        {
            // Arrange
            await _gameRepository.RemoveAllAsync(); // Ensure no games exist

            // Act
            var request = new JoinGameRequest();
            var response = await _mediator.Send(request);

            var games = await _gameRepository.GetAllAsync();
            var currentGame = games.FirstOrDefault();

            // Assert
            Assert.Equal(1, games.Count());
            Assert.Equal(1, currentGame.NumberOfPlayers);
            Assert.Equal(GameStatus.Running, currentGame.GameStatus);
            Assert.NotNull(response.Min);
            Assert.NotNull(response.Max);
        }


        [Fact]
        public async Task Add_Player_To_Existing_Game_When_Games_Exist()
        {
            // Arrange
            var mysteryNumber = 57;
            var numberOfPlayers = 1;
            var gameStatus = GameStatus.Running;
            var existingGame = new Game(mysteryNumber, numberOfPlayers, gameStatus);
            await _gameRepository.AddAsync(existingGame);

            // Act
            var request = new JoinGameRequest();
            var response = await _mediator.Send(request);

            // Assert
            var games = await _gameRepository.GetAllAsync();
            Assert.Single(games);
            var game = games.Single();
            Assert.Equal(numberOfPlayers + 1, game.NumberOfPlayers);
            Assert.Equal(GameStatus.Running, game.GameStatus);
            Assert.Equal(mysteryNumber, game.MysteryNumber);
            Assert.Equal(existingGame.Id, game.Id);
        }
    }
}
