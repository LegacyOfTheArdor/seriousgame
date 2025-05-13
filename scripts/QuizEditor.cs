using Godot;

public partial class QuizTemplate : Control
{
 	private PackedScene quizTemplate;
	private Node quizInstance;
	private QuizData currentQuizData;

	public override void _Ready()
	{
		quizTemplate = GD.Load<PackedScene>("res://QuizTemplate.tscn");
		// Optionally, load an existing quiz or create a new one
		if (FileAccess.FileExists("user://my_quiz.json"))
			LoadQuiz("my_quiz");
		else
			CreateNewQuiz();
	}

	public void CreateNewQuiz()
	{
		currentQuizData = new QuizData();
		// Add a sample question for demonstration
		currentQuizData.questions.Add(new Question
		{
			questionText = "What is 2 + 2?",
			answers = new List<string> { "3", "4", "5" },
			correctIndex = 1
		});
		ShowQuiz();
	}

	public void ShowQuiz()
	{
		// Remove previous instance if it exists
		if (quizInstance != null && quizInstance.IsInsideTree())
			quizInstance.QueueFree();

		quizInstance = quizTemplate.Instantiate();
		AddChild(quizInstance);

		// Pass quiz data to the quiz scene (see next script)
		if (quizInstance is QuizTemplate quizScript)
			quizScript.SetQuizData(currentQuizData);
	}

	public void SaveQuiz(string filename)
	{
		string json = JsonSerializer.Serialize(currentQuizData);
		using var file = FileAccess.Open($"user://{filename}.json", FileAccess.ModeFlags.Write);
		file.StoreString(json);
	}

	public void LoadQuiz(string filename)
	{
		using var file = FileAccess.Open($"user://{filename}.json", FileAccess.ModeFlags.Read);
		string json = file.GetAsText();
		currentQuizData = JsonSerializer.Deserialize<QuizData>(json);
		ShowQuiz();
	}
	}
