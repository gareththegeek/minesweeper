using Godot;
using Schema.Game;
using Schema.Main;
using Schema.Menu;

namespace Game
{
    public class Stats
    {
        private int remainingBombs;
        private float startTime;
        private int time;
        private bool running = false;

        [Topic(Schema.Game.Topic.RemainingBombsChange)]
        private ITopic<RemainingBombsMessage> remainingBombsTopic;
        [Topic(Schema.Game.Topic.GameTimeChange)]
        private ITopic<GameTimeMessage> gameTimeTopic;

        [SubscribesTo(Schema.Menu.Topic.NewGame)]
        private void HandleNewGame(NewGameMessage msg)
        {
            remainingBombs = msg.BombCount;
            remainingBombsTopic.Publish(new RemainingBombsMessage
            {
                RemainingBombs = remainingBombs
            });

            startTime = Time.GetTicksMsec();
            running = true;
            PublishTime();
        }

        [SubscribesTo(Schema.Game.Topic.GameOver)]
        private void HandleGameOver(GameOverMessage msg)
        {
            running = false;
        }

        [SubscribesTo(Schema.Main.Topic.Process)]
        private void HandleFrame(ProcessMessage msg)
        {
            if (!running)
            {
                return;
            }
            PublishTime();
        }

        private void PublishTime()
        {
            var timeMs = Time.GetTicksMsec() - startTime;
            var time = Mathf.FloorToInt(timeMs / 1000f);

            gameTimeTopic.Publish(new GameTimeMessage
            {
                Seconds = time
            });
        }

        [SubscribesTo(Schema.Game.Topic.CellStatusChange)]
        private void HandleStatusChange(StatusChangeMessage msg)
        {
            if (!running)
            {
                return;
            }
            if (msg.Status != CellStatus.Flag && msg.Previous != CellStatus.Flag)
            {
                return;
            }

            if (msg.Status == CellStatus.Flag)
            {
                remainingBombs -= 1;
            }
            else if (msg.Previous == CellStatus.Flag)
            {
                remainingBombs += 1;
            }

            remainingBombsTopic.Publish(new RemainingBombsMessage
            {
                RemainingBombs = remainingBombs
            });
        }
    }
}