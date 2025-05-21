using Godot;

public partial class MainMenu : CanvasLayer
{
	private Panel panel;
	private Button PlayButton;
	private Button EditButton;
	private Button ExitButton;

	public override void _Ready()
	{
		panel = GetNode<Panel>("PanelContainer/outerborder/Panel");
		PlayButton = GetNode<Button>("PanelContainer/outerborder/Panel/VBoxContainer/PlayButton");
		EditButton = GetNode<Button>("PanelContainer/outerborder/Panel/VBoxContainer/EditButton");
		ExitButton = GetNode<Button>("PanelContainer/outerborder/Panel/VBoxContainer/ExitButton");

		PlayButton.Pressed += OnPlayButtonPressed;
		EditButton.Pressed += OnEditButtonPressed;
		ExitButton.Pressed += OnExitButtonPressed;

		panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
	}

	private void OnPlayButtonPressed()
	{
		((GameState)GetNode("/root/GameState")).EditMode = false;
		GetTree().ChangeSceneToFile("res://scenes/QuizSelection.tscn");
	}

	private void OnEditButtonPressed()
	{
		((GameState)GetNode("/root/GameState")).EditMode = true;
		GetTree().ChangeSceneToFile("res://scenes/QuizSelection.tscn");
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}
