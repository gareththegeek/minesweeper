namespace Schema.Main
{
    public static class Topic
    {
        public const string Ready = "Main.Ready";
        public const string Process = "Main.Process";
    }
    
    public struct ProcessMessage
    {
        public float Delta;
    }
}