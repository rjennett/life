using Godot;
using System;

public partial class LifeManager : Node2D
{
    private RichTextLabel generationDisplay;

    public override void _Ready()
    {
        base._Ready();
        generationDisplay = GetNode<RichTextLabel>("GenerationDisplay");
    }

    public void IterateLifeGrid(TileMapLayer lifeGrid)
    {
        // Iterate grid
        for (int i = 0; i < lifeGrid.GetUsedRect().Size.X; i++)
        {
            for (int j = 0; j < lifeGrid.GetUsedRect().Size.Y; j++)
            {

            }

        }
    }

    // Check neighbors for life
    public void AssessNeighbors()
    {

    }

    // Change state
    public void CalculateNextGeneration()
    {

    }

    // Iterate generation
    public void IterateGeneration()
    {
        bool progress = true;
        int generation = 0;
        while (progress)
        {
            // TODO: Delay speed of iteration based on UI slider
            
            generation++;
            generationDisplay.Text = generation.ToString();
        }
    }
}
