extends CanvasLayer

@onready var quiz_grid: GridContainer = $PanelContainer/outerborder/Panel/QuizGrid
@onready var new_quiz_button: Button = $PanelContainer/outerborder/Panel/MenuGrid/NewQuizButton
@onready var next_page_button: Button = $PanelContainer/outerborder/Panel/MenuGrid/NextPageButton
@onready var prev_page_button: Button = $PanelContainer/outerborder/Panel/MenuGrid/PrevPageButton

var quiz_files: Array = []
var page: int = 0
var quizzes_per_page: int = 20  # 10 columns × 2 rows
var quiz_button_scene = preload("res://scenes/SelectQuizButton.tscn")

func _ready():
	new_quiz_button.pressed.connect(_on_new_quiz_pressed)
	next_page_button.pressed.connect(_on_next_page_pressed)
	prev_page_button.pressed.connect(_on_prev_page_pressed)
	
	quiz_grid.columns = 10
	quiz_grid.size_flags_vertical = Control.SIZE_EXPAND_FILL
	
	_load_quiz_files()
	_update_ui()

func _load_quiz_files():
	quiz_files.clear()
	var dir_path = "user://seriousgame/quizzes/"
	var dir = DirAccess.open(dir_path)
	
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if not dir.current_is_dir() and file_name.ends_with(".json"):
				quiz_files.append(file_name.get_basename())
			file_name = dir.get_next()
		dir.list_dir_end()

func _update_ui():
	var game_state = get_node("/root/GameState")
	new_quiz_button.visible = game_state.edit_mode
	
	var total_pages = ceil(quiz_files.size() / float(quizzes_per_page))
	next_page_button.visible = page < total_pages - 1
	prev_page_button.visible = page > 0
	
	# Clear existing buttons
	for child in quiz_grid.get_children():
		child.queue_free()
	
	var start_idx = page * quizzes_per_page
	var end_idx = min(start_idx + quizzes_per_page, quiz_files.size())
	
	for i in range(start_idx, end_idx):
		var quiz_name = quiz_files[i]
		var btn = quiz_button_scene.instantiate()
		
		if not btn is Button:
			push_error("QuizButton scene does not inherit Button!")
			continue
		
		btn.text = quiz_name
		btn.pressed.connect(_on_quiz_button_pressed.bind(quiz_name))
		quiz_grid.add_child(btn)

func _on_new_quiz_pressed():
	var game_state = get_node("/root/GameState")
	game_state.selected_quiz_file_path = null
	get_tree().change_scene_to_file("res://scenes/QuizTemplate.tscn")

func _on_quiz_button_pressed(quiz_name: String):
	var game_state = get_node("/root/GameState")
	game_state.selected_quiz_file_path = "user://seriousgame/quizzes/%s.json" % quiz_name
	game_state.quiz_name = quiz_name
	get_tree().change_scene_to_file("res://scenes/QuizTemplate.tscn")

func _on_next_page_pressed():
	page += 1
	_update_ui()

func _on_prev_page_pressed():
	page -= 1
	_update_ui()
