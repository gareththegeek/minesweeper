namespace Schema.Menu
{
    public static class Topic
    {
        public const string NewGame = "Menu.NewGame";
    }

    public struct NewGameMessage
    {
        public int Width;
        public int Height;
        public int BombCount;
    }
}