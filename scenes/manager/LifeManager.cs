using Godot;
using System;

public partial class LifeManager : Node2D
{
    private RichTextLabel generationDisplay;
    private PackedScene lifeScene;
    private GridManager gridManager;


    public override void _Ready()
    {
        base._Ready();
        lifeScene = GD.Load<PackedScene>("res://scenes/life/Life.tscn");
        generationDisplay = GetNode<RichTextLabel>("UserInterface/GenerationDisplay");
        gridManager = GetNode<GridManager>("../");

    }
}
