namespace Schema.Input
{
    public static class Topic
    {
        public const string CellClick = "Input.CellClick";
        public const string CellFlag = "Input.CellFlag";
        public const string CellSweep = "Input.CellSweep";
    }

    public struct CellClickMessage
    {
        public int X;
        public int Y;
    }
}