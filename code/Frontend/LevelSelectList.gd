extends Control

@export var LabelTheme : Theme
@export var LabelMaterial : Material
var Labels : Array
var Paths: Array
var SelectedItem = 0
var ItemCount = 0
var Cooldown = 0.0
var VideoDone = false
var FE_CLICK = RehabGame.AssetsPath + "Sounds/FE_CLICK.res"
var FE_BACK = RehabGame.AssetsPath + "Sounds/FE_BACK.res"
var FE_SELECT = RehabGame.AssetsPath + "Sounds/FE_SELECT.res"
var MovieTest = RehabGame.AssetsPath + "Movies/ttident.ogv"

func _ready():
	await get_tree().process_frame
	await get_tree().process_frame
	await get_tree().process_frame
	
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
			$TitleLabel.text = "#FE-Explorer-ImportNotFound"
			return;
		if (ResourceLoader.exists(MovieTest)):
			$VideoPlayer.stream = load(MovieTest)
			$VideoPlayer.visible = true
			$VideoPlayer.play()
		else:
			VideoDone = true
			await get_tree().process_frame
			RehabSceneRoot.Root.PlayMusic(27)
	else:
		print("[LEVEL SELECT] Cannot open " + RehabGame.AssetsPath + "Levels/")
		$TitleLabel.text = "#FE-Explorer-ImportNotFound"


func CreateItem(itemname, id):
	var NewNode = Button.new()
	NewNode.text = itemname.replace("_","/").trim_suffix(".tscn")
	NewNode.theme = LabelTheme
	NewNode.material = LabelMaterial
	NewNode.pressed.connect(func(): StartLevel(itemname))
	NewNode.name = "LevelSelectItem" + str(id)
	NewNode.flat = true
	NewNode.mouse_default_cursor_shape = Control.CURSOR_POINTING_HAND
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
		
	if (Input.is_action_pressed("ui_accept")):
		if (!VideoDone):
			VideoDone = true
			$VideoPlayer.stop()
			$VideoPlayer.visible = false
			Cooldown = 0.25

func _on_VideoPlayer_finished():
	VideoDone = true
	$VideoPlayer.visible = false
	await get_tree().process_frame
	RehabSceneRoot.Root.PlayMusic(27)
