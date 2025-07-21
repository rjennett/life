using Godot;
using System;

public partial class Main : Node2D
{
    private PackedScene lifeScene;
    private GridManager gridManager;
    private Vector2I? hoveredGridCell;

    public override void _Ready()
    {
        base._Ready();

        // Initialize scenes and nodes to be accessed programmatically
        lifeScene = GD.Load<PackedScene>("res://scenes/life/Life.tscn");
        gridManager = GetNode<GridManager>("GridManager");
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        base._UnhandledInput(evt);

        if (evt.IsActionPressed("left click"))
        {
            PlaceLifeAtHoveredCellPosition();
        }
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        Vector2I gridPosition = gridManager.GetMouseGridCellPosition();
        hoveredGridCell = gridPosition;

    }

    private void PlaceLifeAtHoveredCellPosition()
    {
        var life = lifeScene.Instantiate<Node2D>();
        AddChild(life);

        life.GlobalPosition = hoveredGridCell.Value * 16;
    }

    private void RemoveLifeAtHoveredCellPosition()
    {
        // TODO: remove and free any life that existed in clicked cell if clicked again
    }

}
