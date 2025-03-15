using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("ButtonContainer/Quit").Connect("pressed", new Callable(this, nameof(QuitPressed)));
		GetNode<Button>("ButtonContainer/Start").Connect("pressed", new Callable(this, nameof(StartPressed)));
	}

	private void QuitPressed()
	{
		GetTree().Quit();
	}
	private void StartPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
