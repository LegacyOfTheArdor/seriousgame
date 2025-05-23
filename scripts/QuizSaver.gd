# QuizSaver.gd
extends Node
const QuizData = preload("res://scripts/QuizData.gd") # Update path as needed

static func save_quiz(quiz_name: String, quiz_data: QuizData) -> void:
	var dir_path := "user://seriousgame/quizzes"
	var abs_dir_path := ProjectSettings.globalize_path(dir_path)
	if not DirAccess.dir_exists_absolute(abs_dir_path):
		DirAccess.make_dir_recursive_absolute(abs_dir_path)
	var file_path := "%s/%s.json" % [dir_path, quiz_name]
	var data = {
		"questions": [],
		"answers": []
	}
	for q in quiz_data.questions:
		data.questions.append({
			"text": q.text,
			"image": q.image
		})
	for a in quiz_data.answers:
		var options = []
		for o in a.options:
			options.append({
				"title": o.title,
				"description": o.description,
				"positive": o.positive,
				"negative": o.negative,
				"value_1": o.value_1,
				"value_2": o.value_2,
				"value_3": o.value_3,
				"value_4": o.value_4,
				"score1": o.score1,
				"score2": o.score2,
				"score3": o.score3,
				"score4": o.score4
			})
		data.answers.append({
			"text": a.text,
			"image": a.image,
			"options": options
		})
	var file = FileAccess.open(file_path, FileAccess.WRITE)
	file.store_string(JSON.stringify(data, "\t"))
	file.close()
	print("Quiz saved to %s" % file_path)
