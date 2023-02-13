using Godot;
using Schema.Main;
using System;

public class Main : Node
{
    [Topic(Schema.Main.Topic.Ready)]
    private ITopic<Message> readyTopic;
    [Topic(Schema.Main.Topic.Process)]
    private ITopic<ProcessMessage> processTopic;
    [Topic(Schema.Menu.Topic.NewGame)]
    private ITopic<Schema.Menu.NewGameMessage> newGameTopic;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        F.Bind(this);

        new SystemBoot().Boot();
        readyTopic.Publish(new Message());

        newGameTopic.Publish(new Schema.Menu.NewGameMessage
        {
            Width = 10,
            Height = 10,
            BombCount = 10
        });
    }

    private static ProcessMessage processMessage = new ProcessMessage();
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        processMessage.Delta = delta;
        processTopic.Publish(processMessage);
    }
}
