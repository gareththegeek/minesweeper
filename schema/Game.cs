namespace Schema.Game
{
    public static class Topic
    {
        public const string CellStatusChange = "Game.CellStatusChange";
        public const string GameInitialised = "Game.GameInitialised";
        public const string RemainingBombsChange = "Game.RemainingBombsChange";
        public const string GameTimeChange = "Game.GameTimeChange";
        public const string GameOver = "Game.GameOver";
    }

    public enum CellStatus
    {
        Clear = 0,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Button,
        Flag,
        Bomb,
        BombBlown,
        FalseFlag,
        Count
    }

    public struct StatusChangeMessage
    {
        public int X;
        public int Y;
        public CellStatus Status;
        public CellStatus Previous;
    }

    public struct GameInitialisedMessage
    {
        public int Width;
        public int Height;
    }

    public struct RemainingBombsMessage
    {
        public int RemainingBombs;
    }

    public struct GameTimeMessage
    {
        public int Seconds;
    }

    public struct GameOverMessage
    {
        public bool Winner;
    }
}