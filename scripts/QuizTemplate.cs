using Godot;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class QuizTemplate : Node2D
{
	
	private List<Panel> panels = new List<Panel>();
	private int activePanel = 0;
	
	private LineEdit quizNameEdit;
	
	public string SelectedQuizFilePath { get; set; }
	
	private Button BackToMain;
	private Button SaveQuiz;

	// --- NEW: Dictionary to keep all value_X LineEdits grouped by name ---
	private Dictionary<string, List<LineEdit>> valueEdits = new();

	public override void _Ready()
	{
		// Initialize panels
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/verbinding"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Question_1"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Answers_1"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Question_2"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Answers_2"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Question_3"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Answers_3"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Question_4"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Answers_4"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Question_5"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Answers_5"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Question_6"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Answers_6"));
		panels.Add(GetNode<Panel>("Node/CanvasLayer/PanelContainer/Einde"));
		
		bool EditMode = ((GameState)GetNode("/root/GameState")).EditMode;
		SetEditMode(EditMode);
		
		quizNameEdit = GetNode<LineEdit>("Node/CanvasLayer/PanelContainer/Einde/Panel/SaveQuizPanel/VBoxContainer/Quiztitle");
		
		
		
		if (((GameState)GetNode("/root/GameState")).QuizName != null)
		{
			quizNameEdit.Text = ((GameState)GetNode("/root/GameState")).QuizName;
		}
		
		BackToMain = GetNode<Button>("Node/CanvasLayer/PanelContainer/Einde/Panel/SaveQuizPanel/VBoxContainer/backtomenu");
		SaveQuiz = GetNode<Button>("Node/CanvasLayer/PanelContainer/Einde/Panel/SaveQuizPanel/VBoxContainer/Savequizz");

		BackToMain.Pressed += OnBackToMenuButtonPressed;
		SaveQuiz.Pressed += OnSaveQuizPressed;
		
		// Initialize score synchronization
		 InitializeValueSync();

		ShowOnlyActivePanel(0);
		
		SelectedQuizFilePath =((GameState)GetNode("/root/GameState")).SelectedQuizFilePath;
		

		if (!string.IsNullOrEmpty(SelectedQuizFilePath))
			LoadQuizDataInUI(SelectedQuizFilePath);
		else
			GD.Print("Geen quizbestand opgegeven!");
	}
	
	private void OnBackToMenuButtonPressed()
	{
		
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
	
	private void OnSaveQuizPressed()
	{
		  
		string quizName = quizNameEdit != null && !string.IsNullOrWhiteSpace(quizNameEdit.Text) ? quizNameEdit.Text: "untitled_quiz";

   		 QuizSaver.SaveQuiz(quizName, this);
	}
	

	private void SetEditMode(bool enabled)
{
	void ProcessNode(Node node)
	{
		if (node is LineEdit lineEdit)
		{
			lineEdit.Editable = enabled;
			lineEdit.FocusMode = enabled ? Control.FocusModeEnum.All : Control.FocusModeEnum.None;
			if (!enabled)
				lineEdit.ReleaseFocus();
		}
		else if (node is TextEdit textEdit)
		{
			textEdit.Editable = enabled;
			textEdit.FocusMode = enabled ? Control.FocusModeEnum.All : Control.FocusModeEnum.None;
			if (!enabled)
				textEdit.ReleaseFocus();
		}
		else if (node is Button button)
		{
			if (button is ImageUploadButton)
			{
				button.Visible = enabled;
			}
			// Example: Hide SaveButton and DeleteButton when not in edit mode
			if (button.Name == "SaveButton" || button.Name == "DeleteButton")
			{
				button.Visible = enabled;
			}
		}
		// Hide other controls by name or type if needed
		if (node.Name == "slidervalues")
		{
			if (node is CanvasItem ci)
				ci.Visible = enabled;
		}

		foreach (Node child in node.GetChildren())
			ProcessNode(child);
	}

	ProcessNode(this);
}



 // Recursive function to find all value_X LineEdits
	private void FindValueEdits(Node node, Regex valueRegex)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is LineEdit le)
			{
				if (valueRegex.IsMatch(le.Name))
				{
					if (!valueEdits.ContainsKey(le.Name))
						valueEdits[le.Name] = new List<LineEdit>();

					valueEdits[le.Name].Add(le);

					string valueName = le.Name;
					le.TextChanged += (string newText) => OnValueChanged(le, valueName, newText);
				}
			}
			FindValueEdits(child, valueRegex);
		}
	}

	private void InitializeValueSync()
	{
		var valueRegex = new Regex(@"^value_\d+$"); // No IgnoreCase needed
		foreach (var panel in panels)
			FindValueEdits(panel, valueRegex);
	}

	private void OnValueChanged(LineEdit source, string valueName, string newText)
	{
		foreach (var edit in valueEdits[valueName])
		{
			if (edit != source && edit.Text != newText)
				edit.Text = newText;
		}
	}

	// Rest of your existing methods remain unchanged
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if ((int)keyEvent.Keycode == 91 && 0 < activePanel)
			{
				GD.Print("left bracket");
				activePanel = Mathf.Max(0, activePanel - 1);
				ShowOnlyActivePanel(activePanel);
				GD.Print(activePanel);
			}
			else if ((int)keyEvent.Keycode == 93 && activePanel < panels.Count)
			{
				GD.Print("right bracket");
				activePanel = Mathf.Min(panels.Count - 1, activePanel + 1);
				ShowOnlyActivePanel(activePanel);
				GD.Print(activePanel);
			}
		}
	}

	private void ShowOnlyActivePanel(int activePanelIndex)
	{
		int i = 0;
		for (i = 0; i < panels.Count; i++)
			
		if(i != activePanelIndex)
		{
			panels[i].Visible = false;
			
		}
		else
		{
			panels[activePanelIndex].Visible = true;
			
		}
	}

	private void ShowBlankTemplate()
	{
		// Voorbeeld: QR-code tonen met een standaardtekst
		GenerateAndShowQRCode("https://voorbeeld.nl");
	}

	private void ShowQuiz()
	{
		// Voorbeeld: QR-code tonen met quiznaam
		//GenerateAndShowQRCode(quizData.Name);
	}

	private void GenerateAndShowQRCode(string text)
	{
		// Laad het GDScript van de QR code generator
		var qrCodeScript = GD.Load<GDScript>("res://addons/QrCode.gd");
		GodotObject qrCode = (GodotObject)qrCodeScript.New();

		// Zet het foutcorrectieniveau
		var errorCorrectionLevels = qrCode.Get("ErrorCorrectionLevel").As<GodotObject>();
		qrCode.Set("error_correct_level", errorCorrectionLevels.Get("LOW"));

		// Genereer de texture van de QR-code
		var texture = (ImageTexture)qrCode.Call("get_texture", text);

		// Zet de texture op de TextureRect genaamd QRCodeTexture
		var qrDisplay = GetNode<TextureRect>("QRCodeTexture");
		qrDisplay.Texture = texture;

		// Opruimen
		qrCode.Call("queue_free");
	}
	
