using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

public partial class QuizSelection : CanvasLayer
{
	[Export] public PackedScene QuizEditorScene;
	[Export] public PackedScene QuizTemplateScene;
	[Export] public GridContainer QuizGrid;
	[Export] public HBoxContainer PaginationContainer;
	[Export] public string QuizFolder = "res://quizzes/";

	private Node quizInstance;

	public bool IsEditMode { get; private set; } = false;

	private int columns = 10;
	private int rows = 2;
	private int quizzesPerPage => columns * rows;
	private int currentPage = 0;
	private List<string> quizFiles = new List<string>();

	public void SetEditMode(bool editMode)
	{
		IsEditMode = editMode;
		PopulateQuizList();
	}

	public override void _Ready()
	{
		// Set edit mode from your global state if needed
		// SetEditMode(GetNode<GameState>("/root/GameState").EditMode);

		string absPath = ProjectSettings.GlobalizePath(QuizFolder);
		if (!DirAccess.DirExistsAbsolute(absPath))
			DirAccess.MakeDirRecursiveAbsolute(absPath);

		QuizGrid.Columns = columns;
		LoadQuizFiles();
		PopulateQuizList();
	}

	private void LoadQuizFiles()
	{
		quizFiles.Clear();
		var dir = DirAccess.Open(QuizFolder);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (fileName.EndsWith(".json"))
					quizFiles.Add(Path.GetFileNameWithoutExtension(fileName));
				fileName = dir.GetNext();
			}
			dir.ListDirEnd();
		}
	}

	private void PopulateQuizList()
	{
		foreach (Node child in QuizGrid.GetChildren())
			child.QueueFree();
		foreach (Node child in PaginationContainer.GetChildren())
			child.QueueFree();

		if (IsEditMode)
		{
			// "+ New Quiz" button is always the first button in the grid
			Button newQuizBtn = new Button();
			newQuizBtn.Text = "+ New Quiz";
			newQuizBtn.Pressed += NewQuiz;
			QuizGrid.AddChild(newQuizBtn);

			int startIdx = currentPage * (quizzesPerPage - 1);
			int endIdx = Math.Min(startIdx + (quizzesPerPage - 1), quizFiles.Count);

			for (int i = startIdx; i < endIdx; i++)
			{
				string quizName = quizFiles[i];
				Button btn = new Button();
				btn.Text = quizName;
				btn.Pressed += () => LoadQuiz(quizName, true); // Edit mode
				QuizGrid.AddChild(btn);
			}

			int totalSlotsUsed = 1 + (endIdx - startIdx);
			for (int i = totalSlotsUsed; i < quizzesPerPage; i++)
				QuizGrid.AddChild(new Control());
		}
		else
		{
			if (quizFiles.Count == 0) return;

			int startIdx = currentPage * quizzesPerPage;
			int endIdx = Math.Min(startIdx + quizzesPerPage, quizFiles.Count);

			for (int i = startIdx; i < endIdx; i++)
			{
				string quizName = quizFiles[i];
				Button btn = new Button();
				btn.Text = quizName;
				btn.Pressed += () => LoadQuiz(quizName, false); // Play mode
				QuizGrid.AddChild(btn);
			}

			for (int i = endIdx - startIdx; i < quizzesPerPage; i++)
				QuizGrid.AddChild(new Control());
		}

		AddPaginationButtons();
	}

	private void AddPaginationButtons()
	{
		int totalPages = IsEditMode ?
			Math.Max(1, (int)Math.Ceiling(quizFiles.Count / (double)(quizzesPerPage - 1))) :
			Math.Max(1, (int)Math.Ceiling(quizFiles.Count / (double)quizzesPerPage));

		if (currentPage > 0)
		{
			Button prevBtn = new Button();
			prevBtn.Text = "< Previous";
			prevBtn.Pressed += () => { currentPage--; PopulateQuizList(); };
			PaginationContainer.AddChild(prevBtn);
		}

		if (currentPage < totalPages - 1)
		{
			Button nextBtn = new Button();
			nextBtn.Text = "Next >";
			nextBtn.Pressed += () => { currentPage++; PopulateQuizList(); };
			PaginationContainer.AddChild(nextBtn);
		}
	}

	public void NewQuiz()
	{
		var quizData = new QuizData();
		quizData.sliderTitles = new List<string> { "Slider 1", "Slider 2", "Slider 3", "Slider 4" };

		for (int q = 0; q < 4; q++)
		{
			var question = new Question();
			question.questionTitle = $"Question {q + 1}";
			for (int a = 0; a < 4; a++)
			{
				var answer = new Answer();
				answer.answerTitle = $"Answer {a + 1}";
				answer.description = "";
				answer.upside = "";
				answer.downside = "";
				answer.sliderValues = new List<float>(new float[quizData.sliderTitles.Count]);
				question.answers.Add(answer);
			}
			quizData.questions.Add(question);
		}
		ShowQuizEditor(quizData);
	}

	public void LoadQuiz(string quizName, bool editMode)
	{
		string filePath = $"{QuizFolder}{quizName}.json";
		using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
		string json = file.GetAsText();
		var quizData = JsonSerializer.Deserialize<QuizData>(json);

		if (editMode)
			ShowQuizEditor(quizData);
		else
			ShowQuizTemplate(quizData);
	}

	private void ShowQuizEditor(QuizData quizData)
	{
		if (quizInstance != null && quizInstance.IsInsideTree())
			quizInstance.QueueFree();

		quizInstance = QuizEditorScene.Instantiate();
		AddChild(quizInstance);

		if (quizInstance is QuizEditor quizEditor)
			quizEditor.SetQuizData(quizData, SaveQuiz, ReturnToList);
	}

	private void ShowQuizTemplate(QuizData quizData)
	{
		if (quizInstance != null && quizInstance.IsInsideTree())
			quizInstance.QueueFree();

		quizInstance = QuizTemplateScene.Instantiate();
		AddChild(quizInstance);

		if (quizInstance is QuizTemplate quizTemplate)
			quizTemplate.SetQuizData(quizData, ReturnToList);
	}

	public void SaveQuiz(string title, QuizData data)
	{
		string safeTitle = string.IsNullOrEmpty(title) ? "Untitled_Quiz" : title;
		string filePath = $"{QuizFolder}{safeTitle}.json";
		string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

		using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Write);
		file.StoreString(json);

		GD.Print($"Quiz saved to {filePath}");
		LoadQuizFiles();
		ReturnToList();
	}

	public void ReturnToList()
	{
		if (quizInstance != null && quizInstance.IsInsideTree())
			quizInstance.QueueFree();
		PopulateQuizList();
	}
}
