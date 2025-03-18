using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance;
	
	[Export] Player PlayerChar;
	[Export] int Level;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
