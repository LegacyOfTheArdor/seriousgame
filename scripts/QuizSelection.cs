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
	private int quizzesPerPage = 10; // Adjust to your grid

	public override void _Ready()
	{
		NewQuizButton = GetNode<Button>("outerborder/Panel/MenuGrid/NewQuizButton");
		NextPageButton = GetNode<Button>("outerborder/Panel/MenuGrid/NextPageButton");
		PrevPageButton = GetNode<Button>("outerborder/Panel/MenuGrid/PrevPageButton");
		QuizGrid = GetNode<GridContainer>("outerborder/Panel/QuizGrid");

		NewQuizButton.Pressed += OnNewQuizPressed;
		NextPageButton.Pressed += OnNextPagePressed;
		PrevPageButton.Pressed += OnPrevPagePressed;

		LoadQuizFiles();
		UpdateUI();
	}

	private void LoadQuizFiles()
	{
		quizFiles.Clear();
		var dir = DirAccess.Open("res://quizzes/");
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (fileName.EndsWith(".json"))
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
			Button btn = new Button();
			btn.Text = quizName;
			btn.Pressed += () => OnQuizButtonPressed(quizName);
			QuizGrid.AddChild(btn);
		}
	}

	private void OnNewQuizPressed()
	{
		((GameState)GetNode("/root/GameState")).SelectedQuizFile = null;
		GetTree().ChangeSceneToFile("res://scenes/QuizTemplate.tscn");
	}

	private void OnQuizButtonPressed(string quizName)
	{
		((GameState)GetNode("/root/GameState")).SelectedQuizFile = quizName;
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
