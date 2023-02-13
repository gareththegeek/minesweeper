using Godot;
using Schema.Game;
using System;

public class TimeLabel : Label
{
    [SubscribesTo(Schema.Game.Topic.GameTimeChange)]
    private void HandleTimeChange(GameTimeMessage msg)
    {
        Text = msg.Seconds.ToString("000");
    }

    public override void _Ready()
    {
        F.Bind(this);
    }
}
