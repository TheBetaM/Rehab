extends Control

@export var LabelTheme : Theme
@export var LabelMaterial : Material
var Labels : Array
var Paths: Array
var SelectedItem = 0
var ItemCount = 0
var Cooldown = 0.0

func Generate():
	CreateItem("", ItemCount)
	ItemCount = ItemCount + 1
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
	else:
		print("[LEVEL SELECT] Cannot open " + RehabGame.AssetsPath + "Levels/")
		$TitleLabel.text = "#FE-Explorer-ImportNotFound"
	
	SetTexture($SimpleList/Control/Button2, RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub01.res", "#FE-LevelName-001")
	SetTexture($SimpleList/Control/Button3, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level01.res", "#FE-LevelName-002")
	SetTexture($SimpleList/Control/Button4, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level02.res", "#FE-LevelName-003")
	SetTexture($SimpleList/Control/Button5, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level03.res", "#FE-LevelName-004")
	SetTexture($SimpleList/Control/Button6, RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub02.res", "#FE-LevelName-005")
	SetTexture($SimpleList/Control/Button7, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level04.res", "#FE-LevelName-006")
	SetTexture($SimpleList/Control/Button8, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level05.res", "#FE-LevelName-007")
	SetTexture($SimpleList/Control/Button9, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level06.res", "#FE-LevelName-008")
	SetTexture($SimpleList/Control/Button10, RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub03.res", "#FE-LevelName-009")
	SetTexture($SimpleList/Control/Button11, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level08.res", "#FE-LevelName-010")
	SetTexture($SimpleList/Control/Button12, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level09.res", "#FE-LevelName-011")
	SetTexture($SimpleList/Control/Button13, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level10.res", "#FE-LevelName-012")
	SetTexture($SimpleList/Control/Button14, RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub04.res", "#FE-LevelName-013")
	SetTexture($SimpleList/Control/Button15, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level11.res", "#FE-LevelName-014")
	SetTexture($SimpleList/Control/Button16, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level12.res", "#FE-LevelName-015")
	SetTexture($SimpleList/Control/Button17, RehabGame.AssetsPath + "Textures/Language/Titles/English/Level13.res", "#FE-LevelName-016")


func CreateItem(itemname, id):
	var NodeScane : PackedScene = load("res://assets/frontend/windows/RehabMenuButton.tscn")
	var NewNode = NodeScane.instantiate()
	NewNode.name = "LevelSelectItem" + str(id)
	if (id == 0):
		NewNode.text = "#FE-Back"
		NewNode.pressed.connect(func(): Adv_ToSimple())
	else:
		NewNode.text = itemname.replace("_","/").trim_suffix(".tscn")
		NewNode.pressed.connect(func(): StartLevel(itemname))
	$AdvList/VBoxContainer.add_child(NewNode)
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

func _on_VideoPlayer_finished():
	$VideoPlayer.visible = false

func Activate():
	$AdvList.visible = false
	$SimpleList.visible = false
	Hover_Clear()
	$ColorRectBG.scale = Vector2(1.0, 0.0)
	var bgTween = create_tween()
	bgTween.tween_property($ColorRectBG,"scale:y", 1.0, 0.5).set_trans(Tween.TRANS_CIRC)
	$ColorRectUpper.position.y = -160.0
	var rectUpperTween = create_tween()
	rectUpperTween.tween_property($ColorRectUpper, "position:y", 0.0, 0.5).set_trans(Tween.TRANS_CIRC).set_delay(0.5)
	$TitleLabel.position.y = -140.0
	var rectUpperTextTween = create_tween()
	rectUpperTextTween.tween_property($TitleLabel, "position:y", 0.0, 0.5).set_trans(Tween.TRANS_CIRC).set_delay(0.5)
	var rootHeight = get_parent().size.y
	$TitleLabel2.position.y = rootHeight + 5.0 #725.0 / 575.0
	var rectLowerTextTween = create_tween()
	rectLowerTextTween.tween_property($TitleLabel2, "position:y", rootHeight - 145.0, 0.5).set_trans(Tween.TRANS_CIRC).set_delay(0.5)
	$ColorRectLower.position.y = rootHeight + 120.0 #840.0 / 680.0
	var rectLowerTween = create_tween()
	rectLowerTween.tween_property($ColorRectLower, "position:y", rootHeight - 40.0, 0.5).set_trans(Tween.TRANS_CIRC).set_delay(0.5)
	RehabSceneRoot.Root.PlayMenuSound_Back()
	visible = true
	process_mode = Node.PROCESS_MODE_INHERIT
	await get_tree().create_timer(1.0).timeout
	$SimpleList.visible = true
	$SimpleList/Control/Button2.grab_focus()
	RehabSceneRoot.Root.PlayMusic(60)

func SetTexture(button : Button, path : String, backup : String):
	if (ResourceLoader.exists(path)):
		button.icon = load(path)
	else:
		button.text = backup

func Simple_ToAdvanced():
	$AdvList.visible = true
	$SimpleList.visible = false
	Labels[0].grab_focus()

func Adv_ToSimple():
	$AdvList.visible = false
	$SimpleList.visible = true
	$SimpleList/Control/Button2.grab_focus()

func StartLevelPath(path):
	if (ResourceLoader.exists(path)):
		visible = false
		process_mode = Node.PROCESS_MODE_DISABLED
		RehabSceneRoot.Root.LoadScene(path)

func Simple_GoHub01(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_hub_beach.tscn")
func Simple_GoLevel01(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_hub_huba.tscn")
func Simple_GoLevel02(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_cavern_cavent.tscn")
func Simple_GoLevel03(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_earth_docamok_docamok1.tscn")
func Simple_GoHub02(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_hub_labext.tscn")
func Simple_GoLevel04(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_iceclimb_caveent.tscn")
func Simple_GoLevel05(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_slipslide_l05start.tscn")
func Simple_GoLevel06(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_ice_highseas_gpa01.tscn")
func Simple_GoHub03(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_sch_hub_sch_hub.tscn")
func Simple_GoLevel07(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_boiler_boiler_1.tscn")
func Simple_GoLevel08(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_crash_crashent.tscn")
func Simple_GoLevel09(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_school_rooftop_roof01.tscn")
func Simple_GoHub04(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_lab_labext.tscn")
func Simple_GoLevel10(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_rockslid_l10start.tscn")
func Simple_GoLevel11(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_hub_altdoc.tscn")
func Simple_GoLevel12(): StartLevelPath(RehabGame.AssetsPath + "Levels/levels_altearth_core_corea.tscn")

func Hover_Clear(): $TitleLabel2.text = ""
func Hover_Hub01(): $TitleLabel2.text = "#FE-LevelName-001"
func Hover_Level01(): $TitleLabel2.text = "#FE-LevelName-002"
func Hover_Level02(): $TitleLabel2.text = "#FE-LevelName-003"
func Hover_Level03(): $TitleLabel2.text = "#FE-LevelName-004"
func Hover_Hub02(): $TitleLabel2.text = "#FE-LevelName-005"
func Hover_Level04(): $TitleLabel2.text = "#FE-LevelName-006"
func Hover_Level05(): $TitleLabel2.text = "#FE-LevelName-007"
func Hover_Level06(): $TitleLabel2.text = "#FE-LevelName-008"
func Hover_Hub03(): $TitleLabel2.text = "#FE-LevelName-009"
func Hover_Level07(): $TitleLabel2.text = "#FE-LevelName-010"
func Hover_Level08(): $TitleLabel2.text = "#FE-LevelName-011"
func Hover_Level09(): $TitleLabel2.text = "#FE-LevelName-012"
func Hover_Hub04(): $TitleLabel2.text = "#FE-LevelName-013"
func Hover_Level10(): $TitleLabel2.text = "#FE-LevelName-014"
func Hover_Level11(): $TitleLabel2.text = "#FE-LevelName-015"
func Hover_Level12(): $TitleLabel2.text = "#FE-LevelName-016"
