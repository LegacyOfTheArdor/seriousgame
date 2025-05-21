using Godot;
using System;

public partial class QuizSaver : Node
{
	[Export] public NodePath PanelContainerPath { get; set; }
	[Export] public NodePath QuizTitleLineEditPath { get; set; }
	[Export] public NodePath SaveButtonPath { get; set; }

	public override void _Ready()
	{
		var saveButton = GetNode<Button>(SaveButtonPath);
		saveButton.Pressed += OnSavePressed;
	}

	private void OnSavePressed()
	{
		var panelContainer = GetNode(PanelContainerPath);
		var quizTitleLineEdit = GetNode<LineEdit>(QuizTitleLineEditPath);
		string quizTitle = quizTitleLineEdit.Text.StripEdges();
		if (string.IsNullOrEmpty(quizTitle))
		{
			GD.PrintErr("Quiz title is empty!");
			return;
		}

		var quizData = new Godot.Collections.Dictionary();
		var universalValues = new Godot.Collections.Dictionary();

		// --- Universal Values: Get from first score_panel/values ---
		var firstAnswersPanel = panelContainer.GetNodeOrNull("answers_1");
		if (firstAnswersPanel != null)
		{
			var scorePanel = firstAnswersPanel.GetNodeOrNull("score_panel");
			if (scorePanel != null)
			{
				var valuesGrid = scorePanel.GetNodeOrNull("values");
				if (valuesGrid != null)
				{
					foreach (Node valueEdit in valuesGrid.GetChildren())
					{
						if (valueEdit is LineEdit lineEdit)
						{
							universalValues[lineEdit.Name] = lineEdit.Text;
						}
					}
				}
			}
		}
		quizData["universal_values"] = universalValues;

		for (int i = 1; i <= 6; i++)
		{
			// --- QUESTION ---
			var questionDict = new Godot.Collections.Dictionary();
			var questionPanel = panelContainer.GetNodeOrNull($"question_{i}");
			if (questionPanel != null)
			{
				var qGrid = questionPanel.GetNodeOrNull("Panel/GridContainer");
				if (qGrid != null)
				{
					var questionImage = qGrid.GetNodeOrNull<TextureRect>("QuestionImage");
					if (questionImage != null && questionImage.Texture is ImageTexture imgTex)
					{
						var img = imgTex.GetImage();
						if (img != null)
						{
							string imagesDir = "user://seriousgame";
							DirAccess.MakeDirAbsolute(imagesDir);
							string imagePath = $"{imagesDir}/{quizTitle}_q{i}_question.png";
							img.SavePng(imagePath);
							questionDict["image"] = imagePath;
						}
					}
					var questionText = qGrid.GetNodeOrNull<TextEdit>("QuestionText");
					if (questionText != null)
						questionDict["text"] = questionText.Text;
				}
			}
			quizData[$"question_{i}"] = questionDict;

			// --- ANSWER ---
			var answerDict = new Godot.Collections.Dictionary();
			var answerPanel = panelContainer.GetNodeOrNull($"answers_{i}");
			if (answerPanel != null)
			{
				// VBoxContainer
				var vbox = answerPanel.GetNodeOrNull("Panel/VBoxContainer");
				if (vbox != null)
				{
					var answerImage = vbox.GetNodeOrNull<TextureRect>("AnswerImage");
					if (answerImage != null && answerImage.Texture is ImageTexture ansImgTex)
					{
						var img = ansImgTex.GetImage();
						if (img != null)
						{
							string imagesDir = "user://quiz_images";
							DirAccess.MakeDirAbsolute(imagesDir);
							string imagePath = $"{imagesDir}/{quizTitle}_q{i}_answer.png";
							img.SavePng(imagePath);
							answerDict["image"] = imagePath;
						}
					}
					var answerText = vbox.GetNodeOrNull<TextEdit>("AnswerText");
					if (answerText != null)
						answerDict["text"] = answerText.Text;
				}

				// GridContainer with 4 buttons, each with Title (LineEdit) and Description (TextEdit)
				var grid = answerPanel.GetNodeOrNull("Panel/GridContainer");
				if (grid != null)
				{
					var buttonsData = new Godot.Collections.Array();
					foreach (Node btn in grid.GetChildren())
					{
						if (btn is Button button)
						{
							var btnDict = new Godot.Collections.Dictionary();
							var titleEdit = button.GetNodeOrNull<LineEdit>("Title");
							var descEdit = button.GetNodeOrNull<TextEdit>("Description");
							if (titleEdit != null) btnDict["title"] = titleEdit.Text;
							if (descEdit != null) btnDict["description"] = descEdit.Text;
							btnDict["button_name"] = button.Name;
							buttonsData.Add(btnDict);
						}
					}
					answerDict["options"] = buttonsData;
				}

				// score_panel
				var scorePanel = answerPanel.GetNodeOrNull("score_panel");
				if (scorePanel != null)
				{
					// sliders (BoxContainer with 4 sliders named score1, score2, score3, score4)
					var slidersBox = scorePanel.GetNodeOrNull("sliders");
					if (slidersBox != null)
					{
						var slidersDict = new Godot.Collections.Dictionary();
						foreach (Node slider in slidersBox.GetChildren())
						{
							if (slider is HSlider hslider)
								slidersDict[hslider.Name] = hslider.Value;
						}
						answerDict["sliders"] = slidersDict;
					}
				}
			}
			quizData[$"answers_{i}"] = answerDict;
		}

		// --- Ensure the quizzes_seriousgame directory exists ---
		string dirPath = "user://seriousgame/quizzes";
		DirAccess.MakeDirAbsolute(dirPath);

		// --- Save to file as JSON in that directory ---
		string savePath = $"{dirPath}/{quizTitle}.json";
		using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(quizData, "\t"));
		file.Close();

		GD.Print($"Quiz saved to: {savePath}");
	}
}
