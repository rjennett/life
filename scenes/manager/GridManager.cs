using Godot;
using System;
using System.Collections.Generic;

public partial class GridManager : Node2D
{
    // Hashset to track living cells
    // One for each type of life?
    private HashSet<Vector2I> occupiedCells = new();

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
        var customData = baseTerrainTileMapLayer.GetCellTileData(tilePosition);
        if (customData == null) return false;

        return !occupiedCells.Contains(tilePosition);
    }

    // Mark tile as alive by adding to HashSet occupiedTiles
    public void MarkTileAsAlive(Vector2I tilePosition)
    {
        occupiedCells.Add(tilePosition);
    }
}
