using Godot;
using System;
using System.Collections.Generic;
using Schema.Game;
using Schema.Input;

public class Cell : TextureButton
{
    [Export] private Texture buttonTexture;
    [Export] private Texture clearedTexture;
    [Export] private Texture bombTexture;
    [Export] private Texture blownTexture;
    [Export] private Texture flagTexture;
    [Export] private Texture oneTexture;
    [Export] private Texture twoTexture;
    [Export] private Texture threeTexture;
    [Export] private Texture fourTexture;
    [Export] private Texture fiveTexture;
    [Export] private Texture sixTexture;
    [Export] private Texture sevenTexture;
    [Export] private Texture eightTexture;
    [Export] private Texture falseFlagTexture;

    [Topic(Schema.Input.Topic.CellFlag)]
    private ITopic<CellClickMessage> cellFlagTopic;

    [Topic(Schema.Input.Topic.CellSweep)]
    private ITopic<CellClickMessage> cellSweepTopic;

    [Topic(Schema.Input.Topic.CellClick)]
    private ITopic<CellClickMessage> cellClickTopic;

    private List<Texture> numberTextures;

    private CellStatus status;

    public int X { get; set; }
    public int Y { get; set; }

    public CellStatus Status
    {
        get => status;
        set
        {
            status = value;
            TextureNormal = StatusToTexture(status);
        }
    }

    private Texture StatusToTexture(CellStatus status)
    {
        switch (status)
        {
            case CellStatus.Button:
                return buttonTexture;
            case CellStatus.Flag:
                return flagTexture;
            case CellStatus.Clear:
                return clearedTexture;
            case CellStatus.Bomb:
                return bombTexture;
            case CellStatus.BombBlown:
                return blownTexture;
            case CellStatus.FalseFlag:
                return falseFlagTexture;
            default:
                return numberTextures[(int)status];
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        numberTextures = new List<Texture> { clearedTexture, oneTexture, twoTexture, threeTexture, fourTexture, fiveTexture, sixTexture, sevenTexture, eightTexture };
        F.Bind(this);
    }

    public override void _GuiInput(InputEvent @event)
    {
        var e = @event as InputEventMouseButton;
        if (e == null)
        {
            return;
        }

        var pressed = e.IsPressed();
        var isLeft = e.ButtonIndex == (int)ButtonList.Left;
        var isRight = e.ButtonIndex == (int)ButtonList.Right;
        var wasLeft = Input.IsMouseButtonPressed((int)ButtonList.Left);
        var wasRight = Input.IsMouseButtonPressed((int)ButtonList.Right);

        if (pressed && isRight && !wasLeft)
        {
            cellFlagTopic.Publish(new CellClickMessage
            {
                X = X,
                Y = Y
            });
            return;
        }

        if (!pressed && isLeft && !wasRight)
        {
            cellClickTopic.Publish(new CellClickMessage
            {
                X = X,
                Y = Y
            });
            return;
        }

        if (!pressed)
        {
            if ((isLeft && wasRight) || (isRight && wasLeft))
            {
                cellSweepTopic.Publish(new CellClickMessage
                {
                    X = X,
                    Y = Y
                });
                return;
            }
        }
    }
}
