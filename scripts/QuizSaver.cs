using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class QuizSaver : Node
{
	public static void SaveQuiz(string quizName, Node quizRoot)
	{
		string dirPath = "user://seriousgame/quizzes";
		string absDirPath = ProjectSettings.GlobalizePath(dirPath);

		if (!DirAccess.DirExistsAbsolute(absDirPath))
			DirAccess.MakeDirRecursiveAbsolute(absDirPath);

		string filePath = $"{dirPath}/{quizName}.json";

		QuizData quizData = new();

		for (int q = 1; q <= 6; q++)
		{
			// Get question text and image
			string questionTextPath = $"Node/CanvasLayer/PanelContainer/Question_{q}/Panel/GridContainer/QuestionText";
			string questionImagePath = $"Node/CanvasLayer/PanelContainer/Question_{q}/Panel/GridContainer/QuestionImage";
			var questionTextEdit = quizRoot.GetNodeOrNull<TextEdit>(questionTextPath);
			var questionImageNode = quizRoot.GetNodeOrNull<TextureRect>(questionImagePath);

			if (questionTextEdit == null) continue;

			QuestionData question = new()
			{
				text = questionTextEdit.Text,
				image = (questionImageNode != null && questionImageNode.Texture != null) ? questionImageNode.Texture.ResourcePath : ""
			};

			// 6 answers per question
			for (int a = 1; a <= 6; a++)
			{
				string answerTextPath = $"Node/CanvasLayer/PanelContainer/Answers_{q}/Panel/VBoxContainer/AnswerText";
				string answerImagePath = $"Node/CanvasLayer/PanelContainer/Answers_{q}/Panel/VBoxContainer/AnswerImage";
				var answerTextEdit = quizRoot.GetNodeOrNull<TextEdit>(answerTextPath);
				var answerImageNode = quizRoot.GetNodeOrNull<TextureRect>(answerImagePath);

				if (answerTextEdit == null) continue;

				AnswerData answer = new()
				{
					text = answerTextEdit.Text,
					image = (answerImageNode != null && answerImageNode.Texture != null) ? answerImageNode.Texture.ResourcePath : ""
				};

				// 4 options per answer
				for (int o = 1; o <= 4; o++)
				{
					string basePath = $"Node/CanvasLayer/PanelContainer/Answers_{q}/Panel/GridContainer/Option_{0}/";
					var option = new OptionData
					{
						title = quizRoot.GetNodeOrNull<LineEdit>(basePath + "Title")?.Text ?? "",
						description = quizRoot.GetNodeOrNull<TextEdit>(basePath + "Description")?.Text ?? "",
						positive = quizRoot.GetNodeOrNull<LineEdit>(basePath + "GridContainer/Positive")?.Text ?? "",
						negative = quizRoot.GetNodeOrNull<LineEdit>(basePath + "GridContainer/Negative")?.Text ?? "",
						value_1 = quizRoot.GetNodeOrNull<LineEdit>(basePath + "score_panel/values/value_1")?.Text ?? "",
						value_2 = quizRoot.GetNodeOrNull<LineEdit>(basePath + "score_panel/values/value_2")?.Text ?? "",
						value_3 = quizRoot.GetNodeOrNull<LineEdit>(basePath + "score_panel/values/value_3")?.Text ?? "",
						value_4 = quizRoot.GetNodeOrNull<LineEdit>(basePath + "score_panel/values/value_4")?.Text ?? "",
						score1 = quizRoot.GetNodeOrNull<Slider>(basePath + "score_panel/sliders/score1")?.Value ?? 0.0,
						score2 = quizRoot.GetNodeOrNull<Slider>(basePath + "score_panel/sliders/score2")?.Value ?? 0.0,
						score3 = quizRoot.GetNodeOrNull<Slider>(basePath + "score_panel/sliders/score3")?.Value ?? 0.0,
						score4 = quizRoot.GetNodeOrNull<Slider>(basePath + "score_panel/sliders/score4")?.Value ?? 0.0
					};
					answer.options.Add(option);
				}

				question.answers.Add(answer);
			}

			quizData.questions.Add(question);
		}


		var options = new JsonSerializerOptions { WriteIndented = true };
		string json = JsonSerializer.Serialize(quizData, options);

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		file.StoreString(json);
		file.Close();

		GD.Print($"Quiz saved to {filePath}");
	}
}
