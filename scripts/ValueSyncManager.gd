class_name ValueSyncManager
extends RefCounted

var _value_edits: Dictionary = {}

func initialize(panels: Array) -> void:
	var regex = RegEx.new()
	regex.compile("^value_\\d+$")  # Regex to match "value_" followed by digits
	
	for panel in panels:
		if panel is Panel:
			_find_edits(panel, regex)

func _find_edits(node: Node, regex: RegEx) -> void:
	for child in node.get_children():
		if child is LineEdit && regex.search(child.name):
			var key = child.name
			if not _value_edits.has(key):
				_value_edits[key] = []
			
			if not _value_edits[key].has(child):
				_value_edits[key].append(child)
				child.text_changed.connect(_sync_text.bind(key, child))
		
		# Recursively search children
		_find_edits(child, regex)

func _sync_text(new_text: String, key: String, source: LineEdit) -> void:
	if not _value_edits.has(key):
		return
	
	for le in _value_edits[key]:
		if le != source && le.text != new_text:
			le.text = new_text
