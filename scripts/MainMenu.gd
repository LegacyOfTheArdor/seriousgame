extends CanvasLayer

@onready var panel: Panel = $PanelContainer/outerborder/Panel
@onready var play_button: Button = $PanelContainer/outerborder/Panel/VBoxContainer/PlayButton
@onready var edit_button: Button = $PanelContainer/outerborder/Panel/VBoxContainer/EditButton
@onready var exit_button: Button = $PanelContainer/outerborder/Panel/VBoxContainer/ExitButton

func _ready():
	print(ProjectSettings.globalize_path("user://"))

	play_button.pressed.connect(_on_play_button_pressed)
	edit_button.pressed.connect(_on_edit_button_pressed)
	exit_button.pressed.connect(_on_exit_button_pressed)

	panel.set_anchors_and_offsets_preset(Control.LAYOUT_PRESET_FULL_RECT)

func _on_play_button_pressed():
	var game_state = get_node("/root/GameState")
	game_state.edit_mode = false
	get_tree().change_scene_to_file("res://scenes/QuizSelection.tscn")

func _on_edit_button_pressed():
	var game_state = get_node("/root/GameState")
	game_state.edit_mode = true
	get_tree().change_scene_to_file("res://scenes/QuizSelection.tscn")

func _on_exit_button_pressed():
	get_tree().quit()
