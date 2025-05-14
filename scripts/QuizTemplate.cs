using Godot;
using System;
using System.Collections.Generic;

public partial class QuizTemplate : Control
{
	private QuizData quizData;
	private Action returnCallback;

	private VBoxContainer sliderTitleContainer;
	private VBoxContainer questionsContainer;
	private Button backButton;

	public override void _Ready()
	{
		sliderTitleContainer = GetNode<VBoxContainer>("SliderTitleContainer");
		questionsContainer = GetNode<VBoxContainer>("QuestionsContainer");
		backButton = GetNode<Button>("BackButton");
		backButton.Pressed += OnBackPressed;
	}

	public void SetQuizData(QuizData data, Action returnCb)
	{
		quizData = data;
		returnCallback = returnCb;

		ShowSliderTitles();
		ShowQuestions();
	}

	private void ShowSliderTitles()
	{
		foreach (Node child in sliderTitleContainer.GetChildren())
			child.QueueFree();

		foreach (var title in quizData.sliderTitles)
		{
			var label = new Label { Text = title };
			sliderTitleContainer.AddChild(label);
		}
	}

	private void ShowQuestions()
	{
		foreach (Node child in questionsContainer.GetChildren())
			child.QueueFree();

		for (int q = 0; q < quizData.questions.Count; q++)
		{
			var question = quizData.questions[q];
			var questionVBox = new VBoxContainer();

			var questionLabel = new Label { Text = $"Q{q + 1}: {question.questionTitle}" };
			questionVBox.AddChild(questionLabel);

			for (int a = 0; a < question.answers.Count; a++)
			{
				var answer = question.answers[a];
				var answerHBox = new HBoxContainer();

				var answerLabel = new Label { Text = $"A{a + 1}: {answer.answerTitle}" };
				answerHBox.AddChild(answerLabel);

				// Show the slider values as labels (not editable)
				for (int s = 0; s < quizData.sliderTitles.Count; s++)
				{
					string sliderText = quizData.sliderTitles[s];
					float value = answer.sliderValues.Count > s ? answer.sliderValues[s] : 0f;
					var valueLabel = new Label
					{
						Text = $"{sliderText}: {value}"
					};
					answerHBox.AddChild(valueLabel);
				}

				questionVBox.AddChild(answerHBox);
			}

			questionsContainer.AddChild(questionVBox);
		}
	}

	private void OnBackPressed()
	{
		returnCallback?.Invoke();
	}
}
	
