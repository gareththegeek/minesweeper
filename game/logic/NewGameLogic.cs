using System;
using Game.State;
using Godot;
using Schema.Game;
using Schema.Menu;

namespace Game.Logic
{
    public class NewGameLogic
    {
        [Topic(Schema.Game.Topic.GameInitialised)]
        private ITopic<GameInitialisedMessage> gameInitialisedTopic;

        private GameState s;
        private System system;

        public NewGameLogic(System system)
        {
            this.system = system;
            s = system.State.Game;
        }

        [SubscribesTo(Schema.Menu.Topic.NewGame)]
        private void HandleNewGame(NewGameMessage msg)
        {
            if (msg.BombCount > msg.Width * msg.Height)
            {
                throw new ArgumentException($"Too many bombs and not enough space {msg.BombCount} bombs {msg.Width}x{msg.Height}");
            }

            MakeCells(msg);
            s.Width = msg.Width;
            s.Height = msg.Height;
            PlaceBombs(msg);
            AssignNeighbours(msg);

            s.GameOver = false;
            gameInitialisedTopic.Publish(new GameInitialisedMessage { Width = s.Width, Height = s.Height });
        }

        private void MakeCells(NewGameMessage msg)
        {
            s.Cells = new CellState[msg.Width, msg.Height];
            for (var y = 0; y < msg.Height; y++)
            {
                for (var x = 0; x < msg.Width; x++)
                {
                    s.Cells[x, y] = new CellState(x, y);
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
                var cell = s.Cells[x, y];
                if (cell.HasBomb)
                {
                    continue;
                }

                system.CellLogic.SetBomb(cell, true);
                remainingBombs -= 1;
            }
        }

        private void AssignNeighbours(CellState cell)
        {
            for (var y = cell.Y - 1; y <= cell.Y + 1; y++)
            {
                for (var x = cell.X - 1; x <= cell.X + 1; x++)
                {
                    if (x < 0 || x >= s.Width || y < 0 || y >= s.Height)
                    {
                        continue;
                    }
                    if (x == cell.X && y == cell.Y)
                    {
                        continue;
                    }
                    system.CellLogic.AddNeighbour(cell, s.Cells[x, y]);
                }
            }
        }

        private void AssignNeighbours(NewGameMessage msg)
        {
            for (var y = 0; y < msg.Height; y++)
            {
                for (var x = 0; x < msg.Width; x++)
                {
                    AssignNeighbours(s.Cells[x, y]);
                }
            }
        }
    }
}