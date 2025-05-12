using Godot;

public partial class MainMenu : CanvasLayer
{
	private Panel panel;
	private Button playButton;
	private Button editButton;
	private Button exitButton;

	public override void _Ready()
	{
		panel = GetNode<Panel>("Panel");
		playButton = GetNode<Button>("Panel/VBoxContainer/PlayButton");
		editButton = GetNode<Button>("Panel/VBoxContainer/EditButton");
		exitButton = GetNode<Button>("Panel/VBoxContainer/ExitButton");

		playButton.Pressed += OnPlayButtonPressed;
		editButton.Pressed += OnEditButtonPressed;
		exitButton.Pressed += OnExitButtonPressed;

		// Optionally, set the Panel to expand and fill the entire viewport
		panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
	}

	private void OnPlayButtonPressed()
	{
		
	  GetTree().ChangeSceneToFile("res://QuizSelection.tscn");
	}

	private void OnEditButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://QuizSelection.tscn");
		// Pass a parameter to indicate edit mode
		GetNode<QuizSelection>("/root/QuizSelection").SetEditMode(true);
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}
