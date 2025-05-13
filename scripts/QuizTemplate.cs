using Godot;
using System;
using System.Collections.Generic;

public partial class QuizTemplate : Node
{
	// Quiz title fields
	[Export] public LineEdit TitleEdit;
	[Export] public Label TitleLabel;

	// Containers for slider titles, questions, and answers
	[Export] public VBoxContainer SliderTitlesContainer;
	[Export] public VBoxContainer QuestionsContainer;

	[Export] public Button SaveButton;
	[Export] public Button BackButton;
	[Export] public Button ToggleEditButton; // Optional: button to toggle modes

	private QuizData quizData;
	private Action<string, QuizData> saveCallback;
	private Action backCallback;

	private bool isEditMode = true;

	public override void _Ready()
	{
		// Optional: connect toggle button if present
		if (ToggleEditButton != null)
			ToggleEditButton.Pressed += () => SetEditMode(!isEditMode);
	}

	public void SetQuizData(QuizData data, Action<string, QuizData> onSave, Action onBack)
	{
		quizData = data;
		saveCallback = onSave;
		backCallback = onBack;

		BuildUI();
		SetEditMode(true); // Start in edit mode
	}

	// Build all UI fields for quiz title, slider titles, questions, and answers
	private void BuildUI()
	{
		// --- Quiz Title ---
		TitleEdit.Text = quizData.quizTitle;
		TitleLabel.Text = quizData.quizTitle;

		// --- Slider Titles ---
		SliderTitlesContainer.QueueFreeChildren();
		for (int i = 0; i < quizData.sliderTitles.Count; i++)
		{
			var sliderTitleEdit = new LineEdit { Name = $"SliderTitleEdit{i}", Text = quizData.sliderTitles[i] };
			var sliderTitleLabel = new Label { Name = $"SliderTitleLabel{i}", Text = quizData.sliderTitles[i] };
			sliderTitleEdit.TextChanged += (txt) => { quizData.sliderTitles[i] = txt; sliderTitleLabel.Text = txt; };
			sliderTitleEdit.AddToGroup("edit_fields");
			sliderTitleLabel.AddToGroup("display_fields");
			SliderTitlesContainer.AddChild(sliderTitleEdit);
			SliderTitlesContainer.AddChild(sliderTitleLabel);
		}

		// --- Questions and Answers ---
		QuestionsContainer.QueueFreeChildren();
		for (int q = 0; q < quizData.questions.Count; q++)
		{
			var question = quizData.questions[q];

			// Question title
			var questionEdit = new LineEdit { Name = $"Question{q}Edit", Text = question.questionTitle };
			var questionLabel = new Label { Name = $"Question{q}Label", Text = question.questionTitle };
			int qIdx = q;
			questionEdit.TextChanged += (txt) => { quizData.questions[qIdx].questionTitle = txt; questionLabel.Text = txt; };
			questionEdit.AddToGroup("edit_fields");
			questionLabel.AddToGroup("display_fields");
			QuestionsContainer.AddChild(new Label { Text = $"Question {q + 1}:" });
			QuestionsContainer.AddChild(questionEdit);
			QuestionsContainer.AddChild(questionLabel);

			// Answers
			for (int a = 0; a < question.answers.Count; a++)
			{
				var answer = question.answers[a];
				int aIdx = a;

				// Answer title
				var answerTitleEdit = new LineEdit { Name = $"Q{q}A{a}TitleEdit", Text = answer.answerTitle };
				var answerTitleLabel = new Label { Name = $"Q{q}A{a}TitleLabel", Text = answer.answerTitle };
				answerTitleEdit.TextChanged += (txt) => { quizData.questions[qIdx].answers[aIdx].answerTitle = txt; answerTitleLabel.Text = txt; };
				answerTitleEdit.AddToGroup("edit_fields");
				answerTitleLabel.AddToGroup("display_fields");
				QuestionsContainer.AddChild(new Label { Text = $"Answer {a + 1}:" });
				QuestionsContainer.AddChild(answerTitleEdit);
				QuestionsContainer.AddChild(answerTitleLabel);

				// Description
				var descEdit = new LineEdit { Name = $"Q{q}A{a}DescEdit", Text = answer.description };
				var descLabel = new Label { Name = $"Q{q}A{a}DescLabel", Text = answer.description };
				descEdit.TextChanged += (txt) => { quizData.questions[qIdx].answers[aIdx].description = txt; descLabel.Text = txt; };
				descEdit.AddToGroup("edit_fields");
				descLabel.AddToGroup("display_fields");
				QuestionsContainer.AddChild(new Label { Text = "Description:" });
				QuestionsContainer.AddChild(descEdit);
				QuestionsContainer.AddChild(descLabel);

				// Upside
				var upsideEdit = new LineEdit { Name = $"Q{q}A{a}UpsideEdit", Text = answer.upside };
				var upsideLabel = new Label { Name = $"Q{q}A{a}UpsideLabel", Text = answer.upside };
				upsideEdit.TextChanged += (txt) => { quizData.questions[qIdx].answers[aIdx].upside = txt; upsideLabel.Text = txt; };
				upsideEdit.AddToGroup("edit_fields");
				upsideLabel.AddToGroup("display_fields");
				QuestionsContainer.AddChild(new Label { Text = "Upside:" });
				QuestionsContainer.AddChild(upsideEdit);
				QuestionsContainer.AddChild(upsideLabel);

				// Downside
				var downsideEdit = new LineEdit { Name = $"Q{q}A{a}DownsideEdit", Text = answer.downside };
				var downsideLabel = new Label { Name = $"Q{q}A{a}DownsideLabel", Text = answer.downside };
				downsideEdit.TextChanged += (txt) => { quizData.questions[qIdx].answers[aIdx].downside = txt; downsideLabel.Text = txt; };
				downsideEdit.AddToGroup("edit_fields");
				downsideLabel.AddToGroup("display_fields");
				QuestionsContainer.AddChild(new Label { Text = "Downside:" });
				QuestionsContainer.AddChild(downsideEdit);
				QuestionsContainer.AddChild(downsideLabel);

				// Sliders for values
				for (int s = 0; s < quizData.sliderTitles.Count; s++)
				{
					var sliderLabel = new Label { Text = quizData.sliderTitles[s] };
					var slider = new HSlider
					{
						MinValue = 0,
						MaxValue = 10,
						Step = 1,
						Value = answer.sliderValues[s]
					};
					int sIdx = s;
					slider.ValueChanged += (val) => quizData.questions[qIdx].answers[aIdx].sliderValues[sIdx] = (float)val;
					slider.AddToGroup("edit_fields"); // Only editable in edit mode
					QuestionsContainer.AddChild(sliderLabel);
					QuestionsContainer.AddChild(slider);
				}

				QuestionsContainer.AddChild(new HSeparator());
			}
			QuestionsContainer.AddChild(new VSeparator());
		}

		// Connect save/back buttons
		SaveButton.Pressed += OnSavePressed;
		BackButton.Pressed += OnBackPressed;
	}

	// Toggle between edit mode (LineEdits/sliders) and display mode (Labels)
	public void SetEditMode(bool edit)
	{
		isEditMode = edit;

		// Quiz title
		TitleEdit.Visible = edit;
		TitleLabel.Visible = !edit;
		if (!edit) TitleLabel.Text = TitleEdit.Text;

		// All dynamic fields
		foreach (Node node in GetTree().GetNodesInGroup("edit_fields"))
			node.Visible = edit;
		foreach (Node node in GetTree().GetNodesInGroup("display_fields"))
			node.Visible = !edit;
	}

	private void OnSavePressed()
	{
		quizData.quizTitle = TitleEdit.Text;
		saveCallback?.Invoke(quizData.quizTitle, quizData);
	}

	private void OnBackPressed()
	{
		backCallback?.Invoke();
	}
}
