using Godot;
using System;
using Manager.GridManager;

public partial class Main : Node2D
{
    private PackedScene lifeScene;
    private GridManager gridManager;
    private Vector2I? hoveredGridCell;
    private Button playButton;
    private Timer lifeTimer;

    public override void _Ready()
    {
        base._Ready();

        // Initialize scenes and nodes to be accessed programmatically
        lifeScene = GD.Load<PackedScene>("res://scenes/life/Life.tscn");
        gridManager = GetNode<GridManager>("GridManager");
        playButton = GetNode<Button>("UserInterface/PlayButton");
        lifeTimer = GetNode<Timer>("LifeTimer");
        

        playButton.Pressed += () => lifeTimer.Start();
        lifeTimer.Timeout += () => gridManager.IterateLifeGrid();
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        base._UnhandledInput(evt);

        if (evt.IsActionPressed("left click") && !gridManager.IsTileAlive(gridManager.GetMouseGridCellPosition()))
        {
            PlaceLifeAtHoveredCellPosition();
        }
        else if (evt.IsActionPressed("left click") && gridManager.IsTileAlive(gridManager.GetMouseGridCellPosition()))
        {
            RemoveLifeAtHoveredCellPosition();
        }
    }
    

    // Executes every frame
    public override void _Process(double delta)
    {
        base._Process(delta);

        Vector2I gridPosition = gridManager.GetMouseGridCellPosition();
        hoveredGridCell = gridPosition;

    }

    // TODO: refactor to place life at argument position for use elsewhere
    private void PlaceLifeAtHoveredCellPosition()
    {
        var life = lifeScene.Instantiate<Node2D>();
        AddChild(life);

        // Position converted to global values
        life.GlobalPosition = hoveredGridCell.Value * 16;
        // Pass local position to store in life dictionary
        gridManager.MarkTileAsAlive(hoveredGridCell.Value, life);
    }

    private void RemoveLifeAtHoveredCellPosition()
    {
        // TODO: remove and free any life that existed in clicked cell if clicked again
        var key = hoveredGridCell.Value;
        if (gridManager.gridLife.ContainsKey(key))
        {
            Node2D life = gridManager.gridLife[key];
            life.QueueFree();
            gridManager.gridLife.Remove(key);
            gridManager.MarkTileAsDead(key);
        }
    }

}
