using Application.Handlers.Gaming.Status;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using IntegrationTests.Application.Common;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Application.Handlers.Gaming
{
    public class GameStatusHandlerShould : IClassFixture<DatabaseFixture>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Game> _gameRepository;

        public GameStatusHandlerShould(DatabaseFixture databaseFixture)
        {
            _mediator = databaseFixture.Mediator;
            _gameRepository = databaseFixture.GameRepository;
        }

        [Fact]
        public async Task Return_GameStatusResponse_With_CurrentGame_When_GameExists()
        {
            // Arrange
            await _gameRepository.RemoveAllAsync();
            var expectedGame = new Game(25, 2, GameStatus.Running);
            await _gameRepository.AddAsync(expectedGame);

            // Act
            var request = new GameStatusRequest();
            var response = await _mediator.Send(request);

            // Assert
            Assert.NotNull(response.Game);
            Assert.Equal(expectedGame.MysteryNumber, response.Game.MysteryNumber);
            Assert.Equal(expectedGame.NumberOfPlayers, response.Game.NumberOfPlayers);
            Assert.Equal(expectedGame.GameStatus, response.Game.GameStatus);
        }

        [Fact]
        public async Task Return_GameStatusResponse_With_NullGame_When_NoGameExists()
        {
            // Arrange
            await _gameRepository.RemoveAllAsync();

            // Act
            var request = new GameStatusRequest();
            var response = await _mediator.Send(request);

            // Assert
            Assert.Null(response.Game);
        }
    }
}
