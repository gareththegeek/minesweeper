using Godot;
using Schema.Game;
using System;

public class BombsLabel : Label
{
    [SubscribesTo(Topic.RemainingBombsChange)]
    private void HandleRemainingBombsChange(RemainingBombsMessage msg)
    {
        Text = msg.RemainingBombs.ToString("000");
    }

    public override void _Ready()
    {
        F.Bind(this);
    }
}
