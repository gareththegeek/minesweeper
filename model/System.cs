using Godot;
using System;
using Schema.Menu;
using Schema.Game;
using Schema.Input;

namespace Game
{
    [SubSystem]
    public class System : IBootable
    {
        private static System instance;

        [Topic(Schema.Game.Topic.GameInitialised)]
        private ITopic<GameInitialisedMessage> gameInitialisedTopic;
        [Topic(Schema.Game.Topic.GameOver)]
        private ITopic<GameOverMessage> gameOverTopic;

        private int width;
        private int height;
        private Cell[,] cells;
        private Stats stats;
        private bool gameOver = false;

        public void Boot()
        {
            instance = this;
            stats = F.Create<Stats>();
        }

        private void MakeCells(NewGameMessage msg)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    F.Unbind(cells[x, y]);
                }
            }

            cells = new Cell[msg.Width, msg.Height];
            for (var y = 0; y < msg.Height; y++)
            {
                for (var x = 0; x < msg.Width; x++)
                {
                    cells[x, y] = F.Create<Cell>(x, y);
                }
            }
        }

        private void PlaceBombs(NewGameMessage msg)
        {
            GD.Randomize();
            var remainingBombs = msg.BombCount;
            while (remainingBombs > 0)
            {
                var x = GD.Randi() % msg.Width;
                var y = GD.Randi() % msg.Height;
                var cell = cells[x, y];
                if (cell.HasBomb)
                {
                    continue;
                }

                cell.SetBomb(true);
                remainingBombs -= 1;
            }
        }

        private void AssignNeighbours(Cell cell)
        {
            for (var y = cell.Y - 1; y <= cell.Y + 1; y++)
            {
                for (var x = cell.X - 1; x <= cell.X + 1; x++)
                {
                    if (x < 0 || x >= width || y < 0 || y >= height)
                    {
                        continue;
                    }
                    if (x == cell.X && y == cell.Y)
                    {
                        continue;
                    }
                    cell.AddNeighbour(cells[x, y]);
                }
            }
        }

        private void AssignNeighbours(NewGameMessage msg)
        {
            for (var y = 0; y < msg.Height; y++)
            {
                for (var x = 0; x < msg.Width; x++)
                {
                    AssignNeighbours(cells[x, y]);
                }
            }
        }

        [SubscribesTo(Schema.Menu.Topic.NewGame)]
        private void NewGame(NewGameMessage msg)
        {
            if (msg.BombCount > msg.Width * msg.Height)
            {
                throw new ArgumentException($"Too many bombs and not enough space {msg.BombCount} bombs {msg.Width}x{msg.Height}");
            }

            MakeCells(msg);
            width = msg.Width;
            height = msg.Height;
            PlaceBombs(msg);
            AssignNeighbours(msg);

            gameOver = false;
            gameInitialisedTopic.Publish(new GameInitialisedMessage { Width = width, Height = height });
        }

        [SubscribesTo(Schema.Input.Topic.CellClick)]
        private void HandleCellClick(CellClickMessage msg)
        {
            if (gameOver)
            {
                return;
            }
            if (msg.X >= width || msg.Y >= height)
            {
                throw new ArgumentException($"Click coordinate outside game area {msg.X},{msg.Y} {width}x{height}");
            }

            cells[msg.X, msg.Y].SetRevealed(true);
        }

        [SubscribesTo(Schema.Input.Topic.CellFlag)]
        private void HandleCellFlag(CellClickMessage msg)
        {
            if (gameOver)
            {
                return;
            }
            if (msg.X >= width || msg.Y >= height)
            {
                throw new ArgumentException($"Flag coordinate outside game area {msg.X},{msg.Y} {width}x{height}");
            }

            cells[msg.X, msg.Y].ToggleFlag();
        }

        [SubscribesTo(Schema.Input.Topic.CellSweep)]
        private void HandleCellSweep(CellClickMessage msg)
        {
            if (gameOver)
            {
                return;
            }
            if (msg.X >= width || msg.Y >= height)
            {
                throw new ArgumentException($"Click coordinate outside game area {msg.X},{msg.Y} {width}x{height}");
            }

            cells[msg.X, msg.Y].Sweep();
        }

        private void CheckWinCondition()
        {
            foreach (var cell in cells)
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
            gameOver = true;
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
                gameOver = true;
                return;
            }

            CheckWinCondition();
        }
    }
}