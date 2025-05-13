using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

public partial class QuizEditor : Node
{
	[Export] public PackedScene QuizTemplateScene;
	[Export] public VBoxContainer QuizListContainer;
	[Export] public string QuizFolder = "res://quizzes/";

	private Node quizInstance;
	private QuizData currentQuizData;
	private string currentQuizFile;

	public override void _Ready()
	{
		string absPath = ProjectSettings.GlobalizePath(QuizFolder);
		if (!DirAccess.DirExistsAbsolute(absPath))
			DirAccess.MakeDirRecursiveAbsolute(absPath);

		PopulateQuizList();
	}

	private void PopulateQuizList()
	{
		for (int i = QuizListContainer.GetChildCount() - 1; i >= 0; i--)
			QuizListContainer.GetChild(i).QueueFree();

		var dir = DirAccess.Open(QuizFolder);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (fileName.EndsWith(".json"))
				{
					Button btn = new Button();
					btn.Text = Path.GetFileNameWithoutExtension(fileName);
					btn.Pressed += () => LoadQuiz(btn.Text);
					QuizListContainer.AddChild(btn);
				}
				fileName = dir.GetNext();
			}
			dir.ListDirEnd();
		}

		Button newQuizBtn = new Button();
		newQuizBtn.Text = "+ New Quiz";
		newQuizBtn.Pressed += NewQuiz;
		QuizListContainer.AddChild(newQuizBtn);
	}

	public void NewQuiz()
	{
		currentQuizData = new QuizData();
		currentQuizData.sliderTitles = new List<string> { "Strength", "Speed", "Intelligence", "Luck" };

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
				answer.sliderValues = new List<float> { 0, 0, 0, 0 };
				question.answers.Add(answer);
			}
			currentQuizData.questions.Add(question);
		}
		currentQuizFile = null;
		ShowQuizEditor();
	}

	public void LoadQuiz(string quizName)
	{
		string filePath = $"{QuizFolder}{quizName}.json";
		using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
		string json = file.GetAsText();
		currentQuizData = JsonSerializer.Deserialize<QuizData>(json);
		currentQuizFile = filePath;
		ShowQuizEditor();
	}

	private void ShowQuizEditor()
	{
		if (quizInstance != null && quizInstance.IsInsideTree())
			quizInstance.QueueFree();

		quizInstance = QuizTemplateScene.Instantiate();
		AddChild(quizInstance);

		// Always edit mode in the editor; pass required callbacks
		if (quizInstance is QuizTemplate quizScript)
			quizScript.SetQuizData(currentQuizData, SaveQuiz, ReturnToList);
	}

	public void SaveQuiz(string title, QuizData data)
	{
		string safeTitle = string.IsNullOrEmpty(title) ? "Untitled_Quiz" : title;
		string filePath = $"{QuizFolder}{safeTitle}.json";
		string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

		using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Write);
		file.StoreString(json);

		currentQuizFile = filePath;
		currentQuizData = data;

		GD.Print($"Quiz saved to {filePath}");

		ReturnToList();
	}

	public void ReturnToList()
	{
		if (quizInstance != null && quizInstance.IsInsideTree())
			quizInstance.QueueFree();
		PopulateQuizList();
	}
}
