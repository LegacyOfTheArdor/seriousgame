using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

public partial class QuizSelection : Node
{
	[Export] public PackedScene QuizTemplateScene;      // Set in Inspector: Your quiz editor template scene
	[Export] public VBoxContainer QuizListContainer;    // Set in Inspector: VBoxContainer for quiz list
	[Export] public string QuizFolder = "user://quizzes/"; // Where quizzes are stored

	private Node quizInstance;
	private QuizData currentQuizData;
	private string currentQuizFile;

	public override void _Ready()
	{
		// Ensure quiz folder exists
		if (!DirAccess.DirExistsAbsolute(ProjectSettings.GlobalizePath(QuizFolder)))
			DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath(QuizFolder));

		PopulateQuizList();
	}

	// List all quizzes as buttons, plus a "New Quiz" button
	private void PopulateQuizList()
	{
		QuizListContainer.QueueFreeChildren();

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

		// Add "New Quiz" button
		Button newQuizBtn = new Button();
		newQuizBtn.Text = "+ New Quiz";
		newQuizBtn.Pressed += NewQuiz;
		QuizListContainer.AddChild(newQuizBtn);
	}

	// Create a new quiz with 4 questions, each with 4 answers and 4 sliders
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

	// Load a quiz from file and open it in the editor
	public void LoadQuiz(string quizName)
	{
		string filePath =
