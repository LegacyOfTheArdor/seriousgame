using Godot;

public partial class MainMenu : CanvasLayer
{
	private Panel panel;
	[Export] public Button PlayButton;
	[Export] public Button EditButton;
	[Export] public Button ExitButton;

	public override void _Ready()
	{
		panel = GetNode<Panel>("outerborder/Panel");
		PlayButton = GetNode<Button>("outerborder/Panel/VBoxContainer/PlayButton");
		EditButton = GetNode<Button>("outerborder/Panel/VBoxContainer/EditButton");
		ExitButton = GetNode<Button>("outerborder/Panel/VBoxContainer/ExitButton");

		PlayButton.Pressed += OnPlayButtonPressed;
		EditButton.Pressed += OnEditButtonPressed;
		ExitButton.Pressed += OnExitButtonPressed;

		// Optionally, set the Panel to expand and fill the entire viewport
		panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
	}

	private void OnPlayButtonPressed()
	{
		// Set edit mode to false in the singleton before switching scenes
		GetNode<GameState>("/root/GameState").EditMode = false;
		GetTree().ChangeSceneToFile("res://scenes/QuizSelection.tscn");
	}

	private void OnEditButtonPressed()
	{
		// Set edit mode to true in the singleton before switching scenes
		GetNode<GameState>("/root/GameState").EditMode = true;
		GetTree().ChangeSceneToFile("res://scenes/QuizSelection.tscn");
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}
