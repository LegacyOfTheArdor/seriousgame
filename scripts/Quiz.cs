using Godot;

public partial class Quiz : Control
{
	 private QRCgameside qrcGameside;

	public override void _Ready()
	{
		// Create a new instance of the RCgameside script
		qrcGameside = new QRCgameside();

		// Generate and display the QR code
		GenerateQRCode();

		// Optionally connect back button
		var backButton = GetNode<Button>("Panel/BackButton");
		backButton.Pressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed()
	{
		GetTree().ChangeScene("res://scenes/MainMenu.tscn");
	}

	private void GenerateQRCode()
	{
		string url = "http://192.168.1.100:8080/controller.html";
		ImageTexture texture = qrcGameside.GenerateQRCode(url);

		var qrRect = GetNode<TextureRect>("Panel/TextureRect");
		qrRect.Texture = texture;
	}


	public void LoadQuiz(string quizName)
	{
		currentQuiz = quizName;
		// Load quiz data and set up the game
	}

	// Implement quiz gameplay logic
}
