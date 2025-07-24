using Godot;
using System;

public partial class LifeManager : Node2D
{
    private RichTextLabel generationDisplay;

    public override void _Ready()
    {
        base._Ready();
        generationDisplay = GetNode<RichTextLabel>("UserInterface/GenerationDisplay");
    }

    
}
