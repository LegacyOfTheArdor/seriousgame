using Godot;

public partial class QuizEditor : Control
{
	private QuizResource currentQuiz;
	private Button deleteQuizButton;

	public override void _Ready()
	{
		deleteQuizButton = GetNode<Button>("DeleteQuizButton");
		deleteQuizButton.Pressed += OnDeleteQuizPressed;
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
		e
