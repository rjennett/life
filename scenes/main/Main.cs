using Godot;
using System;

public partial class Main : Node2D
{
    private PackedScene lifeScene;
    private PackedScene lifeSceneSolitary;
    private PackedScene lifeSceneSocial;
    private GridManager gridManager;
    private Vector2I? hoveredGridCell;
    private Button buttonPlay;
    private Button buttonPause;
    private Button buttonReset;
    private Button buttonAverage;
    private Button buttonSolitary;
    private Button buttonSocial;
    private Timer lifeTimer;
    private string lifeType;

    public override void _Ready()
    {
        base._Ready();

        // Initialize scenes and nodes to be accessed programmatically
        lifeScene = GD.Load<PackedScene>("res://scenes/life/Life.tscn");
        lifeSceneSolitary = GD.Load<PackedScene>("");
        lifeSceneSocial = GD.Load<PackedScene>("");
        gridManager = GetNode<GridManager>("GridManager");
        buttonPlay = GetNode<Button>("UiRoot/ButtonPlay");
        buttonPause = GetNode<Button>("UiRoot/ButtonPause");
        buttonReset = GetNode<Button>("UiRoot/ButtonReset");
        buttonAverage = GetNode<Button>("UiRoot/ButtonAverage");
        buttonSolitary = GetNode<Button>("UiRoot/ButtonSolitary");
        buttonSocial = GetNode<Button>("UiRoot/ButtonSocial");
        lifeTimer = GetNode<Timer>("LifeTimer");

        buttonPlay.Pressed += () =>
        {
            if (!lifeTimer.Paused)
            {
                lifeTimer.Start();
            }
            else
            {
                lifeTimer.Paused = false;
            }
        };
        buttonPause.Pressed += () => lifeTimer.Paused = true;
        buttonAverage.Pressed += () =>
        lifeTimer.Timeout += () => gridManager.IterateLifeGrid();
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        base._UnhandledInput(evt);

        // Handle clicks to place and remove life
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
    
    public void SetLifeType(string lifeType)
    {
        switch (lifeType)
        {
            case "Average":

                break;
        }
    }
}
