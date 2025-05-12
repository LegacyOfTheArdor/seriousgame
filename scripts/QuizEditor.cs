using Godot;

public partial class QuizEditor : Control
{
	private QuizResource currentQuiz;
	


	public override void _Ready()
	{
		
		// Initialize other UI elements
	}

	public void LoadQuiz(string quizName)
	{
		string path = $"user://quizzes/{quizName}.tres";
		if (ResourceLoader.Exists(path))
		{
			currentQuiz = ResourceLoader.Load<QuizResource>(path);
			UpdateUI();
		}
	}
	}
