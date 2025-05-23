extends LineEdit

@export var slider_path: NodePath

var _slider: HSlider

func _ready():
	# Get the slider node using the exported NodePath
	if slider_path != NodePath():
		_slider = get_node(slider_path)
		# Set the LineEdit text to the slider's current value at startup
		text = str(int(_slider.value))
		# Connect slider's value_changed to update this LineEdit
		_slider.value_changed.connect(_on_slider_value_changed)
	
	# Connect LineEdit's text_changed to update the slider
	text_changed.connect(_on_line_edit_text_changed)

func _on_slider_value_changed(value: float) -> void:
	# Update LineEdit when the slider changes
	var value_str = str(int(value))
	if text != value_str:
		text = value_str

func _on_line_edit_text_changed(new_text: String) -> void:
	# Update slider when the LineEdit changes, if input is valid
	if _slider != null and new_text.is_valid_int():
		var val = int(new_text)
		val = clamp(val, int(_slider.min_value), int(_slider.max_value))
		if int(_slider.value) != val:
			_slider.value = val
