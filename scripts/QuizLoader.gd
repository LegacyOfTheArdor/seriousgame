# QuizLoader.gd
class_name QuizLoader
extends RefCounted

var _root: Node = null

func _init(root: Node) -> void:
	_root = root

func load_from_file(file_path: String) -> void:
	if not FileAccess.file_exists(file_path):
		push_error("Quizbestand niet gevonden: %s" % file_path)
		return

	var file = FileAccess.open(file_path, FileAccess.READ)
	var json_text = file.get_as_text()
	file.close()

	var quiz_data = JSON.parse_string(json_text)
	if typeof(quiz_data) != TYPE_DICTIONARY:
		push_error("Fout bij inlezen JSON: geen dictionary")
		return

	if not quiz_data.has("questions"):
		push_error("Quizdata bevat geen 'questions' veld!")
		return

	var questions = quiz_data["questions"]
	for q in range(min(questions.size(), 6)): # max 6 vragen
		var question_dict = questions[q]

		# Question text
		var vraag_text_pad = "Node/CanvasLayer/PanelContainer/Question_%d/Panel/GridContainer/QuestionText" % [q + 1]
		var vraag_text_edit = _root.get_node_or_null(vraag_text_pad)
		if vraag_text_edit and question_dict.has("text"):
			vraag_text_edit.text = str(question_dict["text"])

		# Question image
		var vraag_image_pad = "Node/CanvasLayer/PanelContainer/Question_%d/Panel/GridContainer/QuestionImage" % [q + 1]
		var vraag_image_rect = _root.get_node_or_null(vraag_image_pad)
		if vraag_image_rect and question_dict.has("image"):
			var image_path = str(question_dict["image"])
			vraag_image_rect.texture = load(image_path) if image_path != "" else load("res://Fotos/placeholder.png")

		# Answers
		if question_dict.has("answers"):
			var answers = question_dict["answers"]
			for a in range(min(answers.size(), 6)): # max 6 answers per question
				var answer_dict = answers[a]

				var answer_text_pad = "Node/CanvasLayer/PanelContainer/Answers_%d/Panel/VBoxContainer/AnswerText" % [q + 1]
				var answer_text_edit = _root.get_node_or_null(answer_text_pad)
				if answer_text_edit and answer_dict.has("text"):
					answer_text_edit.text = str(answer_dict["text"])

				var answer_image_pad = "Node/CanvasLayer/PanelContainer/Answers_%d/Panel/VBoxContainer/AnswerImage" % [q + 1]
				var answer_image_rect = _root.get_node_or_null(answer_image_pad)
				if answer_image_rect and answer_dict.has("image"):
					var image_path = str(answer_dict["image"])
					answer_image_rect.texture = load(image_path) if image_path != "" else load("res://Fotos/placeholder.png")

				# Options inside each answer
				if answer_dict.has("options"):
					var options = answer_dict["options"]
					for o in range(min(options.size(), 4)): # max 4 options per answer
						var option_dict = options[o]
						var base_path = "Node/CanvasLayer/PanelContainer/Answers_%d/Panel/GridContainer/Option_%d/" % [q + 1, o + 1]

						var title_edit = _root.get_node_or_null(base_path + "Title")
						if title_edit and option_dict.has("title"):
							title_edit.text = str(option_dict["title"])

						var desc_edit = _root.get_node_or_null(base_path + "Description")
						if desc_edit and option_dict.has("description"):
							desc_edit.text = str(option_dict["description"])

						var positive_edit = _root.get_node_or_null(base_path + "GridContainer/Positive")
						if positive_edit and option_dict.has("positive"):
							positive_edit.text = str(option_dict["positive"])

						var negative_edit = _root.get_node_or_null(base_path + "GridContainer/Negative")
						if negative_edit and option_dict.has("negative"):
							negative_edit.text = str(option_dict["negative"])

						for v in range(1, 5):
							var value_key = "score_panel/values/value_%d" % v
							var value_edit = _root.get_node_or_null(base_path + value_key)
							if value_edit and option_dict.has(value_key):
								value_edit.text = str(option_dict[value_key])

						for s in range(1, 5):
							var score_key = "score_panel/sliders/Score%d" % s
							var score_slider = _root.get_node_or_null(base_path + score_key)
							if score_slider and option_dict.has(score_key):
								score_slider.value = float(option_dict[score_key])
