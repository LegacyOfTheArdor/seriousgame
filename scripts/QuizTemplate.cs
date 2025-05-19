using Godot;
using System.Text.Json;
using System.Collections.Generic;

public partial class QuizTemplate : Node2D
{
	private QuizData quizData;

	private List<Panel> panels = new List<Panel>();
	private int activePanel = 0;

	public override void _Ready()
	{
		// Voeg al je panels toe in de juiste volgorde
		panels.Add(GetNode<Panel>("MarginContainer/verbinding"));
		panels.Add(GetNode<Panel>("MarginContainer/Question_1"));
		panels.Add(GetNode<Panel>("MarginContainer/Answers_1"));
		panels.Add(GetNode<Panel>("MarginContainer/Question_2"));
		panels.Add(GetNode<Panel>("MarginContainer/Answers_2"));
		panels.Add(GetNode<Panel>("MarginContainer/Question_3"));
		panels.Add(GetNode<Panel>("MarginContainer/Answers_3"));
		panels.Add(GetNode<Panel>("MarginContainer/Question_4"));
		panels.Add(GetNode<Panel>("MarginContainer/Answers_4"));
		// Voeg meer toe als je meer panels hebt

		ShowOnlyActivePanel(0);
	}

	public override void _Input(InputEvent @event)
	{
		
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			// [ toets (left bracket)
			if ((int)keyEvent.Keycode == 91 && 0 < activePanel )
			{
				 GD.Print("left bracket");
				activePanel = Mathf.Max(0, activePanel - 1);
				ShowOnlyActivePanel(activePanel);
				GD.Print(activePanel);
			}
			// ] toets (right bracket)
			else if ((int)keyEvent.Keycode == 93 && activePanel < panels.Count )
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
