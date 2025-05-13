using Godot;
using System;

public partial class QRCodeGenerator : Node
{
	/// <summary>
	/// Generates a QR code texture for the given URL and assigns it to the specified TextureRect.
	/// </summary>
	public void GenerateQRCode(TextureRect target, string url)
	{
		// Load the GDScript QR code generator
		var qrCodeScript = GD.Load<GDScript>("res://addons/QrCode.gd");
		GodotObject qrCode = (GodotObject)qrCodeScript.New();


		// Set error correction level (optional)
		var errorCorrectionLevels = qrCode.Get("ErrorCorrectionLevel").As<GodotObject>();
		qrCode.Set("error_correct_level", errorCorrectionLevels.Get("LOW"));;

		// Generate the QR code texture
		var texture = (ImageTexture)qrCode.Call("get_texture", url);

		// Assign to the TextureRect node
		target.Texture = texture;

		// Clean up
		qrCode.Call("queue_free");
	}

	/// <summary>
	/// Returns the first non-localhost IP address found.
	/// </summary>
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
