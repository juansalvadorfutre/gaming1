using Domain.Entities;
using Domain.Enums;

namespace Application.Handlers.Gaming.Play
{
    public class PlayGameResponse
    {
        public BetResult BetResult { get; private set; }
        public GameStatus GameStatus{ get; private set; }

        public void SetBetResult(BetResult betResult)
        {
            BetResult = betResult;
        }

        public void SetGameStatus(GameStatus gameStatus)
        {
            GameStatus = gameStatus;
        }
    }
}
