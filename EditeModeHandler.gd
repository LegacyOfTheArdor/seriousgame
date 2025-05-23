# EditModeHandler.gd
class_name EditModeHandler

static func apply_edit_mode(root: Node, enabled: bool = false) -> void:
	# Always treat as NOT in edit mode (enabled = false)
	_process_node(root, false)

static func _process_node(node: Node, enabled: bool) -> void:
	# LineEdit
	if node is LineEdit:
		node.editable = enabled
		node.focus_mode = Control.FOCUS_ALL if enabled else Control.FOCUS_NONE
		if not enabled:
			node.release_focus()
	# TextEdit
	elif node is TextEdit:
		node.editable = enabled
		node.focus_mode = Control.FOCUS_ALL if enabled else Control.FOCUS_NONE
		if not enabled:
			node.release_focus()
	# Button (ImageUploadButton or Save/Delete)
	elif node is Button:
		if node.name == "imagebutton" or node.name == "SaveButton" or node.name == "DeleteButton":
			node.visible = enabled
	# CanvasItem named "slidervalues"
	elif node is CanvasItem and node.name == "slidervalues":
		node.visible = enabled

	# Recurse
	for child in node.get_children():
		if child is Node:
			_process_node(child, enabled)
