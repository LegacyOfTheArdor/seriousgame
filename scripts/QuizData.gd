# QuizData.gd
class_name QuizData
extends RefCounted

var questions: Array = [] # Array of QuestionData
var answers: Array = []   # Array of AnswerData

class QuestionData:
	var text: String = ""
	var image: String = ""

class AnswerData:
	var text: String = ""
	var image: String = ""
	var options: Array = [] # Array of OptionData (should be 4)

class OptionData:
	var title: String = ""
	var description: String = ""
	var positive: String = ""
	var negative: String = ""
	var value_1: String = ""
	var value_2: String = ""
	var value_3: String = ""
	var value_4: String = ""
	var score1: float = 0.0
	var score2: float = 0.0
	var score3: float = 0.0
	var score4: float = 0.0

# Helper to add a question
func add_question(text: String, image: String) -> void:
	var q = QuestionData.new()
	q.text = text
	q.image = image
	questions.append(q)

# Helper to add an answer
func add_answer(text: String, image: String, options: Array) -> void:
	var a = AnswerData.new()
	a.text = text
	a.image = image
	a.options = options
	answers.append(a)
