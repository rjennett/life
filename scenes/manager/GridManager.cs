using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
        
        GD.Print(String.Join(",", occupiedCells));
        foreach (KeyValuePair<Vector2I, Node2D> kvp in gridLife)
        {
            GD.Print(kvp.Key, kvp.Value);
        }
        GD.Print(occupiedCells.Contains(tilePosition));
        GD.Print(tilePosition);

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
