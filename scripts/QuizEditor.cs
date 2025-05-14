using Godot;
using System;
using System.Collections.Generic;

public partial class QuizEditor : Control
{
	private QuizData quizData;
	private Action<string, QuizData> saveCallback;
	private Action returnCallback;

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

	public void SetQuizData(QuizData data, Action<string, QuizData> saveCb, Action returnCb)
	{
		quizData = data;
		saveCallback = saveCb;
		returnCallback = returnCb;

		ShowSliderTitles();
		ShowQuestions();
	}

	private void ShowSliderTitles()
	{
		// Remove all children
		foreach (Node child in sliderTitleContainer.GetChildren())
			child.QueueFree();

		List<LineEdit> edits = new List<LineEdit>();
		for (int i = 0; i < quizData.sliderTitles.Count; i++)
		{
			var edit = new LineEdit { Text = quizData.sliderTitles[i] };
			int idx = i;
			edit.TextChanged += (newText) => quizData.sliderTitles[idx] = newText;
			sliderTitleContainer.AddChild(edit);
			edits.Add(edit);
		}

		var addSliderBtn = new Button { Text = "Add Slider Title" };
		addSliderBtn.Pressed += () =>
		{
			quizData.sliderTitles.Add($"Slider {quizData.sliderTitles.Count + 1}");
			// Add a zero value for each answer's sliderValues
			foreach (var question in quizData.questions)
				foreach (var answer in question.answers)
					answer.sliderValues.Add(0f);
			ShowSliderTitles();
			ShowQuestions();
		};
		sliderTitleContainer.AddChild(addSliderBtn);
	}

	private void ShowQuestions()
	{
		foreach (Node child in questionsContainer.GetChildren())
			child.QueueFree();

		for (int q = 0; q < quizData.questions.Count; q++)
		{
			var question = quizData.questions[q];
			var questionVBox = new VBoxContainer();

			var questionEdit = new LineEdit { Text = question.questionTitle };
			int questionIdx = q;
			questionEdit.TextChanged += (newText) => quizData.questions[questionIdx].questionTitle = newText;
			questionVBox.AddChild(questionEdit);

			for (int a = 0; a < question.answers.Count; a++)
			{
				var answer = question.answers[a];
				var answerHBox = new HBoxContainer();

				var answerEdit = new LineEdit { Text = answer.answerTitle };
				int answerIdx = a;
				answerEdit.TextChanged += (newText) => quizData.questions[questionIdx].answers[answerIdx].answerTitle = newText;
				answerHBox.AddChild(answerEdit);

				// Sliders for each stat
				for (int s = 0; s < quizData.sliderTitles.Count; s++)
				{
					// Ensure sliderValues list is up to date
					while (answer.sliderValues.Count < quizData.sliderTitles.Count)
						answer.sliderValues.Add(0f);

					var slider = new HSlider
					{
						MinValue = 0,
						MaxValue = 10,
						Step = 1,
						Value = answer.sliderValues[s],
						SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
					};
					int sliderIdx = s;
					slider.ValueChanged += (value) =>
					{
						quizData.questions[questionIdx].answers[answerIdx].sliderValues[sliderIdx] = (float)value;
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
		string quizTitle = quizData.questions.Count > 0 ? quizData.questions[0].questionTitle : "Untitled_Quiz";
		saveCallback?.Invoke(quizTitle, quizData);
	}

	private void OnBackPressed()
	{
		returnCallback?.Invoke();
	}
}
