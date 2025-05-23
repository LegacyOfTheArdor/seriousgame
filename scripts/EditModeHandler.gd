# EditModeHandler.gd
extends Node

static func apply_edit_mode(root: Node, enabled: bool) -> void:
	func process(node: Node) -> void:
		if node is LineEdit:
			node.editable = enabled
			node.focus_mode = Control.FOCUS_ALL if enabled else Control.FOCUS_NONE
			if not enabled:
				node.release_focus()
		elif node is TextEdit:
			node.editable = enabled
			node.focus_mode = Control.FOCUS_ALL if enabled else Control.FOCUS_NONE
			if not enabled:
				node.release_focus()
		elif node is Button and (
			node is ImageUploadButton or node.name == "SaveButton" or node.name == "DeleteButton"
		):
			node.visible = enabled
		elif node is CanvasItem and node.name == "slidervalues":
			node.visible = enabled

		for child in node.get_children():
			if child is Node:
				process(child)

	process(root)