private void LoadQuizDataInUI(string filePath)
{
	if (!FileAccess.FileExists(filePath))
	{
		GD.PrintErr($"Quizbestand niet gevonden: {filePath}");
		return;
	}

	using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
	string jsonText = file.GetAsText();

	var json = new Json();
	var error = json.Parse(jsonText);
	if (error != Error.Ok)
	{
		GD.PrintErr("Fout bij inlezen JSON: " + error.ToString());
		return;
	}

	// Haal de root dictionary uit de JSON
	var quizData = (Godot.Collections.Dictionary<string, Variant>)json.Data;

	if (!quizData.ContainsKey("questions"))
	{
		GD.PrintErr("Quizdata bevat geen 'questions' veld!");
		return;
	}

	// Haal de vragen array uit de dictionary
	var questions = quizData["questions"].AsGodotArray();

	for (int q = 0; q < questions.Count && q < 6; q++) // max 6 vragen
	{
		var questionDict = questions[q].AsGodotDictionary();

		string vraagTextPad = $"Node/CanvasLayer/PanelContainer/Question_{q + 1}/Panel/GridContainer/QuestionText";
		var vraagTextEdit = GetNodeOrNull<TextEdit>(vraagTextPad);
		if (vraagTextEdit != null && questionDict.ContainsKey("text"))
			vraagTextEdit.Text = questionDict["text"].AsString();

		string vraagImagePad = $"Node/CanvasLayer/PanelContainer/Question_{q + 1}/Panel/GridContainer/QuestionImage";
		var vraagImageRect = GetNodeOrNull<TextureRect>(vraagImagePad);
		if (vraagImageRect != null && questionDict.ContainsKey("image"))
		{
			string imagePath = questionDict["image"].AsString();
			if (!string.IsNullOrEmpty(imagePath))
			{
				var texture = GD.Load<Texture2D>(imagePath);
				vraagImageRect.Texture = texture;
			}
			else
			{
				vraagImageRect.Texture = GD.Load<Texture2D>("res://Fotos/placeholder.png");
			}
		}

		// Answers
		if (questionDict.ContainsKey("answers"))
		{
			var answers = questionDict["answers"].AsGodotArray();

			for (int a = 0; a < answers.Count && a < 6; a++) // max 6 answers per question
			{
				var answerDict = answers[a].AsGodotDictionary();

				// Answer text
				string answerTextPad = $"Node/CanvasLayer/PanelContainer/Answers_{q + 1}/Panel/VBoxContainer/AnswerText";
				var answerTextEdit = GetNodeOrNull<TextEdit>(answerTextPad);
				if (answerTextEdit != null && answerDict.ContainsKey("text"))
					answerTextEdit.Text = answerDict["text"].AsString();

				// Answer image
				string answerImagePad = $"Node/CanvasLayer/PanelContainer/Answers_{q + 1}/Panel/VBoxContainer/AnswerImage";
				var answerImageRect = GetNodeOrNull<TextureRect>(answerImagePad);
				if (answerImageRect != null && answerDict.ContainsKey("image"))
				{
					string imagePath = answerDict["image"].AsString();
					if (!string.IsNullOrEmpty(imagePath))
						answerImageRect.Texture = GD.Load<Texture2D>(imagePath);
					else
						answerImageRect.Texture = GD.Load<Texture2D>("res://Fotos/placeholder.png");
				}

					// Options for this answer
					if (answerDict.ContainsKey("options"))
					{
						var options = answerDict["options"].AsGodotArray();
						for (int o = 0; o < options.Count && o < 4; o++) // max 4 options per answer
						{
							var optionDict = options[o].AsGodotDictionary();

							string basePath = $"Node/CanvasLayer/PanelContainer/Answers_{q + 1}/Panel/GridContainer/Option_{o + 1}/";

							// Title
							var titleEdit = GetNodeOrNull<LineEdit>(basePath + "Title");
							if (titleEdit != null && optionDict.ContainsKey("title"))
								titleEdit.Text = optionDict["title"].AsString();

							// Description
							var descEdit = GetNodeOrNull<TextEdit>(basePath + "Description");
							if (descEdit != null && optionDict.ContainsKey("description"))
								descEdit.Text = optionDict["description"].AsString();

							// Positive
							var positiveEdit = GetNodeOrNull<LineEdit>(basePath + "GridContainer/Positive");
							if (positiveEdit != null && optionDict.ContainsKey("positive"))
								positiveEdit.Text = optionDict["positive"].AsString();

							// Negative
							var negativeEdit = GetNodeOrNull<LineEdit>(basePath + "GridContainer/Negative");
							if (negativeEdit != null && optionDict.ContainsKey("negative"))
								negativeEdit.Text = optionDict["negative"].AsString();

							// value_1 to value_4
							for (int v = 1; v <= 4; v++)
							{
								string valueKey = $"score_panel/values/value_{v}";
								var valueEdit = GetNodeOrNull<LineEdit>(basePath + valueKey);
								if (valueEdit != null && optionDict.ContainsKey(valueKey))
									valueEdit.Text = optionDict[valueKey].AsString();
							}

							// Score1 to Score4 (assuming HSlider)
							for (int s = 1; s <= 4; s++)
							{
						   	 string scoreKey = $"score_panel/sliders/Score{s}";
						   	 var scoreSlider = GetNodeOrNull<HSlider>(basePath + scoreKey);
						  	  if (scoreSlider != null && optionDict.ContainsKey(scoreKey))
							 	   scoreSlider.Value = optionDict[scoreKey].AsDouble();
							}
						} // end options loop
					} // end if options
				} // end answers loop
			} // end if answers
		} // end questions loop
	} // end method

}	
