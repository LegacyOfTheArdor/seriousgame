using Godot;
using System;
using System.Collections.Generic;

public partial class QuizSelection : CanvasLayer
{
	private GridContainer QuizGrid;
	private Button NewQuizButton;
	private Button NextPageButton;
	private Button PrevPageButton;

	private List<string> quizFiles = new List<string>();
	private int page = 0;
	private int quizzesPerPage = 20; // 10 columns × 2 rows

	// Path to your custom quiz button scene (design this in Godot and set the path)
	private string quizButtonScenePath = "res://scenes/SelectQuizButton.tscn";

	public override void _Ready()
	{
		NewQuizButton = GetNode<Button>("PanelContainer/outerborder/Panel/MenuGrid/NewQuizButton");
		NextPageButton = GetNode<Button>("PanelContainer/outerborder/Panel/MenuGrid/NextPageButton");
		PrevPageButton = GetNode<Button>("PanelContainer/outerborder/Panel/MenuGrid/PrevPageButton");
		QuizGrid = GetNode<GridContainer>("PanelContainer/outerborder/Panel/QuizGrid");

		NewQuizButton.Pressed += OnNewQuizPressed;
		NextPageButton.Pressed += OnNextPagePressed;
		PrevPageButton.Pressed += OnPrevPagePressed;

		// Set grid columns and rows
		QuizGrid.Columns = 10;
		QuizGrid.SizeFlagsVertical = Control.SizeFlags.ExpandFill;


		LoadQuizFiles();
		UpdateUI();
	}

	private void LoadQuizFiles()
	{
		quizFiles.Clear();
		string dirPath = "user://seriousgame/quizzes/";
		var dir = DirAccess.Open(dirPath);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (!dir.CurrentIsDir() && fileName.EndsWith(".json"))
					quizFiles.Add(System.IO.Path.GetFileNameWithoutExtension(fileName));
				fileName = dir.GetNext();
			}
			dir.ListDirEnd();
		}
	}

	private void UpdateUI()
	{
		// Show/hide NewQuizButton based on edit mode
		NewQuizButton.Visible = ((GameState)GetNode("/root/GameState")).EditMode;

		// Pagination logic
		int totalPages = (int)Math.Ceiling(quizFiles.Count / (float)quizzesPerPage);
		NextPageButton.Visible = (page < totalPages - 1);
		PrevPageButton.Visible = (page > 0);

		foreach (Node child in QuizGrid.GetChildren())
			child.QueueFree();

		int startIdx = page * quizzesPerPage;
		int endIdx = Math.Min(startIdx + quizzesPerPage, quizFiles.Count);

		for (int i = startIdx; i < endIdx; i++)
		{
			string quizName = quizFiles[i];

			// Load your custom button scene
			var buttonScene = GD.Load<PackedScene>(quizButtonScenePath);
			if (buttonScene == null)
			{
				GD.PrintErr("QuizButton scene not found at: " + quizButtonScenePath);
				continue;
			}

			var btnNode = buttonScene.Instantiate();
			Button btn = btnNode as Button;
			if (btn == null)
			{
				GD.PrintErr("QuizButton scene does not inherit Button!");
				continue;
			}

			btn.Text = quizName;
			btn.Pressed += () => OnQuizButtonPressed(quizName);
			QuizGrid.AddChild(btn);
		}
	}

	private void OnNewQuizPressed()
	{
		((GameState)GetNode("/root/GameState")).SelectedQuizFilePath = null;
		GetTree().ChangeSceneToFile("res://scenes/QuizTemplate.tscn");
	}

	private void OnQuizButtonPressed(string quizName)
	{
		((GameState)GetNode("/root/GameState")).SelectedQuizFilePath = $"user://seriousgame/quizzes/" +quizName +".json";
		((GameState)GetNode("/root/GameState")).QuizName = quizName;
		GD.Print(((GameState)GetNode("/root/GameState")).SelectedQuizFilePath);
		GetTree().ChangeSceneToFile("res://scenes/QuizTemplate.tscn");
	}

	private void OnNextPagePressed()
	{
		page++;
		UpdateUI();
	}

	private void OnPrevPagePressed()
	{
		page--;
		UpdateUI();
	}
	

}
