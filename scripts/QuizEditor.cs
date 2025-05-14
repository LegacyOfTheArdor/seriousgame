using Godot;
using System;
using System.Text.Json;

public partial class QuizEditor : Control
{
	private Action returnCallback;
	private QuizData quizData;

	// Example UI nodes (adjust as needed)
	private VBoxContainer sliderTitleContainer;
	private VBoxContainer questionsContainer;
	private Button saveQuizButton;
	private Button backButton;

	public override void _Ready()
	{
		sliderTitleContainer = GetNode<VBoxContainer>("SliderTitleContainer");
		questionsContainer = GetNode<VBoxContainer>("QuestionsContainer");
		saveQuizButton = GetNode<Button>("SaveQuizButton");
		backButton = GetNode<Button>("BackButton");

		saveQuizButton.Pressed += OnSaveQuizPressed;
		backButton.Pressed += OnBackPressed;
	}

	// Loader pattern: called from QuizSelection
	public void LoadQuiz(string quizName, Action returnCb)
	{
		returnCallback = returnCb;

		if (string.IsNullOrEmpty(quizName))
		{
			// New quiz
			quizData = CreateNewQuiz();
		}
		else
		{
			// Load quiz from file
			string filePath = $"res://quizzes/{quizName}.json";
			using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			quizData = JsonSerializer.Deserialize<QuizData>(json);
		}

		ShowQuiz();
	}

	private QuizData CreateNewQuiz()
	{
		var newQuiz = new QuizData();
		newQuiz.sliderTitles = new System.Collections.Generic.List<string> { "Slider 1", "Slider 2", "Slider 3", "Slider 4" };
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
				answer.sliderValues = new System.Collections.Generic.List<float>(new float[newQuiz.sliderTitles.Count]);
				question.answers.Add(answer);
			}
			newQuiz.questions.Add(question);
		}
		return newQuiz;
	}

	private void ShowQuiz()
	{
		// Clear previous UI
		foreach (Node child in sliderTitleContainer.GetChildren())
			child.QueueFree();
		foreach (Node child in questionsContainer.GetChildren())
			child.QueueFree();

		// Show slider titles
		foreach (var title in quizData.sliderTitles)
		{
			var edit = new LineEdit { Text = title };
			sliderTitleContainer.AddChild(edit);
		}

		// Show questions and answers (simplified)
		for (int q = 0; q < quizData.questions.Count; q++)
		{
			var question = quizData.questions[q];
			var questionVBox = new VBoxContainer();

			var questionEdit = new LineEdit { Text = question.questionTitle };
			questionVBox.AddChild(questionEdit);

			for (int a = 0; a < question.answers.Count; a++)
			{
				var answer = question.answers[a];
				var answerHBox = new HBoxContainer();

				var answerEdit = new LineEdit { Text = answer.answerTitle };
				answerHBox.AddChild(answerEdit);

				// Sliders for each stat
				for (int s = 0; s < quizData.sliderTitles.Count; s++)
				{
					var slider = new HSlider
					{
						MinValue = 0,
						MaxValue = 10,
						Step = 1,
						Value = answer.sliderValues[s],
						SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
					};
					answerHBox.AddChild(new Label { Text = quizData.sliderTitles[s] });
					answerHBox.AddChild(slider);
				}

				questionVBox.AddChild(answerHBox);
			}

			questionsContainer.AddChild(questionVBox);
		}
	}

	private void OnSaveQuizPressed()
	{
		// Save logic here (you can call back to QuizSelection or handle saving here)
		// Example: SaveQuiz(quizData);
		returnCallback?.Invoke();
	}

	private void OnBackPressed()
	{
		returnCallback?.Invoke();
	}
}
