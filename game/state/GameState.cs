namespace Game.State
{
    public class GameState
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public CellState[,] Cells { get; set; }
        public bool GameOver { get; set; } = false;
        public int RemainingBombs { get; set; }
        public float StartTime { get; set; }
        public int Time { get; set; }
    }
}