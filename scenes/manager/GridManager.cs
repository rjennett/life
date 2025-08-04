using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

public partial class GridManager : Node2D
{
    // Hashset to track living cells
    // One for each type of life?
    private HashSet<Vector2I> occupiedCells = new();

    // Store each life in a dictionary with its coords for easy access
    public Dictionary<Vector2I, Node2D> gridLife = new();

    private PackedScene lifeScene;
    private PackedScene lifeSceneSolitary;
    private PackedScene lifeSceneSocial;
    private Node2D mainNode;
    private LifeManager lifeManager;
    public List<Vector2I> nextGenerationLifeCoords = new();
    public List<Vector2I> nextGenerationDeathCoords = new();

    [Export]
    public TileMapLayer baseTerrainTileMapLayer;

    public override void _Ready()
    {
        base._Ready();
        lifeScene = GD.Load<PackedScene>("res://scenes/life/LifeAverage.tscn");
        lifeSceneSolitary = GD.Load<PackedScene>("res://scenes/life/LifeSolitary.tscn");
        lifeSceneSocial = GD.Load<PackedScene>("res://scenes/life/LifeSocial.tscn");
        mainNode = GetNode<Node2D>("/root/Main");
        lifeManager = GetNode<LifeManager>("/root/Main/GridManager/LifeManager");
    }


    // Get mouse grid cell position
    public Vector2I GetMouseGridCellPosition()
    {
        Vector2 mousePosition = baseTerrainTileMapLayer.GetGlobalMousePosition();
        Vector2 gridPosition = mousePosition / 16;
        gridPosition = gridPosition.Floor();
        return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
    }

    // Check for life
    public bool IsTileAlive(Vector2I tilePosition)
    {
        GD.Print("is alive tile position", tilePosition);
        return occupiedCells.Contains(tilePosition);
    }

    // Mark tile as alive by adding to HashSet occupiedTiles
    public void MarkTileAsAlive(Vector2I tilePosition, Node2D newLife)
    {
        occupiedCells.Add(tilePosition);
        gridLife.Add(tilePosition, newLife);
    }

    public void MarkTileAsDead(Vector2I tilePosition)
    {
        occupiedCells.Remove(tilePosition);
    }
}
