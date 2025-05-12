using QRCoder;
using Godot;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public partial class QRCgameside : Node
{
	 // Method to generate a QR code as a texture

	 public ImageTexture GenerateQRCode(string url)
	{
		var generator = new QRCoder.QRCodeGenerator();
		var data = generator.CreateQrCode(url, QRCoder.QRCodeGenerator.ECCLevel.Q);
		var qr = new QRCoder.QRCode(data);
		Bitmap bitmap = qr.GetGraphic(20);

		// Convert Bitmap to Godot Image
		Image gdImage = ConvertBitmapToGodotImage(bitmap);
		
		// Create a texture from the image
		var texture = new ImageTexture();
		texture.SetImage(gdImage);

		return texture;
	}
	
	// Helper function to convert a Bitmap to a Godot Image
	private Image ConvertBitmapToGodotImage(Bitmap bmp)
	{
		using var ms = new MemoryStream();
		bmp.Save(ms, ImageFormat.Png);
		byte[] pngBytes = ms.ToArray();

		var img = new Image();
		img.LoadPngFromBuffer(pngBytes);
		return img;
	}
}
