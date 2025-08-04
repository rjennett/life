using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

public partial class LifeManager : Node2D
{
    private RichTextLabel generationDisplay;
    private PackedScene lifeScene;
    private PackedScene lifeSceneSolitary;
    private PackedScene lifeSceneSocial;
    private GridManager gridManager;
    private List<List<Vector2I>> listOfNeighborhoods = new();


    public override void _Ready()
    {
        base._Ready();
        lifeScene = GD.Load<PackedScene>("res://scenes/life/LifeAverage.tscn");
        lifeSceneSolitary = GD.Load<PackedScene>("res://scenes/life/LifeSolitary.tscn");
        lifeSceneSocial = GD.Load<PackedScene>("res://scenes/life/LifeSocial.tscn");
        // generationDisplay = GetNode<RichTextLabel>("UserInterface/GenerationDisplay");
        gridManager = GetNode<GridManager>("../");
    }

    #region Management

    // Iterate children on timer.timeout
    public void IterateLifeNodes()
    {
        foreach (Node2D child in GetChildren())
        {
            Vector2I childPosition = (Vector2I)child.Position;
            
            // Store the group a node belongs to. These nodes will only ever belong to one group, hence [0]
            var nodeLifeType = child.GetGroups()[0];
            GD.Print(nodeLifeType);
            
            // Get the neighborhood of the life node
            var neighborhoodInfo = AssessNeighbors(childPosition, gridManager.baseTerrainTileMapLayer);

            // Adhere to rules for average life
            if (nodeLifeType == "average")
            {
                // Determine the next generation for the life node (will it die)
                if (neighborhoodInfo.countLivingNeighbors < 2)
                {
                    // This node will die next generation during update

                    GD.Print("This node will die by UNDER: ", childPosition);
                    gridManager.nextGenerationDeathCoords.Add(childPosition);
                }
                else if (neighborhoodInfo.countLivingNeighbors > 3)
                {
                    // This node will die next generation during update

                    GD.Print("This node will die by OVER: ", childPosition);
                    gridManager.nextGenerationDeathCoords.Add(childPosition);
                }
                else
                {
                    GD.Print("This node will not die: ", childPosition);
                }
            }
            else if (nodeLifeType == "solitary")
            {
                if (neighborhoodInfo.countLivingNeighbors < 1)
                {
                    gridManager.nextGenerationDeathCoords.Add(childPosition);
                }
                else if (neighborhoodInfo.countLivingNeighbors > 2)
                {
                    gridManager.nextGenerationDeathCoords.Add(childPosition);
                }
            }
            else if (nodeLifeType == "social")
            {
                if (neighborhoodInfo.countLivingNeighbors < 3)
                {
                    gridManager.nextGenerationDeathCoords.Add(childPosition);
                }
                else if (neighborhoodInfo.countLivingNeighbors > 4)
                {
                    gridManager.nextGenerationDeathCoords.Add(childPosition);
                }
            }

            // Add current neighborhood to list of neighborhoods
            listOfNeighborhoods.Add(neighborhoodInfo.neighborhood);
        }
        // foreach (var list in listOfNeighborhoods)
        // {
        //     foreach (var item in list)
        //     {
        //         GD.Print(item);
        //     }
        // }
        foreach (var item in gridManager.nextGenerationDeathCoords)
        {
            GD.Print("next gen death: ", item);
        }
        GD.Print("- - - - - - - - - ");


        // Compare neighborhoods from all children to locate new life
        // Needs to contain information about which type of life the neighborhood came from
        // Start by placing average life every time, update later 
        var uniquePositions = listOfNeighborhoods.SelectMany(l => l)
            .GroupBy(item => item)
            .Select(group => new { Item = group.Key, Count = group.Count() });

        foreach (var item in uniquePositions)
        {
            // GD.Print($"Item: {item.Item}, Count: {item.Count}");
            if (item.Count == 3)
            {
                gridManager.nextGenerationLifeCoords.Add(item.Item);
            }
        }

        // Store type of life at each position from current generation

        // Calculate the next generation

        // Update the generation
        UpdateGeneration();

        // Clear the list of neighborhoods to reuse
        listOfNeighborhoods.Clear();
    }

    #endregion

    #region Workers

    // Get neighborhood of node position and find most common neighbor type
    private string calculateMostCommonNeighbor(Vector2I currentGridPosition, TileMapLayer baseTerrainTileMapLayer)
    {
        // Initialize new list for neighbor positions
        List<Vector2I> neighborhood = new();

        // Initialize group string to return
        string mostCommonGroup;

        // Counts for each group type
        int countAverage = 0;
        int countSolitary = 0;
        int countSocial = 0;

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
            // Divide node position by 16 to get correct coordintes for TileMapLayer neighbors
            Vector2I neighborPosition = baseTerrainTileMapLayer.GetNeighborCell(currentGridPosition, neighbor);

            // Add the neighbor position to the neighborhood list
            neighborhood.Add(neighborPosition);
        }

        foreach (Vector2I position in neighborhood)
        {
            GD.Print("+ + + + + + + + + +", position);
        }

