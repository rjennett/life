using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manager.GridManager;

public partial class GridManager : Node2D
{
    // Hashset to track living cells
    // One for each type of life?
    private HashSet<Vector2I> occupiedCells = new();

    // Store each life in a dictionary with its coords for easy access
    public Dictionary<Vector2I, Node2D> gridLife = new();

    [Export]
    private TileMapLayer baseTerrainTileMapLayer;

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
                AssessNeighbors(position);
                // IterateGeneration();
            }

        }
    }

    // Check neighbors for life
    public void AssessNeighbors(Vector2I currentGridPosition)
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

        foreach (TileSet.CellNeighbor neighbor in neighbors)
        {
            var cell = baseTerrainTileMapLayer.GetNeighborCell(currentGridPosition, neighbor);

            // Is the neighbor alive?
            if (IsTileAlive(cell))
            {
                GD.Print($"Neighbor {neighbor} Lives!");
                // Add to list of living neighbor locations/coords
                // Increment count of living neighbors
            }
        }


    }

    // Change state
    public void CalculateNextGeneration()
    {

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
    #endregion
}
