using Game.State;
using Schema.Game;
using Schema.Menu;

namespace Game.Logic
{
    public class RemainingBombLogic
    {
        [Topic(Schema.Game.Topic.RemainingBombsChange)]
        private ITopic<RemainingBombsMessage> remainingBombsTopic;

        private GameState s;

        public RemainingBombLogic(System system)
        {
            s = system.State.Game;
        }

        [SubscribesTo(Schema.Menu.Topic.NewGame)]
        private void HandleNewGame(NewGameMessage msg)
        {
            s.RemainingBombs = msg.BombCount;
            remainingBombsTopic.Publish(new RemainingBombsMessage
            {
                RemainingBombs = s.RemainingBombs
            });
        }

        [SubscribesTo(Schema.Game.Topic.CellStatusChange)]
        private void HandleStatusChange(StatusChangeMessage msg)
        {
            if (s.GameOver)
            {
                return;
            }
            if (msg.Status != CellStatus.Flag && msg.Previous != CellStatus.Flag)
            {
                return;
            }

            if (msg.Status == CellStatus.Flag)
            {
                s.RemainingBombs -= 1;
            }
            else if (msg.Previous == CellStatus.Flag)
            {
                s.RemainingBombs += 1;
            }

            remainingBombsTopic.Publish(new RemainingBombsMessage
            {
                RemainingBombs = s.RemainingBombs
            });
        }
    }
}