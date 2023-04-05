using Domain.Enums;

namespace Domain.Entities
{
    public class Game
    {
        public Game()
        {
        }
        public Game(int mysteryNumber, int numberOfPlayers, GameStatus gameStatus)
        {
            MysteryNumber = mysteryNumber;
            NumberOfPlayers = numberOfPlayers;
            GameStatus = gameStatus;
        }
        public int Id { get; init; }
        public int MysteryNumber { get; private set; }
        public int NumberOfPlayers { get; private set; }
        public GameStatus GameStatus { get; private set; }

        public void SetNumberOfPlayers(int numberOfPlayers)
        {
            NumberOfPlayers = numberOfPlayers;
        }

        public void SetMysteryNumber(int mysteryNumber)
        {
            MysteryNumber = mysteryNumber;
        }

        public void SetGameStatus(GameStatus gameStatus)
        {
            GameStatus = gameStatus;
        }
    }
}