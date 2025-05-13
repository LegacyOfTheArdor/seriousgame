using Godot;

public partial class Quiz : Control
{
	 private QRCodeGenerator qrCodeGenerator;


	public override void _Ready()
	{
		// Add the QRCodeGenerator as a child or instantiate it as needed
		qrCodeGenerator = new QRCodeGenerator();
		AddChild(qrCodeGenerator);

		// Get local IP address
		string ip = QRCodeGenerator.GetLocalIPAddress();
		if (string.IsNullOrEmpty(ip))
		{
			GD.PrintErr("Could not find local IP address.");
			return;
		}

		// Build the connection URL
		string url = $"http://{ip}:8080";

		// Get reference to the TextureRect node
		var qrDisplay = GetNode<TextureRect>("QRCodeDisplay");

		// Generate and display the QR code
		qrCodeGenerator.GenerateQRCode(qrDisplay, url);
	}

	private void OnBackButtonPressed()
	{
		//GetTree().ChangeScene("res://scenes/MainMenu.tscn");
	}




	public void LoadQuiz(string quizName)
	{
		//currentQuiz = quizName;
		// Load quiz data and set up the game
	}

	// Implement quiz gameplay logic
}
