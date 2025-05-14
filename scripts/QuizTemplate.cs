using Godot;
using System.Text.Json;

public partial class QuizTemplate : Control
{
	private QuizData quizData;

	public override void _Ready()
	{
		string quizName = ((GameState)GetNode("/root/GameState")).SelectedQuizFile;
		if (string.IsNullOrEmpty(quizName))
		{
			ShowBlankTemplate();
		}
		else
		{
			string filePath = $"res://quizzes/{quizName}.json";
			using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			quizData = JsonSerializer.Deserialize<QuizData>(json);
			ShowQuiz();
		}
	}

	private void ShowBlankTemplate()
	{
		// Show blank or "Create new quiz" UI
	}

	private void ShowQuiz()
	{
		// Show loaded quiz
	}
}
