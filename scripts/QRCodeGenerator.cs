using Godot;

public partial class QRCodeGenerator : Node
{
	public void GenerateQRCode(TextureRect target, string text)
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
		target.Texture = texture;

		// Opruimen
		qrCode.Call("queue_free");
	}
	
	public static string GetLocalIPAddress()
	{
		foreach (string addr in IP.GetLocalAddresses())
		{
			if (!addr.StartsWith("127."))
				return addr;
		}
		return "";
	}
}
