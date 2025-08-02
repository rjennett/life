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

    private PackedScene lifeTypeToPlace;
    private PackedScene lifeScene;
    private PackedScene lifeSceneSolitary;
    private PackedScene lifeSceneSocial;
    private Node2D mainNode;
    private LifeManager lifeManager;
    private List<Vector2I> nextGenerationLifeCoords = new();
    private List<Vector2I> nextGenerationDeathCoords = new();
    private List<Vector2I> nextSolitaryLifeCoords = new();
    private List<Vector2I> nextSolitaryDeathCoords = new();
    private List<Vector2I> nextSocialLifeCoords = new();
    private List<Vector2I> nextSocialDeathCoords = new();

    [Export]
    private TileMapLayer baseTerrainTileMapLayer;

    public override void _Ready()
    {
        base._Ready();
        lifeScene = GD.Load<PackedScene>("res://scenes/life/Life.tscn");
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

    #region Life Management
    public void IterateLifeGrid()
    {
        // Iterate grid
        for (int i = 0; i < baseTerrainTileMapLayer.GetUsedRect().Size.X; i++)
        {
            for (int j = 0; j < baseTerrainTileMapLayer.GetUsedRect().Size.Y; j++)
            {
                Vector2I position = new Vector2I(i, j);
                GD.Print("Position: ", position);
                int livingNeighbors = AssessNeighbors(position);

                // TODO: get type of node at position, branch generation calculation based on this
                // if nodeInstance is lifeScene:
                foreach (Node2D child in lifeManager.GetChildren())
                {
                    // Narrow computation by group
                    // GD.Print(child.GetGroups());
                    if (child.IsInGroup("average"))
                    {
                        // Check each child for current coord
                        // if it is, that means it's the current one
                        // GD.Print(child.Position);
                        // GD.Print(position);
                        if ((child.Position / 16) == position)
                        {
                            // use the average rules
                            // GD.Print("calcing average...");
                            CalculateNextGeneration(livingNeighbors, position);
                            foreach (Vector2I item in occupiedCells)
                            {
                                GD.Print(item);
                            }
                            // break;
                        }
                    }
                    // repeat for other rules
                    else if (child.IsInGroup("solitary"))
                    {
                        // GD.Print(child.GetGroups());
                        // GD.Print(child.Position);
                        // GD.Print(position);
                        if ((child.Position / 16) == position)
                        {
                            // use the average rules
                            // GD.Print("calcing solitary...");
                            CalculateSolitaryGeneration(livingNeighbors, position);
                            // break;
                        }
                    }
                    else if (child.IsInGroup("social"))
                    {
                        GD.Print(child.GetGroups());
                        break;
                    }
                    else
                    {
                        GD.Print("Unhandled condition");
                    }
                }

                // Calculate for cells that do not have nodes on them
                CalculateNextGeneration(livingNeighbors, position);
                CalculateSolitaryGeneration(livingNeighbors, position);
            }
        }

        UpdateGeneration();
        UpdateSolitaryGeneration();
    }

    // Check neighbors for life
    public int AssessNeighbors(Vector2I currentGridPosition)
    {
        // Neighbor cell positions
        // GD.Print("Judging neighbors...");

        // Iterate all 8 neighbors to be evaluated
        List<TileSet.CellNeighbor> neighbors = [
            TileSet.CellNeighbor.RightSide,
            TileSet.CellNeighbor.BottomRightCorner,
            TileSet.CellNeighbor.BottomSide,
            TileSet.CellNeighbor.BottomLeftCorner,
            TileSet.CellNeighbor.LeftSide,
            TileSet.CellNeighbor.TopLeftCorner,
            TileSet.CellNeighbor.TopSide,
            TileSet.CellNeighbor.TopRightCorner
        ];

        int livingNeighbors = 0;
        foreach (TileSet.CellNeighbor neighbor in neighbors)
        {
            Vector2I cell = baseTerrainTileMapLayer.GetNeighborCell(currentGridPosition, neighbor);

            // Is the neighbor alive?
            if (IsTileAlive(cell))
            {
                GD.Print($"Neighbor {neighbor} Lives!");
                // Add to list of living neighbor locations/coords
                // Increment count of living neighbors
                livingNeighbors++;
            }
        }

        GD.Print("Living neighbors: ", livingNeighbors);

        return livingNeighbors;

    }

    // Change state
    private void CalculateNextGeneration(int countLivingNeighbors, Vector2I selfCoord)
    {
        // GD.Print("selfCoord: ", selfCoord);
        // GD.Print("Living neighbors: ", countLivingNeighbors);
        // GD.Print("Tile is alive?: ", IsTileAlive(selfCoord));
        GD.Print("CALUCLATE AVERAGE");
        GD.Print("Received position: ", selfCoord);

        // Rules for life
        // if dead && livingNeighbors == 3: become alive
        if (!IsTileAlive(selfCoord) && (countLivingNeighbors == 3))
        // if (!IsTileAlive(selfCoord))
        {
            // GD.Print(selfCoord);
            nextGenerationLifeCoords.Add(selfCoord);
            GD.Print("Condition 1");
        }
        // if alive && livingNeighbors == 2 or 3: remain alive
        else if (IsTileAlive(selfCoord) && ((countLivingNeighbors == 2) || (countLivingNeighbors == 3)))
        {
            // Condition does nothing?
            GD.Print("Condition 2");
            return;
        }
        // if alive && livingNeighbors < 2: die
        else if (IsTileAlive(selfCoord) && (countLivingNeighbors < 2))
        {
            nextGenerationDeathCoords.Add(selfCoord);
            GD.Print("Condition 3");
        }
        // if alive && livingNeighbors > 3: die
        else if (IsTileAlive(selfCoord) && (countLivingNeighbors > 3))
        {
            nextGenerationDeathCoords.Add(selfCoord);
            GD.Print("Condition 4");
        }
    }

    private void CalculateSolitaryGeneration(int countLivingNeighbors, Vector2I selfCoord)
    {
        GD.Print("CALCULATE SOLITARY");
        // Rules for life
        // if dead && livingNeighbors == 2: become alive
        if (!IsTileAlive(selfCoord) && (countLivingNeighbors == 2))
        {
            nextSolitaryLifeCoords.Add(selfCoord);
            GD.Print("Condition 1");
        }
        // if alive && livingNeighbors == 1 or 2: remain alive
        else if (IsTileAlive(selfCoord) && ((countLivingNeighbors == 1) || (countLivingNeighbors == 2)))
        {
            GD.Print("Condition 2");
            return;
        }
        // if alive && livingNeighbors < 1: die
        else if (IsTileAlive(selfCoord) && (countLivingNeighbors < 1))
        {
            nextSolitaryDeathCoords.Add(selfCoord);
            GD.Print("Condition 3");
        }
        // if alive && livingNeighbors > 2: die
        else if (IsTileAlive(selfCoord) && (countLivingNeighbors > 2))
        {
            nextSolitaryDeathCoords.Add(selfCoord);
            GD.Print("Condition 4");
        }
    }

    private void UpdateGeneration()
    {
        GD.Print("Next Life");
        nextGenerationLifeCoords.ForEach(item => GD.Print(item));
        foreach (Vector2I coord in nextGenerationLifeCoords)
        {
            PlaceLifeAtPosition(coord);
        }

        GD.Print("Next Death");
        nextGenerationDeathCoords.ForEach(item => GD.Print(item));
        foreach (Vector2I coord in nextGenerationDeathCoords)
        {
            RemoveLifeAtPosition(coord);
        }

        nextGenerationLifeCoords.Clear();
        nextGenerationDeathCoords.Clear();
    }

    private void UpdateSolitaryGeneration()
    {
        foreach (Vector2I coord in nextSolitaryLifeCoords)
        {
            PlaceSolitaryLifeAtPosition(coord);
        }

        foreach (Vector2I coord in nextSolitaryDeathCoords)
        {
            RemoveLifeAtPosition(coord);
        }

        nextSolitaryLifeCoords.Clear();
        nextSolitaryDeathCoords.Clear();
    }

    private void PlaceLifeAtPosition(Vector2I gridPosition)
    {
        var life = lifeScene.Instantiate<Node2D>();
        lifeManager.AddChild(life);

        life.GlobalPosition = gridPosition * 16;
        life.AddToGroup("average");
        MarkTileAsAlive(gridPosition, life);
    }

    private void PlaceSolitaryLifeAtPosition(Vector2I gridPosition)
    {
        var life = lifeSceneSolitary.Instantiate<Node2D>();
        lifeManager.AddChild(life);

        life.GlobalPosition = gridPosition * 16;
        life.AddToGroup("solitary");
        MarkTileAsAlive(gridPosition, life);
    }

    private void RemoveLifeAtPosition(Vector2I gridPosition)
    {
        var key = gridPosition;
        if (gridLife.ContainsKey(key))
        {
            Node2D life = gridLife[key];
            life.QueueFree();
            gridLife.Remove(key);
            MarkTileAsDead(key);
        }
    }
    #endregion
}
