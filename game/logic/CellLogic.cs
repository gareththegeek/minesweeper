using System.Linq;
using Game.State;
using Schema.Game;

namespace Game.Logic
{
    public class CellLogic
    {
        [Topic(Topic.CellStatusChange)]
        private ITopic<StatusChangeMessage> statusChangeTopic;

        private GameState s;

        public CellLogic(System system)
        {
            s = system.State.Game;
        }

        public CellStatus GetStatus(CellState s, bool blow = true)
        {
            if (!s.Revealed)
            {
                return s.Flagged ? CellStatus.Flag : CellStatus.Button;
            }
            if (s.HasBomb)
            {
                return blow ? CellStatus.BombBlown : CellStatus.Bomb;
            }
            if (s.Revealed && s.Flagged)
            {
                return CellStatus.FalseFlag;
            }
            return (CellStatus)s.NeighbourBombCount;
        }

        private void PublishStatus(CellState s)
        {
            var nextStatus = GetStatus(s);
            if (nextStatus == s.PreviousStatus)
            {
                return;
            }
            statusChangeTopic.Publish(
                new StatusChangeMessage
                {
                    X = s.X,
                    Y = s.Y,
                    Status = nextStatus,
                    Previous = s.PreviousStatus
                }
            );
            s.PreviousStatus = nextStatus;
        }

        public void SetBomb(CellState s, bool hasBomb)
        {
            s.HasBomb = hasBomb;
            PublishStatus(s);
        }

        public void SetRevealed(CellState s, bool revealed)
        {
            if (s.Revealed == revealed)
            {
                return;
            }
            if (s.Flagged)
            {
                return;
            }
            s.Revealed = revealed;
            PublishStatus(s);

            if (!revealed)
            {
                return;
            }
            if (s.NeighbourBombCount > 0)
            {
                return;
            }

            foreach (var neighbour in s.Neighbours)
            {
                SetRevealed(neighbour, true);
            }
        }

        public void ToggleFlag(CellState s)
        {
            if (s.Revealed)
            {
                return;
            }
            s.Flagged = !s.Flagged;
            PublishStatus(s);
        }

        public void AddNeighbour(CellState s, CellState neighbour)
        {
            s.Neighbours.Add(neighbour);
            if (neighbour.HasBomb)
            {
                s.NeighbourBombCount += 1;
            }
            PublishStatus(s);
        }

        private int GetNeighourFlagCount(CellState s) => s.Neighbours.Count(x => x.Flagged);

        public void Sweep(CellState s)
        {
            if (!s.Revealed)
            {
                return;
            }
            var neighbourFlagCount = GetNeighourFlagCount(s);
            if (neighbourFlagCount != s.NeighbourBombCount)
            {
                return;
            }
            foreach (var neighbour in s.Neighbours)
            {
                SetRevealed(neighbour, true);
            }
        }

        [SubscribesTo(Topic.CellStatusChange)]
        private void RevealUnexplodedBombs(StatusChangeMessage msg)
        {
            if (msg.Status != CellStatus.BombBlown)
            {
                return;
            }
            for (var y = 0; y < s.Height; y++)
            {
                for (var x = 0; x < s.Width; x++)
                {
                    if (msg.X == x && msg.Y == y)
                    {
                        continue;
                    }

                    var c = s.Cells[x, y];
                    if (!c.HasBomb && !c.Flagged)
                    {
                        continue;
                    }

                    c.PreviousStatus = GetStatus(c, false);
                    c.Revealed = true;
                    statusChangeTopic.Publish(
                        new StatusChangeMessage
                        {
                            X = c.X,
                            Y = c.Y,
                            Status = GetStatus(c, false),
                            Previous = c.PreviousStatus
                        }
                    );

                }
            }
        }
    }
}