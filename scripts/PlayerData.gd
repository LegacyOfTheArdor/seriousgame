# PlayerData.gd
extends RefCounted

class_name PlayerData

var name: String = ""
var scenario: ScenarioData = null
var answers: Array = [] # Array of PlayerAnswerData, can be pre-filled with 6 elements if desired

class ScenarioData:
	var title: String = ""
	var values: Dictionary = {} # Dictionary<String, int>

class PlayerAnswerData:
	var title: String = ""
	var values: Dictionary = {} # Dictionary<String, int>
