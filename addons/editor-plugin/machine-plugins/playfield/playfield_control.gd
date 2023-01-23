extends Control

signal switch_active(name, state) 
var _troughBtns = []

# Called when the node enters the scene tree for the first time.
func _ready():
	for button in get_node("Buttons").get_children():
		if button.name.begins_with("trough"):
			_troughBtns.append(button)
		if(button.toggle_mode == true):
			button.pressed.connect(_on_pressed.bind(button))
		else:
			button.pressed.connect(_on_pulse_pressed.bind(button))	
	# Connect trough
	var troughBtn = get_node("AllTroughButton") as Button	
	troughBtn.pressed.connect(_on_trough_pressed)
	# Connect Reset
	var resetBtn = get_node("ResetGameButton") as Button	
	resetBtn.pressed.connect(_on_reset_pressed)
	# Connect Recordings
	var recordBtn = get_node("RecordingOptionButton") as OptionButton	
	recordBtn.item_selected.connect(_on_record_item_selected)	

func _on_pressed(button):
	print(button.name, " was pressed:", button.button_pressed)	
	var state = 0
	if button.button_pressed:
		state = 1
	emit_signal("switch_active", button.name, state)
	
func _on_pulse_pressed(button):
	print(button.name, " pulse pressed")
	emit_signal("switch_active", button.name, 2)

func _on_reset_pressed():
	var evt = InputEventAction.new()
	evt.action = "reset"
	evt.pressed = true
	Input.parse_input_event((evt))
	
func _on_record_item_selected(index):
	emit_signal("switch_active", "_record", index)
	
func _on_trough_pressed():
	for btn in _troughBtns:
		(btn as Button).set_pressed_no_signal(true);
		#(btn as Button).button_pressed=true;
		emit_signal("switch_active", (btn as Button).name, 1)
