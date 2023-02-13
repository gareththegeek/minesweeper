using System.Collections.Generic;
using System.Linq;
using Schema.Game;

namespace Game
{
    public class Cell
    {
        [Topic(Topic.CellStatusChange)]
        private ITopic<StatusChangeMessage> statusChangeTopic;

        private int x;
        private int y;
        private bool hasBomb = false;
        private bool revealed = false;
        private bool flagged = false;
        private List<Cell> neighbours = new List<Cell>();
        private int neighbourBombCount = 0;
        private CellStatus previousStatus;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get => x; }
        public int Y { get => y; }

        public bool HasBomb
        {
            get => hasBomb;
            set => SetBomb(value);
        }

        public bool Revealed
        {
            get => revealed;
            set => SetRevealed(value);
        }

        public bool Flagged
        {
            get => flagged;
        }

        public int NeighbourBombCount
        {
            get => neighbourBombCount;
        }

        public CellStatus GetStatus(bool blow = true)
        {
            if (!revealed)
            {
                return flagged ? CellStatus.Flag : CellStatus.Button;
            }
            if (hasBomb)
            {
                return blow ? CellStatus.BombBlown : CellStatus.Bomb;
            }
            if (revealed && flagged)
            {
                return CellStatus.FalseFlag;
            }
            return (CellStatus)neighbourBombCount;
        }

        private void PublishStatus()
        {
            var nextStatus = GetStatus();
            if (nextStatus == previousStatus)
            {
                return;
            }
            statusChangeTopic.Publish(
                new StatusChangeMessage
                {
                    X = x,
                    Y = y,
                    Status = nextStatus,
                    Previous = previousStatus
                }
            );
            previousStatus = nextStatus;
        }

        public void SetBomb(bool hasBomb)
        {
            this.hasBomb = hasBomb;
            PublishStatus();
        }

        public void SetRevealed(bool revealed)
        {
            if (this.revealed == revealed)
            {
                return;
            }
            if (flagged)
            {
                return;
            }
            this.revealed = revealed;
            PublishStatus();

            if (!revealed)
            {
                return;
            }
            if (neighbourBombCount > 0)
            {
                return;
            }

            foreach (var neighbour in neighbours)
            {
                neighbour.SetRevealed(true);
            }
        }

        public void ToggleFlag()
        {
            if (revealed)
            {
                return;
            }
            this.flagged = !flagged;
            PublishStatus();
        }

        public void AddNeighbour(Cell neighbour)
        {
            neighbours.Add(neighbour);
            if (neighbour.hasBomb)
            {
                neighbourBombCount += 1;
            }
            PublishStatus();
        }

        private int GetNeighourFlagCount() => neighbours.Count(x => x.Flagged);

        public void Sweep()
        {
            if (!revealed)
            {
                return;
            }
            var neighbourFlagCount = GetNeighourFlagCount();
            if (neighbourFlagCount != neighbourBombCount)
            {
                return;
            }
            foreach (var neighbour in neighbours)
            {
                neighbour.SetRevealed(true);
            }
        }

        [SubscribesTo(Topic.CellStatusChange)]
        private void HandleStatusChange(StatusChangeMessage msg)
        {
            if (msg.Status != CellStatus.BombBlown)
            {
                return;
            }
            if (!hasBomb && !flagged)
            {
                return;
            }
            if (msg.X == X && msg.Y == Y)
            {
                return;
            }
            previousStatus = GetStatus(false);
            revealed = true;
            statusChangeTopic.Publish(
                new StatusChangeMessage
                {
                    X = x,
                    Y = y,
                    Status = GetStatus(false),
                    Previous = previousStatus
                }
            );
        }
    }
}