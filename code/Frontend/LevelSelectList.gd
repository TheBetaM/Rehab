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
#@onready var LogoTex = load("res://assets/Textures/Language/Titles/English/Crash.png")
#@onready var VoiceTest = load("res://assets/Sounds/GlobalVO/DRC244.wav")
#@onready var MusicTest = load("res://assets/Sounds/Music/1_1_Nsanity_Island.wav")

func _ready():
	
	#var success = ProjectSettings.load_resource_pack("res://test.zip")
	#if (success):
		#print("pack loaded")
	#else:
		#print("pack failed")
	
	#if (!DirAccess.dir_exists_absolute(RehabGame.AssetsPath + "Levels/")):
		#printerr("No Levels directory found!")
		#$TitleLabel.text = "imported assets not found"
		#return
		
	var dir = DirAccess.open(RehabGame.AssetsPath + "Levels/");
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
		if ($VideoPlayer.stream != null):
			$VideoPlayer.visible = true
			$VideoPlayer.play()
		else:
			VideoDone = true
		#if (LogoTex):
		#	$TextureRect.texture = LogoTex
	else:
		print("An error occurred when trying to access the path.")
		$TitleLabel.text = "imported assets not found"


func CreateItem(itemname, id):
	var NewNode = Button.new()
	NewNode.text = itemname.replace("_","/").trim_suffix(".tscn")
	NewNode.theme = LabelTheme
	NewNode.pressed.connect(func(): StartLevel(itemname))
	NewNode.name = "LevelSelectItem" + str(id)
	NewNode.mouse_filter = Control.MOUSE_FILTER_PASS
	$ScrollContainer/VBoxContainer.add_child(NewNode)
	if (id == 0):
		NewNode.grab_focus()
	Labels.append(NewNode)
	Paths.append(itemname)

func ResetMenu():
	Labels[0].grab_focus()

func StartLevel(path):
	visible = false
	process_mode = Node.PROCESS_MODE_DISABLED
	RehabSceneRoot.Root.LoadScene(RehabGame.AssetsPath + "Levels/" + path)

func _process(delta):
	if (Cooldown > 0.0):
		Cooldown = Cooldown - delta
		return
		
	if (Input.is_action_pressed("ui_cancel") and VideoDone):
		#$AudioStreamPlayer.stream = VoiceTest
		#$AudioStreamPlayer.play()
		Cooldown = 0.5
	if (Input.is_action_pressed("ui_accept")):
		if (!VideoDone):
			VideoDone = true
			$VideoPlayer.stop()
			$VideoPlayer.visible = false
			Cooldown = 0.25

func _on_VideoPlayer_finished():
	VideoDone = true
	$VideoPlayer.visible = false
	#if (MusicTest != null):
		#$MusicPlayer.stream = MusicTest
		#$MusicPlayer.play()
