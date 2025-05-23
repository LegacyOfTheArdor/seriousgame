# PanelManager.gd
class_name PanelManager
extends RefCounted

var _panels: Array = []
var _active_panel: int = 0

func _init(panels: Array) -> void:
	_panels = panels
	show_only_active_panel(0)

func show_only_active_panel(index: int) -> void:
	for i in _panels.size():
		_panels[i].visible = (i == index)
	_active_panel = index

func next_panel() -> void:
	if _active_panel < _panels.size() - 1:
		show_only_active_panel(_active_panel + 1)

func previous_panel() -> void:
	if _active_panel > 0:
		show_only_active_panel(_active_panel - 1)

func get_active_panel_index() -> int:
	return _active_panel
