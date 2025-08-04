using Godot;
using System;

public partial class Main : Node2D
{
    // Life scenes
    private PackedScene lifeTypeToPlace;
    private PackedScene lifeScene;
    private PackedScene lifeSceneSolitary;
    private PackedScene lifeSceneSocial;

    // Managers
    private GridManager gridManager;
    private LifeManager lifeManager;

    // Store cell coordinates of mouse position
    private Vector2I? hoveredGridCell;

    // UI Buttons
    private Button buttonPlay;
    private Button buttonPause;
    private Button buttonReset;
    private Button buttonAverage;
    private Button buttonSolitary;
    private Button buttonSocial;

    // Timer
    private Timer lifeTimer;

    public override void _Ready()
    {
        base._Ready();

        // Initialize scenes and nodes to be accessed programmatically
        lifeScene = GD.Load<PackedScene>("res://scenes/life/LifeAverage.tscn");
        lifeSceneSolitary = GD.Load<PackedScene>("res://scenes/life/LifeSolitary.tscn");
        lifeSceneSocial = GD.Load<PackedScene>("res://scenes/life/LifeSocial.tscn");

        //Managers
        gridManager = GetNode<GridManager>("GridManager");
        lifeManager = GetNode<LifeManager>("GridManager/LifeManager");

        // UI Buttons
        buttonPlay = GetNode<Button>("UiRoot/ButtonPlay");
        buttonPause = GetNode<Button>("UiRoot/ButtonPause");
        buttonReset = GetNode<Button>("UiRoot/ButtonReset");
        buttonAverage = GetNode<Button>("UiRoot/ButtonAverage");
        buttonSolitary = GetNode<Button>("UiRoot/ButtonSolitary");
        buttonSocial = GetNode<Button>("UiRoot/ButtonSocial");

        // Timer to control generation iteration
        lifeTimer = GetNode<Timer>("LifeTimer");

        // Control game timer
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
        buttonReset.Pressed += () => resetLife();

        lifeTimer.Timeout += () => lifeManager.IterateLifeNodes();

        // Set type of life to be placed
        buttonAverage.Pressed += () => lifeTypeToPlace = lifeScene;
        buttonSolitary.Pressed += () => lifeTypeToPlace = lifeSceneSolitary;
        buttonSocial.Pressed += () => lifeTypeToPlace = lifeSceneSocial;

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

    private void PlaceLifeAtHoveredCellPosition()
    {
        var life = lifeTypeToPlace.Instantiate<Node2D>();
        lifeManager.AddChild(life);

        if (lifeTypeToPlace == lifeScene)
        {
            life.AddToGroup("average");
        }
        else if (lifeTypeToPlace == lifeSceneSolitary)
        {
            life.AddToGroup("solitary");
        }
        else if (lifeTypeToPlace == lifeSceneSocial)
        {
            life.AddToGroup("social");
        }
        else
        {
            GD.Print("error with grouping");
        }

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

    // Remove all the life nodes to reset the board
    private void resetLife()
    {
        GetTree().ReloadCurrentScene();
    }
}
