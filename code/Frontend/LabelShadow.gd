extends Label

@onready var button : Button = get_parent()

func _ready():
	UpdateText()

func _process(_delta):
	if (button.text != text):
		UpdateText()

func UpdateText():
	text = button.text
	horizontal_alignment = button.alignment
	text_overrun_behavior = button.text_overrun_behavior
	remove_theme_font_size_override("font_size")
	add_theme_font_size_override("font_size", button.get_theme_font_size("fort_size"))
