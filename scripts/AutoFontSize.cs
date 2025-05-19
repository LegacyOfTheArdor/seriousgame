using Godot;

[GlobalClass]
public partial class AutoFontSize : Label
{
	[Export] public int FontSizeMin = 8;
	[Export] public int FontSizeMax = 16;
	[Export] public float FontSizeWidthPercent = 0.9f;
	[Export] public bool AutoSizeOn = true;

	private string _lastText = "";

	public override void _Ready()
	{
		UpdateFontSize();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationResized)
		{
			UpdateFontSize();
		}
	}

	public override void _Process(double delta)
	{
		// Detect text changes (since Label doesn't emit a signal)
		if (Text != _lastText)
		{
			_lastText = Text;
			UpdateFontSize();
		}
	}

	private void UpdateFontSize()
	{
		if (!AutoSizeOn)
			return;

		var font = GetThemeFont("font");
		if (font == null)
			return;

		int fontSize = FontSizeMax;
		float availableWidth = Size.X * FontSizeWidthPercent;
		float availableHeight = Size.Y;

		while (fontSize >= FontSizeMin)
		{
			var textSize = font.GetStringSize(Text, HorizontalAlignment.Left, -1, fontSize);
			if (textSize.X <= availableWidth && textSize.Y <= availableHeight)
				break;

			fontSize--;
		}

		AddThemeFontSizeOverride("font_size", Mathf.Clamp(fontSize, FontSizeMin, FontSizeMax));
	}
}
