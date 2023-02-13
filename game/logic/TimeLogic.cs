using Game.State;
using Godot;
using Schema.Game;
using Schema.Main;
using Schema.Menu;

namespace Game.Logic
{
    public class TimeLogic
    {
        [Topic(Schema.Game.Topic.GameTimeChange)]
        private ITopic<GameTimeMessage> gameTimeTopic;

        private GameState s;

        public TimeLogic(System system)
        {
            s = system.State.Game;
        }

        [SubscribesTo(Schema.Menu.Topic.NewGame)]
        private void HandleNewGame(NewGameMessage msg)
        {
            s.StartTime = Time.GetTicksMsec();
            PublishTime();
        }

        [SubscribesTo(Schema.Main.Topic.Process)]
        private void HandleFrame(ProcessMessage msg)
        {
            if (s.GameOver)
            {
                return;
            }
            PublishTime();
        }

        private void PublishTime()
        {
            var timeMs = Time.GetTicksMsec() - s.StartTime;
            var time = Mathf.FloorToInt(timeMs / 1000f);

            gameTimeTopic.Publish(new GameTimeMessage
            {
                Seconds = time
            });
        }
    }
}