using Xunit;
using Domain.Entities;
using Domain.Enums;

namespace UnitTests.Domain
{
    public class GameClassShould
    {
        private Game _sut;
        public GameClassShould()
        {
            _sut = new Game(42, 3, GameStatus.Running);
        }

        [Fact]
        public void ChangeNumberOfPlayers_When_Calling_SetNumberOfPlayers()
        {
            _sut.SetNumberOfPlayers(4);
            Assert.Equal(4, _sut.NumberOfPlayers);
        }

        [Fact]
        public void ChangeMysteryNumber_When_Calling_SetMysteryNumber()
        {
            _sut.SetMysteryNumber(24);
            Assert.Equal(24, _sut.MysteryNumber);
        }

        [Fact]
        public void ChangeGameStatus_When_Calling_SetGameStatus()
        {
            _sut.SetGameStatus(GameStatus.GameOver);
            Assert.Equal(GameStatus.GameOver, _sut.GameStatus);
        }
    }
}
