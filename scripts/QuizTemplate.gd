extends Node2D

# Declare member variables
var panel_manager
var value_sync
var quiz_loader

var panels: Array = []
var active_panel: int = 0

var quiz_name_edit: LineEdit

# This stores the selected quiz file path and can be set from outside
var selected_quiz_file_path: String = ""

var back_to_main: Button
var save_quiz: Button

func _ready():
	# Initialize panels list
	panels = [
		$Node/CanvasLayer/PanelContainer/verbinding,
		$Node/CanvasLayer/PanelContainer/Question_1,
		$Node/CanvasLayer/PanelContainer/Answers_1,
		$Node/CanvasLayer/PanelContainer/Question_2,
		$Node/CanvasLayer/PanelContainer/Answers_2,
		$Node/CanvasLayer/PanelContainer/Question_3,
		$Node/CanvasLayer/PanelContainer/Answers_3,
		$Node/CanvasLayer/PanelContainer/Question_4,
		$Node/CanvasLayer/PanelContainer/Answers_4,
		$Node/CanvasLayer/PanelContainer/Question_5,
		$Node/CanvasLayer/PanelContainer/Answers_5,
		$Node/CanvasLayer/PanelContainer/Question_6,
		$Node/CanvasLayer/PanelContainer/Answers_6,
		$Node/CanvasLayer/PanelContainer/Einde
	]
	
	show_connection_qr_code()
	
	panel_manager = PanelManager.new(panels)
	value_sync = ValueSyncManager.new()
	value_sync.initialize(panels)

	EditModeHandler.apply_edit_mode(self, get_node("/root/GameState").edit_mode)

	# Set the selected_quiz_file_path from GameState (or any other source)
	selected_quiz_file_path = get_node("/root/GameState").selected_quiz_file_path

	# Initialize the QuizLoader with this node as root
	quiz_loader = QuizLoader.new()
	
	# Load the quiz if the path is valid
	if selected_quiz_file_path != "":
		quiz_loader.load_from_file(selected_quiz_file_path, self)

func _input(event):
	if event is InputEventKey and event.pressed and not event.echo:
		if event.keycode == KEY_BRACKETLEFT:
			panel_manager.previous_panel()
		elif event.keycode == KEY_BRACKETRIGHT:
			panel_manager.next_panel()

func show_connection_qr_code():
	# Get the QRCodeGenerator node (which is also a TextureRect)
	var qr_code_node = $Node/CanvasLayer/PanelContainer/verbinding/Panel/GridContainer/QRCodeTexture

	var ip = QRCodeGenerator.get_local_ip_address()
	var port = 12345
	var qr_text = "%s:%d" % [ip, port]

	qr_code_node.generate_qr_code(qr_text)
