using Application.Handlers.Gaming.Join;
using Application.Handlers.Gaming.Play;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using IntegrationTests.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Application.Handlers.Gaming
{
    public class PlayGameHandlerTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Game> _gameRepository;
        private readonly DatabaseFixture _databaseFixture;

        public PlayGameHandlerTests()
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
        public async Task Return_GameStatus_IsGameOver_When_Game_Status_Is_GameOver()
        {
            // Arrange
            var game = new Game();
            game.SetGameStatus(GameStatus.GameOver);
            await _gameRepository.AddAsync(game);

            // Act
            var request = new PlayGameRequest
            {
                Bet = 10
            };
            var response = await _mediator.Send(request);

            // Assert
            Assert.Equal(GameStatus.GameOver, response.GameStatus);
        }

        [Fact]
        public async Task Return_Bet_Result_Is_Lo_And_GameStatus_Is_Running_When_Bet_Is_Lower_Than_Mystery_Number()
        {
            // Arrange
            var mysteryNumber = 20;
            var numberOfPlayers = 2;
            var gameStatus = GameStatus.Running;
            var game = new Game(mysteryNumber, numberOfPlayers, gameStatus);
            await _gameRepository.AddAsync(game);

            // Act
            var request = new PlayGameRequest
            {
                Bet = 10
            };
            var response = await _mediator.Send(request);

            // Assert
            Assert.Equal(BetResult.Lo, response.BetResult);
            Assert.Equal(GameStatus.Running, response.GameStatus);
        }

        [Fact]
        public async Task Return_Bet_Result_Is_Hi_And_GameStatus_Is_Running_When_Bet_Is_Higher_Than_Mystery_Number()
        {
            // Arrange
            var mysteryNumber = 20;
            var numberOfPlayers = 2;
            var gameStatus = GameStatus.Running;
            var game = new Game(mysteryNumber, numberOfPlayers, gameStatus);
            await _gameRepository.AddAsync(game);

            // Act
            var request = new PlayGameRequest
            {
                Bet = 30
            };
            var response = await _mediator.Send(request);

            // Assert
            Assert.Equal(BetResult.Hi, response.BetResult);
            Assert.Equal(GameStatus.Running, response.GameStatus);
        }

        [Fact]
        public async Task Return_Bet_Result_Is_Success_And_GameStatus_Is_GameOver_When_Bet_Is_Equal_Than_Mystery_Number()
        {
            // Arrange
            var mysteryNumber = 20;
            var numberOfPlayers = 2;
            var gameStatus = GameStatus.Running;
            var game = new Game(mysteryNumber, numberOfPlayers, gameStatus);
            await _gameRepository.AddAsync(game);

            // Act
            var request = new PlayGameRequest
            {
                Bet = 20
            };
            var response = await _mediator.Send(request);

            // Assert
            Assert.Equal(BetResult.Success, response.BetResult);
            Assert.Equal(GameStatus.Running, response.GameStatus);

            var games = await _gameRepository.GetAllAsync();
            var currentGame = games?.FirstOrDefault();

            Assert.Equal(0, currentGame.NumberOfPlayers);
            Assert.Equal(GameStatus.GameOver, currentGame.GameStatus);
        }

        [Fact]
        public async Task Return_Just_One_Winner_When_Many_Players_Succeeded_In_The_Same_Moment()
        {
            // Arrange
            var mysteryNumber = 20;
            var numberOfPlayers = 1000;
            var gameStatus = GameStatus.Running;
            var game = new Game(mysteryNumber, numberOfPlayers, gameStatus);
            await _gameRepository.AddAsync(game);

            // Act
            var requestList = Enumerable.Range(0, numberOfPlayers)
                .Select(_ => new PlayGameRequest { Bet = 20 })
                .ToList();

            var responses = await Task.WhenAll(requestList.Select(async request =>
            {
                var response = await _mediator.Send(request);
                return response;
            }));

            var winners = responses.Where(r => r.GameStatus == GameStatus.Running && r.BetResult == BetResult.Success);
            var losers = responses.Where(r => r.GameStatus == GameStatus.GameOver);

            Assert.Equal(1, winners.Count());
            Assert.Equal(numberOfPlayers - 1, losers.Count());
        }
    }
}
