using Godot;
using System;

public partial class ImageUploadButton : Button
{
	[Export] public NodePath TextureRectPath { get; set; }
	[Export] public NodePath FileDialogPath { get; set; }

	private TextureRect _targetTextureRect;
	private FileDialog _fileDialog;
	private string _imagesDir;

	public override void _Ready()
	{
		// Prepare user://seriousgame directory
		_imagesDir = "user://seriousgame";
		DirAccess.MakeDirAbsolute(_imagesDir);

		// Get the TextureRect node
		if (!TextureRectPath.IsEmpty)
			_targetTextureRect = GetNode<TextureRect>(TextureRectPath);

		// Get the existing FileDialog node
		if (!FileDialogPath.IsEmpty)
			_fileDialog = GetNode<FileDialog>(FileDialogPath);

		// Configure FileDialog (if not already set in Inspector)
		if (_fileDialog != null)
		{
			_fileDialog.Access = FileDialog.AccessEnum.Filesystem;
			_fileDialog.Filters = new string[] { "*.png ; PNG Images", "*.jpg ; JPEG Images", "*.jpeg ; JPEG Images" };
			_fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
			_fileDialog.Title = "Select an Image";
			// Optionally set the start directory:
			 _fileDialog.CurrentDir = OS.GetUserDataDir();

			_fileDialog.FileSelected += OnFileSelected;
		}

		Pressed += () => _fileDialog?.PopupCentered();
	}

	private void OnFileSelected(string path)
	{
		string filename = path.GetFile();
		string destPath = $"{_imagesDir}/{filename}";

		// Copy the file to user://seriousgame/
		using (var src = FileAccess.Open(path, FileAccess.ModeFlags.Read))
		using (var dst = FileAccess.Open(destPath, FileAccess.ModeFlags.Write))
		{
			dst.StoreBuffer(src.GetBuffer((long)src.GetLength()));
		}

		GD.Print($"Copied {filename} to {destPath}");

		// Load and display in TextureRect
		if (_targetTextureRect != null)
		{
			var image = new Image();
			var err = image.Load(destPath);
			if (err == Error.Ok)
			{
				var texture = ImageTexture.CreateFromImage(image);
				_targetTextureRect.Texture = texture;
			}
			else
			{
				GD.PrintErr($"Failed to load image at {destPath}");
			}
		}
	}
}
