using Godot;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class QuizTemplate : Node2D
{
	private QuizData quizData;
	private List<Panel> panels = new List<Panel>();
	private int activePanel = 0;
	
	private Button BackToMain;

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
		
		BackToMain = GetNode<Button>("Node/CanvasLayer/PanelContainer/Einde/Panel/SaveQuizPanel/VBoxContainer/backtomenu");

		BackToMain.Pressed += OnBackToMenuButtonPressed;
		
		// Initialize score synchronization
		 InitializeValueSync();

		ShowOnlyActivePanel(0);
	}
	
	private void OnBackToMenuButtonPressed()
	{
		
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
	
	private void SetEditMode(bool enabled)
	{
		// Helper function to recursively process nodes
		void ProcessNode(Node node)
		{
			// Disable editing for LineEdit
			if (node is LineEdit lineEdit)
			{
				lineEdit.Editable = enabled;
			}
			// Disable editing for TextEdit
			else if (node is TextEdit textEdit)
			{
				textEdit.Editable = enabled;
			}
			// Hide buttons with UploadImageButton script attached
			else if (node is Button button)
			{
			// Check if ImageUploadButton script is attached
				if (button is ImageUploadButton)
				{
   				 button.Visible = enabled;
				}
			}
		// Recurse for all children
		foreach (Node child in node.GetChildren())
			ProcessNode(child);
	}

	// Start from this node (QuizTemplate) or a specific root if you want
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
		GenerateAndShowQRCode(quizData.Name);
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
}
