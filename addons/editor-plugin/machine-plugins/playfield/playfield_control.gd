extends Control

signal switch_active(name, state)

# Called when the node enters the scene tree for the first time.
func _ready():
	for button in get_node("Buttons").get_children():
		if(button.toggle_mode == true):
			button.pressed.connect(_on_pressed.bind(button))
		else:
			button.pressed.connect(_on_pulse_pressed.bind(button))

func _on_pressed(button):
	print(button.name, " was pressed:", button.button_pressed)	
	var state = 0
	if button.button_pressed:
		state = 1
	emit_signal("switch_active", button.name, state)
	
func _on_pulse_pressed(button):
	print(button.name, " pulse pressed")
	emit_signal("switch_active", button.name, 2)
