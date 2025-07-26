using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public partial class GridManager : Node2D
{
    // Hashset to track living cells
    // One for each type of life?
    private HashSet<Vector2I> occupiedCells = new();

    // Store each life in a dictionary with its coords for easy access
    public Dictionary<Vector2I, Node2D> gridLife = new();

    private PackedScene lifeScene;
    private Node2D mainNode;

    [Export]
    private TileMapLayer baseTerrainTileMapLayer;

    public override void _Ready()
    {
        base._Ready();
        lifeScene = GD.Load<PackedScene>("res://scenes/life/Life.tscn");
        mainNode = GetNode<Node2D>("/root/Main");
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
                CalculateNextGeneration(livingNeighbors, position);
            }

        }
    }

    // Check neighbors for life
    public int AssessNeighbors(Vector2I currentGridPosition)
    {
        // Neighbor cell positions
        GD.Print("Judging neighbors...");

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
            var cell = baseTerrainTileMapLayer.GetNeighborCell(currentGridPosition, neighbor);

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
    public void CalculateNextGeneration(int countLivingNeighbors, Vector2I selfCoord)
    {
        GD.Print("selfCoord: ", selfCoord);
        GD.Print("Living neighbors: ", countLivingNeighbors);
        GD.Print("Tile is alive?: ", IsTileAlive(selfCoord));
        // Rules for life
        // if dead && livingNeighbors == 3: become alive
        if (!IsTileAlive(selfCoord) && (countLivingNeighbors == 3))
        // if (!IsTileAlive(selfCoord))
        {
            PlaceLifeAtPosition(selfCoord);
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
            RemoveLifeAtPosition(selfCoord);
            GD.Print("Condition 3");
        }
        // if alive && livingNeighbors > 3: die
        else if (IsTileAlive(selfCoord) && (countLivingNeighbors > 3))
        {
            RemoveLifeAtPosition(selfCoord);
            GD.Print("Condition 4");
        }

    }

    // TODO: likely remove; this function performed by timer
    public void IterateGeneration()
    {
        bool progress = true;
        int generation = 0;
        while (progress)
        {
            // TODO: Delay speed of iteration based on UI slider

            generation++;
            // generationDisplay.Text = generation.ToString();
        }
    }

    public void PlaceLifeAtPosition(Vector2I gridPosition)
    {
        var life = lifeScene.Instantiate<Node2D>();
        mainNode.AddChild(life);

        life.GlobalPosition = gridPosition * 16;
        MarkTileAsAlive(gridPosition, life);
    }

    public void RemoveLifeAtPosition(Vector2I gridPosition)
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