        foreach (Node2D child in GetChildren())
        {
            foreach (Vector2I position in neighborhood)
            {
                Vector2I childPosition = (Vector2I)child.Position;

                // Store the group a node belongs to. These nodes will only ever belong to one group, hence [0]
                var nodeLifeType = child.GetGroups()[0];
                GD.Print(nodeLifeType);

                GD.Print("CHILD POSITION: ", childPosition / 16);
                GD.Print("NEIGHBOR POSITION: ", position);

                // If true, the node is at the position of the neighbor
                if ((childPosition / 16) == position)
                {
                    if (child.IsInGroup("average"))
                    {
                        countAverage++;
                    }
                    else if (child.IsInGroup("solitary"))
                    {
                        countSolitary++;
                    }
                    else if (child.IsInGroup("social"))
                    {
                        countSocial++;
                    }
                    // If there is no node at the neighbor position
                    else
                    {
                        continue;
                    }
                }
            }
        }

        GD.Print("Average", countAverage);
        GD.Print("Solitary", countSolitary);
        GD.Print("Social", countSocial);

        List<int> counts = new List<int> { countAverage, countSolitary, countSocial };
        int maxGroup = counts.Max();

        // Convert max count to string groups
        if (maxGroup == countAverage)
        {
            mostCommonGroup = "average";
        }
        else if (maxGroup == countSolitary)
        {
            mostCommonGroup = "solitary";
        }
        else
        {
            mostCommonGroup = "social";
        }

        return mostCommonGroup;
    }

    // Check neighbors for life
    public (int countLivingNeighbors, List<Vector2I> neighborhood) AssessNeighbors(Vector2I currentGridPosition, TileMapLayer baseTerrainTileMapLayer)
    {
        // Neighbor cell positions
        GD.Print("Assessing neighbors for ", currentGridPosition);

        // Initialize new list for neighbor positions
        List<Vector2I> neighborhood = new();

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
            // Divide node position by 16 to get correct coordintes for TileMapLayer neighbors
            Vector2I neighborPosition = baseTerrainTileMapLayer.GetNeighborCell(currentGridPosition / 16, neighbor);

            // Is the neighbor alive?
            if (gridManager.IsTileAlive(neighborPosition))
            {
                GD.Print($"Neighbor {neighbor} Lives!");
                // Add to list of living neighbor locations/coords
                // Increment count of living neighbors
                livingNeighbors++;
            }

            // Add the neighbor position to the neighborhood list
            neighborhood.Add(neighborPosition);
        }

        GD.Print("Living neighbors: ", livingNeighbors);

        return (livingNeighbors, neighborhood);

    }

    private void UpdateGeneration()
    {
        GD.Print("Next Life");
        gridManager.nextGenerationLifeCoords.ForEach(item => GD.Print(item));

        foreach (Vector2I coord in gridManager.nextGenerationLifeCoords)
        {
            if (!gridManager.IsTileAlive(coord))
            {
                PlaceLifeAtPosition(coord);
            }
            else
            {
                GD.Print("Tile was already living!");
            }
        }

        GD.Print("Next Death");
        gridManager.nextGenerationDeathCoords.ForEach(item => GD.Print(item));

        foreach (Vector2I coord in gridManager.nextGenerationDeathCoords)
        {
            RemoveLifeAtPosition(coord);
        }

        gridManager.nextGenerationLifeCoords.Clear();
        gridManager.nextGenerationDeathCoords.Clear();
    }

    private void PlaceLifeAtPosition(Vector2I gridPosition)
    {
        string mostCommonNeighborGroup;
        // Get the neighborhood of the life node
        mostCommonNeighborGroup = calculateMostCommonNeighbor(gridPosition, gridManager.baseTerrainTileMapLayer);
        
        // Randomly select the new life type
        List<int> types = new List<int> { 0, 1, 2 };
        Random random = new();
        int randomType = random.Next(0, types.Count);

        Node2D life;
        string group;

        // Initialize the packed scene to use as the Node2D for new life
        switch (mostCommonNeighborGroup)
        {
            case "average":
                life = lifeScene.Instantiate<Node2D>();
                group = "average";
                break;
            case "social":
                life = lifeSceneSocial.Instantiate<Node2D>();
                group = "social";
                break;
            case "solitary":
                life = lifeSceneSolitary.Instantiate<Node2D>();
                group = "solitary";
                break;
            default:
                life = lifeScene.Instantiate<Node2D>();
                group = "average";
                break;
        }

        AddChild(life);

        // Multiply by 16 to match global position
        life.GlobalPosition = gridPosition * 16;
        life.AddToGroup(group);
        gridManager.MarkTileAsAlive(gridPosition, life);
    }

    private void RemoveLifeAtPosition(Vector2I gridPosition)
    {
        // Divide by 16 to match grid coordinates
        var key = gridPosition / 16;
        if (gridManager.gridLife.ContainsKey(key))
        {
            Node2D life = gridManager.gridLife[key];
            life.QueueFree();
            gridManager.gridLife.Remove(key);
            gridManager.MarkTileAsDead(key);
        }
    }

    #endregion

}
