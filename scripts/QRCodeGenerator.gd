class_name QRCodeGenerator
extends TextureRect

func generate_qr_code(text: String) -> void:
	var qr_code_script = load("res://addons/qr_code.gd")
	if not qr_code_script:
		push_error("Could not load qr_code.gd!")
		return

	var qr_code = qr_code_script.new()
	if not qr_code:
		push_error("Could not instantiate qr_code.gd!")
		return

	# Add to tree to avoid memory leaks if it's a Node-based script
	if qr_code is Node:
		add_child(qr_code)

	# Set error correction level to LOW (0)
	qr_code.set("error_correct_level", 0)

	var texture = qr_code.call("get_texture", text)
	if not texture is Texture2D:
		push_error("Failed to generate QR code texture!")
		if qr_code is Node:
			qr_code.queue_free()
		return

	self.texture = texture
	if qr_code is Node:
		qr_code.queue_free()
	print("QR code generated and assigned.")

static func get_local_ip_address() -> String:
	for addr in IP.get_local_addresses():
		if not addr.begins_with("127."):
			return addr
	return ""
