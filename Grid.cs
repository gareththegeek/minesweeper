using Godot;
using Schema.Game;
using Schema.Menu;
using System;

public class Grid : GridContainer
{
    [Export(PropertyHint.File, "*.tscn")] private string cellSceneFile;

    private T Instantiate<T>(string sceneFile) where T : Node
    {
        var scene = ResourceLoader.Load(cellSceneFile) as PackedScene;
        return scene.Instance<T>();
    }

    private void AddCell(int x, int y)
    {
        var cell = Instantiate<Cell>(cellSceneFile);
        cell.X = x;
        cell.Y = y;
        AddChild(cell);
    }

    [SubscribesTo(Schema.Menu.Topic.NewGame)]
    private void HandleNewGame(NewGameMessage msg)
    {
        foreach (Node node in GetChildren())
        {
            RemoveChild(node);
            F.Unbind(node);
            node.QueueFree();
        }
        Columns = msg.Width;
        for (var y = 0; y < msg.Height; y++)
        {
            for (var x = 0; x < msg.Width; x++)
            {
                AddCell(x, y);
            }
        }
    }

    [SubscribesTo(Schema.Game.Topic.CellStatusChange)]
    private void HandleCellStatusChange(StatusChangeMessage msg)
    {
        var maxX = Columns - 1;
        var maxY = Mathf.CeilToInt(GetChildCount() / Columns) - 1;
        if (msg.X > maxX)
        {
            throw new ArgumentException($"Cell x position {msg.X} exceeds grid width {maxX + 1}");
        }
        if (msg.Y > maxY)
        {
            throw new ArgumentException($"Cell y position {msg.Y} exceeds grid height {maxY + 1}");
        }
        var child = GetChild(msg.Y * Columns + msg.X) as Cell;
        child.Status = msg.Status;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        F.Bind(this);
    }
}
