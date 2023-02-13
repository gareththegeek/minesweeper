using System.Collections.Generic;
using Schema.Game;

namespace Game.State
{
    public class CellState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool HasBomb { get; set; } = false;
        public bool Revealed { get; set; } = false;
        public bool Flagged { get; set; } = false;
        public List<CellState> Neighbours { get; private set; } = new List<CellState>();
        public int NeighbourBombCount { get; set; } = 0;
        public CellStatus PreviousStatus { get; set; }

        public CellState(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}