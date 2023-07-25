extends Control

@export var LabelTheme : Theme
#@export var FE_CLICK : AudioStream
#@export var FE_BACK : AudioStream
#@export var FE_SELECT : AudioStream
var Labels : Array
var Paths: Array
var SelectedItem = 0
var ItemCount = 0
var Cooldown = 0.0
var VideoDone = false
var LevelLoading = false
@onready var LogoTex = load("res://assets/Textures/Language/Titles/English/Crash.png")
@onready var VoiceTest = load("res://assets/Sounds/GlobalVO/DRC244.wav")
@onready var MusicTest = load("res://assets/Sounds/Music/1_1_Nsanity_Island.wav")

func _ready():
	#var success = ProjectSettings.load_resource_pack("res://test.zip")
	#if (success):
	#	print("pack loaded")
	#else:
	#	print("pack failed")
	var dir = DirAccess.open("res://assets/Levels/");
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if !dir.current_is_dir():
				CreateItem(file_name, ItemCount)
				ItemCount = ItemCount + 1
				#print("Found file: " + file_name)
			file_name = dir.get_next()
		if (ItemCount == 0):
			$TitleLabel.text = "imported assets not found"
			return;
		Labels[SelectedItem].add_theme_color_override("font_color", Color.AQUAMARINE)
		Labels[SelectedItem].set_focus_mode(2)
		$VideoPlayer.visible = true
		$VideoPlayer.play()
		if (LogoTex):
			$TextureRect.texture = LogoTex
	else:
		print("An error occurred when trying to access the path.")
		$TitleLabel.text = "imported assets not found"


func CreateItem(itemname, id):
	var NewNode = Label.new()
	NewNode.text = itemname.replace("_","/").trim_suffix(".tscn")
	NewNode.theme = LabelTheme
	NewNode.gui_input.connect(OnLabelSelect.bind(Labels.size()))
	NewNode.name = "LevelSelectItem" + str(id)
	NewNode.mouse_filter = Control.MOUSE_FILTER_PASS
	$ScrollContainer/VBoxContainer.add_child(NewNode)
	Labels.append(NewNode)
	Paths.append(itemname)

func OnLabelSelect(event, id):
	if event is InputEventMouseButton and event.button_index == 1 and event.pressed and !LevelLoading and VideoDone:
		LevelLoading = true
		#$AudioStreamPlayer.stream = FE_CLICK
		#$AudioStreamPlayer.play()
		StartLevel(Paths[id])
	elif event is InputEventMouseMotion and VideoDone:
		for item in Labels:
			item.remove_theme_color_override("font_color")
			item.release_focus()
		Labels[id].add_theme_color_override("font_color", Color.AQUAMARINE)
		Labels[id].set_focus_mode(2)
		SelectedItem = id

func StartLevel(path):
	$ScrollContainer.visible = false
	$TextureRect.visible = false
	$TitleLabel.text = "loading...\n" + path.replace("_","/").trim_suffix(".tscn")
	$TitleLabel.anchor_top = 0.5
	$TitleLabel.anchor_bottom = 0.5
	await get_tree().create_timer(0.1).timeout
	get_tree().change_scene_to_file("assets/Levels/" + path)
	
func _process(delta):
	if (Cooldown > 0.0):
		Cooldown = Cooldown - delta
		return
	
	if (Input.is_action_pressed("ui_up") and VideoDone):
		SelectedItem = SelectedItem - 1
		if (SelectedItem < 0):
			SelectedItem = ItemCount - 1
		Cooldown = 0.04
		for item in Labels:
			item.remove_theme_color_override("font_color")
			item.release_focus()
		Labels[SelectedItem].add_theme_color_override("font_color", Color.AQUAMARINE)
		Labels[SelectedItem].set_focus_mode(2)
		$ScrollContainer.ensure_control_visible(Labels[SelectedItem])
		#$AudioStreamPlayer.stream = FE_SELECT
		#$AudioStreamPlayer.play()
	if (Input.is_action_pressed("ui_down") and VideoDone):
		SelectedItem = SelectedItem + 1
		if (SelectedItem > ItemCount - 1):
			SelectedItem = 0
		Cooldown = 0.04
		for item in Labels:
			item.remove_theme_color_override("font_color")
			item.release_focus()
		Labels[SelectedItem].add_theme_color_override("font_color", Color.AQUAMARINE)
		Labels[SelectedItem].set_focus_mode(2)
		$ScrollContainer.ensure_control_visible(Labels[SelectedItem])
		#$AudioStreamPlayer.stream = FE_SELECT
		#$AudioStreamPlayer.play()
	if (Input.is_action_pressed("ui_cancel") and VoiceTest != null and VideoDone):
		$AudioStreamPlayer.stream = VoiceTest
		$AudioStreamPlayer.play()
		Cooldown = 0.5
	if (Input.is_action_pressed("ui_accept")):
		if (!VideoDone):
			VideoDone = true
			$VideoPlayer.stop()
			$VideoPlayer.visible = false
			if (MusicTest != null):
				$MusicPlayer.stream = MusicTest
				$MusicPlayer.play()
			Cooldown = 0.25
		elif (!LevelLoading):
			#$AudioStreamPlayer.stream = FE_CLICK
			#$AudioStreamPlayer.play()
			LevelLoading = true
			StartLevel(Paths[SelectedItem])

func _on_VideoPlayer_finished():
	VideoDone = true
	$VideoPlayer.visible = false
	if (MusicTest != null):
		$MusicPlayer.stream = MusicTest
		$MusicPlayer.play()
