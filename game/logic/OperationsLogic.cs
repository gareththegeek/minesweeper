using System;
using Game.State;
using Schema.Game;
using Schema.Input;

namespace Game.Logic
{
    public class OperationsLogic
    {
        [Topic(Schema.Game.Topic.GameOver)]
        private ITopic<GameOverMessage> gameOverTopic;

        private GameState s;
        private CellLogic cellLogic;

        public OperationsLogic(System system)
        {
            s = system.State.Game;
            cellLogic = system.CellLogic;
        }

        [SubscribesTo(Schema.Input.Topic.CellClick)]
        private void HandleCellClick(CellClickMessage msg)
        {
            if (s.GameOver)
            {
                return;
            }
            if (msg.X >= s.Width || msg.Y >= s.Height)
            {
                throw new ArgumentException($"Click coordinate outside game area {msg.X},{msg.Y} {s.Width}x{s.Height}");
            }

            cellLogic.SetRevealed(s.Cells[msg.X, msg.Y], true);
        }

        [SubscribesTo(Schema.Input.Topic.CellFlag)]
        private void HandleCellFlag(CellClickMessage msg)
        {
            if (s.GameOver)
            {
                return;
            }
            if (msg.X >= s.Width || msg.Y >= s.Height)
            {
                throw new ArgumentException($"Flag coordinate outside game area {msg.X},{msg.Y} {s.Width}x{s.Height}");
            }

            cellLogic.ToggleFlag(s.Cells[msg.X, msg.Y]);
        }

        [SubscribesTo(Schema.Input.Topic.CellSweep)]
        private void HandleCellSweep(CellClickMessage msg)
        {
            if (s.GameOver)
            {
                return;
            }
            if (msg.X >= s.Width || msg.Y >= s.Height)
            {
                throw new ArgumentException($"Click coordinate outside game area {msg.X},{msg.Y} {s.Width}x{s.Height}");
            }

            cellLogic.Sweep(s.Cells[msg.X, msg.Y]);
        }

        private void CheckWinCondition()
        {
            foreach (var cell in s.Cells)
            {
                if (cell.HasBomb && !cell.Flagged)
                {
                    return;
                }
                if (cell.Flagged && !cell.HasBomb)
                {
                    return;
                }
                if (!cell.Flagged && !cell.Revealed)
                {
                    return;
                }
            }
            gameOverTopic.Publish(new GameOverMessage
            {
                Winner = true
            });
            s.GameOver = true;
        }

        [SubscribesTo(Schema.Game.Topic.CellStatusChange)]
        private void HandleCellStatusChange(StatusChangeMessage msg)
        {
            if (msg.Status == CellStatus.BombBlown)
            {
                gameOverTopic.Publish(new GameOverMessage
                {
                    Winner = false
                });
                s.GameOver = true;
                return;
            }

            CheckWinCondition();
        }
    }
}