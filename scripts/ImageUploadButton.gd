extends Button

@export var texture_rect_path: NodePath
@export var file_dialog_path: NodePath

var _target_texture_rect: TextureRect
var _file_dialog: FileDialog
var _images_dir: String

func _ready():
	# Prepare user://seriousgame directory
	_images_dir = "user://seriousgame"
	DirAccess.make_dir_absolute(_images_dir)

	# Get the TextureRect node
	if texture_rect_path != NodePath():
		_target_texture_rect = get_node(texture_rect_path)

	# Get the existing FileDialog node
	if file_dialog_path != NodePath():
		_file_dialog = get_node(file_dialog_path)

	# Configure FileDialog (if not already set in Inspector)
	if _file_dialog:
		_file_dialog.access = FileDialog.ACCESS_FILESYSTEM
		_file_dialog.filters = [
			"*.png ; PNG Images",
			"*.jpg ; JPEG Images",
			"*.jpeg ; JPEG Images"
		]
		_file_dialog.file_mode = FileDialog.FILE_MODE_OPEN_FILE
		_file_dialog.title = "Select an Image"
		# Optionally set the start directory:
		_file_dialog.current_dir = OS.get_user_data_dir()
		_file_dialog.file_selected.connect(_on_file_selected)

	self.pressed.connect(_on_button_pressed)

func _on_button_pressed():
	if _file_dialog:
		_file_dialog.popup_centered()

func _on_file_selected(path: String):
	var filename = path.get_file()
	var dest_path = "%s/%s" % [_images_dir, filename]

	# Copy the file to user://seriousgame/
	var src = FileAccess.open(path, FileAccess.READ)
	var dst = FileAccess.open(dest_path, FileAccess.WRITE)
	if src and dst:
		dst.store_buffer(src.get_buffer(src.get_length()))
		src.close()
		dst.close()
		print("Copied %s to %s" % [filename, dest_path])
	else:
		push_error("Failed to copy file!")

	# Load and display in TextureRect
	if _target_texture_rect:
		var image = Image.new()
		var err = image.load(dest_path)
		if err == OK:
			var texture = ImageTexture.create_from_image(image)
			_target_texture_rect.texture = texture
		else:
			push_error("Failed to load image at %s" % dest_path)
