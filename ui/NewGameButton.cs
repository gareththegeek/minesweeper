using Godot;
using Schema.Game;
using Schema.Menu;
using System;

public class NewGameButton : TextureButton
{
    [Export] private Texture normalTexture;
    [Export] private Texture scaredTexture;
    [Export] private Texture deadTexture;
    [Export] private Texture coolTexture;

    [Topic(Schema.Menu.Topic.NewGame)]
    private ITopic<NewGameMessage> newGameTopic;

    [SubscribesTo(Schema.Game.Topic.GameOver)]
    private void HandleGameOver(GameOverMessage msg)
    {
        TextureNormal = msg.Winner ? coolTexture : deadTexture;
    }

    public override void _Pressed()
    {
        TextureNormal = normalTexture;
        newGameTopic.Publish(new NewGameMessage
        {
            BombCount = 10,
            Width = 10,
            Height = 10
        });
    }

    public override void _Ready()
    {
        F.Bind(this);
    }
}
