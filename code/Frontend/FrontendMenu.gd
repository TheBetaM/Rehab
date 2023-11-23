extends Control

@export var LabelTheme : Theme
@export var LabelMaterial : Material
@onready var HeaderHolder : Control = $WindowRoundHeader
@onready var HeaderLabel : Label = $WindowRoundHeader/HeaderLabel
@onready var MainLabel : Label = $WindowMainRound/RehabLabel
@onready var FooterHolder : Control = $WindowRound
@onready var LevelIcon : TextureRect = $WindowMainRound/LevelIcon
@onready var WumpaIcon : TextureRect = $WindowRoundWumpa/WumpaIcon
@onready var LivesIcon : TextureRect = $WindowRoundLives/LivesIcon
@onready var CrystalsIcon : TextureRect = $WindowRoundCrystals/CrystalsIcon
@onready var WumpaText : Label = $WindowRoundWumpa/CountWumpa
@onready var LivesText : Label = $WindowRoundLives/CountLives
@onready var CrystalsText : Label = $WindowRoundCrystals/CountCrystals
@onready var GemIcon1 : TextureRect = $WindowGems/GemIcon1
@onready var GemIcon2 : TextureRect = $WindowGems/GemIcon2
@onready var GemIcon3 : TextureRect = $WindowGems/GemIcon3
@onready var GemIcon4 : TextureRect = $WindowGems/GemIcon4
@onready var GemIcon5 : TextureRect = $WindowGems/GemIcon5
@onready var GemIcon6 : TextureRect = $WindowGems/GemIcon6
var FadeTime = 0.25
var MenuActive = false
var OptionsOnly = false
var IconsInit = false
var CrystalIconPath = RehabGame.AssetsPath + "Textures/Icons/crystal_icon.res"
var WumpaIconPath = RehabGame.AssetsPath + "Textures/Icons/wumpa_icon.res"
var LivesIconPaths = [RehabGame.AssetsPath + "Textures/Icons/1up-crash.res", RehabGame.AssetsPath + "Textures/Icons/1up-cortex.res",
 RehabGame.AssetsPath + "Textures/Icons/1up-coco.res", RehabGame.AssetsPath + "Textures/Icons/1up-nina.res", 
RehabGame.AssetsPath + "Textures/Icons/1up-evilcrash.res", RehabGame.AssetsPath + "Textures/Icons/1up-mechabandicoot.res"]
var GemIconPaths = [
	RehabGame.AssetsPath + "Textures/Icons/gem-blue.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-clear.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-green.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-purple.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-red.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-yellow.res",
]
var EmptyGemIconPath = RehabGame.AssetsPath + "Textures/Icons/gem_greyed.res"
var BaseScale = 0.9

func Full_AnimIn():
	visible = false
	pivot_offset = get_window().size / 2
	scale = Vector2.ZERO
	modulate.a = 0.0
	visible = true
	var TargetScale = (get_window().size.y / 720.0) * Vector2.ONE * BaseScale
	var anim = create_tween()
	anim.tween_property(self, "scale", TargetScale, FadeTime)
	var anim1 = create_tween() # tweening both with one somehow bugged
	anim1.tween_property(self, "modulate:a", 1.0, FadeTime)

func Full_AnimOut():
	MenuActive = false
	pivot_offset = get_window().size / 2
	scale = (get_window().size.y / 720.0) * Vector2.ONE * BaseScale
	modulate.a = 1.0
	var anim = create_tween()
	anim.tween_property(self, "scale", Vector2.ZERO, FadeTime)
	var anim1 = create_tween()
	anim1.tween_property(self, "modulate:a", 0.0, FadeTime)
	anim.tween_callback(AnimOutEnd)

func AnimOutEnd():
	visible = false

func InitIcons():
	if (ResourceLoader.exists(WumpaIconPath)):
		WumpaIcon.texture = load(WumpaIconPath)
	if (ResourceLoader.exists(LivesIconPaths[0])):
		LivesIcon.texture = load(LivesIconPaths[0])
	if (ResourceLoader.exists(CrystalIconPath)):
		CrystalsIcon.texture = load(CrystalIconPath)

func Start_PauseMenu(optOnly : bool):
	OptionsOnly = optOnly
	Full_AnimIn()
	process_mode = Node.PROCESS_MODE_ALWAYS
	HeaderHolder.visible = false
	FooterHolder.visible = true
	HeaderLabel.text = "#FE-Paused"
	MainLabel.text = ""
	WumpaText.text = str(RehabSceneRoot.Game.Fruit)
	LivesText.text = str(RehabSceneRoot.Game.Crystals)
	CrystalsText.text = str(RehabSceneRoot.Game.Crystals)
	$WindowRoundWumpa.visible = true
	$WindowRoundLives.visible = true
	$WindowRoundCrystals.visible = true
	$WindowGems.visible = true
	if (!IconsInit):
		InitIcons()
	SetLevelIcon()
	UpdateLives()
	LevelIcon.visible = true
	for i in $WindowRound.get_children():
		if (i is VBoxContainer):
			i.visible = false
	$WindowRound/MenuPauseExplorer.visible = true
	MenuActive = true
	if (optOnly):
		Pause_ToOptions()
	else:
		$WindowRound/MenuPauseExplorer.get_child($WindowRound/MenuPauseExplorer.get_child_count() - 1).grab_focus()

func Start_Message(text : String):
	Full_AnimIn()
	process_mode = Node.PROCESS_MODE_ALWAYS
	HeaderHolder.visible = false
	FooterHolder.visible = true
	MainLabel.text = text
	LevelIcon.visible = false
	$WindowRoundWumpa.visible = false
	$WindowRoundLives.visible = false
	$WindowRoundCrystals.visible = false
	$WindowGems.visible = false
	for i in $WindowRound.get_children():
		if (i is VBoxContainer):
			i.visible = false
	$WindowRound/MenuNotice.visible = true
	$WindowRound/MenuNotice.get_child(0).grab_focus()
	MenuActive = true

func Notice_Close():
	if (!MenuActive):
		return
	Full_AnimOut()
	await get_tree().create_timer(FadeTime).timeout
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	process_mode = Node.PROCESS_MODE_INHERIT

func Pause_Resume():
	Notice_Close()

func Pause_QuitGame():
	get_tree().quit()

func Pause_ReturnToLevelSelect():
	MenuActive = false
	visible = false
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.ExitLevel(false)

func Pause_ReturnToMainMenu():
	MenuActive = false
	visible = false
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.ExitLevel(true)

func Pause_ToOptions():
	HeaderLabel.text = "#FE-Options"
	HeaderHolder.visible = true
	LevelIcon.visible = false
	$WindowRoundWumpa.visible = false
	$WindowRoundLives.visible = false
	$WindowRoundCrystals.visible = false
	$WindowRound.visible = false
	$WindowGems.visible = false
	$WindowRound/MenuPauseExplorer.visible = false
	$WindowMainRound/MenuOptionsMain.visible = true
	$WindowMainRound/MenuOptionsMain.get_child(0).grab_focus()

func OptionsMain_ToGraphics():
	HeaderLabel.text = "#FE-GFXOptions"
	$WindowMainRound/MenuOptionsMain.visible = false
	$WindowMainRound/MenuOptionsGraphics.visible = true
	$WindowMainRound/MenuOptionsGraphics.get_child(0).grab_focus()

func OptionsMain_ToGame():
	HeaderLabel.text = "#FE-GameOptions"
	if (RehabGame.InvertCameraX):
		$WindowMainRound/MenuOptionsGame/Button1.text = "#FE-CamInvertH-On"
	else:
		$WindowMainRound/MenuOptionsGame/Button1.text = "#FE-CamInvertH-Off"
	if (RehabGame.InvertCameraY):
		$WindowMainRound/MenuOptionsGame/Button3.text = "#FE-CamInvertV-On"
	else:
		$WindowMainRound/MenuOptionsGame/Button3.text = "#FE-CamInvertV-Off"
	$WindowMainRound/MenuOptionsGame/Button4.text = tr("#FE-Language") + ": " + tr("#FE-LanguageName")
	$WindowMainRound/MenuOptionsMain.visible = false
	$WindowMainRound/MenuOptionsGame.visible = true
	$WindowMainRound/MenuOptionsGame.get_child(0).grab_focus()

func OptionsMain_ToSound():
	OptionsSound_ToggleVolume(0, true)
	OptionsSound_ToggleVolume(1, true)
	OptionsSound_ToggleVolume(2, true)
	OptionsSound_ToggleVolume(4, true)
	HeaderLabel.text = "#FE-SFXOptions"
	$WindowMainRound/MenuOptionsMain.visible = false
	$WindowMainRound/MenuOptionsSound.visible = true
	$WindowMainRound/MenuOptionsSound.get_child(0).grab_focus()

func OptionsMain_ToPause():
	$WindowMainRound/MenuOptionsMain.visible = false
	if (OptionsOnly):
		Pause_Resume()
		RehabSceneRoot.Root.get_node("FE/FE_MainMenuDynamic").ReturnOptions()
		return;
	HeaderLabel.text = "#FE-Paused"
	HeaderHolder.visible = false
	LevelIcon.visible = true
	$WindowRoundWumpa.visible = true
	$WindowRoundLives.visible = true
	$WindowRoundCrystals.visible = true
	$WindowGems.visible = true
	$WindowRound.visible = true
	$WindowRound/MenuPauseExplorer.visible = true
	$WindowRound/MenuPauseExplorer.get_child(0).grab_focus()

func OptionsGame_ToMain():
	HeaderLabel.text = "#FE-Options"
	$WindowMainRound/MenuOptionsGame.visible = false
	$WindowMainRound/MenuOptionsMain.visible = true
	$WindowMainRound/MenuOptionsMain.get_child(0).grab_focus()

func OptionsGraphics_ToMain():
	HeaderLabel.text = "#FE-Options"
	$WindowMainRound/MenuOptionsGraphics.visible = false
	$WindowMainRound/MenuOptionsMain.visible = true
	$WindowMainRound/MenuOptionsMain.get_child(0).grab_focus()

func OptionsSound_ToMain():
	HeaderLabel.text = "#FE-Options"
	$WindowMainRound/MenuOptionsSound.visible = false
	$WindowMainRound/MenuOptionsMain.visible = true
	$WindowMainRound/MenuOptionsMain.get_child(0).grab_focus()

func OptionsGraphics_ToggleFullscreen():
	if (DisplayServer.window_get_mode() == DisplayServer.WINDOW_MODE_WINDOWED):
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
	pivot_offset = get_window().size / 2
	scale = (get_window().size.y / 720.0) * Vector2.ONE * BaseScale

func OptionsGame_ToggleVibrations():
	pass

func OptionsGraphics_ToggleMSAA():
	var set = get_viewport().msaa_3d
	if (set == Viewport.MSAA_DISABLED):
		get_viewport().msaa_3d = Viewport.MSAA_2X
		$WindowMainRound/MenuOptionsGraphics/Button2.text = "#FE-MSAA-2x"
	elif (set == Viewport.MSAA_2X):
		get_viewport().msaa_3d = Viewport.MSAA_4X
		$WindowMainRound/MenuOptionsGraphics/Button2.text = "#FE-MSAA-4x"
	elif (set == Viewport.MSAA_4X):
		get_viewport().msaa_3d = Viewport.MSAA_8X
		$WindowMainRound/MenuOptionsGraphics/Button2.text = "#FE-MSAA-8x"
	else:
		get_viewport().msaa_3d = Viewport.MSAA_DISABLED
		$WindowMainRound/MenuOptionsGraphics/Button2.text = "#FE-MSAA-Off"

func OptionsGraphics_ToggleTXAA():
	var set = get_viewport().screen_space_aa
	if (set == Viewport.SCREEN_SPACE_AA_DISABLED):
		get_viewport().screen_space_aa = Viewport.SCREEN_SPACE_AA_FXAA
		$WindowMainRound/MenuOptionsGraphics/Button4.text = "#FE-FXAA-On"
	else:
		get_viewport().screen_space_aa = Viewport.SCREEN_SPACE_AA_DISABLED
		$WindowMainRound/MenuOptionsGraphics/Button4.text = "#FE-FXAA-Off"

func OptionsGraphics_ToggleVSync():
	var set = DisplayServer.window_get_vsync_mode()
	if (set == DisplayServer.VSYNC_DISABLED):
		DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_ENABLED)
		$WindowMainRound/MenuOptionsGraphics/Button3.text = "#FE-VSync-On"
	elif (set == DisplayServer.VSYNC_ENABLED):
		DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_ADAPTIVE)
		$WindowMainRound/MenuOptionsGraphics/Button3.text = "#FE-VSync-Adaptive"
	elif (set == DisplayServer.VSYNC_ADAPTIVE):
		DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_MAILBOX)
		$WindowMainRound/MenuOptionsGraphics/Button3.text = "#FE-VSync-Fast"
	else:
		DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_DISABLED)
		$WindowMainRound/MenuOptionsGraphics/Button3.text = "#FE-VSync-Off"

func OptionsSound_ToggleVolume_Global():
	OptionsSound_ToggleVolume(0, false)

func OptionsSound_ToggleVolume_Music():
	OptionsSound_ToggleVolume(1, false)

func OptionsSound_ToggleVolume_SFX():
	OptionsSound_ToggleVolume(2, false)
	OptionsSound_ToggleVolume(3, false)

func OptionsSound_ToggleVolume_Voice():
	OptionsSound_ToggleVolume(4, false)

func OptionsGame_ToggleCameraH():
	RehabGame.InvertCameraX = !RehabGame.InvertCameraX
	if (RehabGame.InvertCameraX):
		$WindowMainRound/MenuOptionsGame/Button1.text = "#FE-CamInvertH-On"
	else:
		$WindowMainRound/MenuOptionsGame/Button1.text = "#FE-CamInvertH-Off"

func OptionsGame_ToggleCameraV():
	RehabGame.InvertCameraY = !RehabGame.InvertCameraY
	if (RehabGame.InvertCameraY):
		$WindowMainRound/MenuOptionsGame/Button3.text = "#FE-CamInvertV-On"
	else:
		$WindowMainRound/MenuOptionsGame/Button3.text = "#FE-CamInvertV-Off"

func OptionsGame_ToggleLanguage():
	var myloc = TranslationServer.get_locale()
	var loc = TranslationServer.get_loaded_locales()
	var dict = []
	for i in loc:
		if !dict.has(i):
			dict.append(i)
	var iter = dict.find(myloc)
	if (iter >= dict.size() - 1):
		iter = 0
	else:
		iter = iter + 1
	TranslationServer.set_locale(dict[iter])
	$WindowMainRound/MenuOptionsGame/Button4.text = tr("#FE-Language") + ": " + tr("#FE-LanguageName")

func OptionsSound_ToggleVolume(busID : int, textOnly : bool):
	var vol = AudioServer.get_bus_volume_db(busID)
	var muted = AudioServer.is_bus_mute(busID)
	var targetText : String = ""
	var targetVol = 0.0
	
	if (textOnly):
		vol += 2.6
	
	match busID:
		0: targetText += tr("#FE-GlobalVolume") + ": "
		1: targetText += tr("#FE-MusicVolume") + ": "
		2, 3: targetText += tr("#FE-EffectsVolume") + ": "
		4: targetText += tr("#FE-VoiceVolume") + ": "
	
	if (muted):
		if (!textOnly):
			AudioServer.set_bus_volume_db(busID, 0.0)
			AudioServer.set_bus_mute(busID, false)
			targetText += "100%"
		else:
			targetText += "0%"
		match busID:
			0: $WindowMainRound/MenuOptionsSound/Button1.text = targetText
			1: $WindowMainRound/MenuOptionsSound/Button3.text = targetText
			2, 3: $WindowMainRound/MenuOptionsSound/Button2.text = targetText
			4: $WindowMainRound/MenuOptionsSound/Button4.text = targetText
		return;
	
	if (vol >= 2.5):
		targetVol = -2.5
		targetText += "100%"
	elif (vol >= 0.0):
		targetVol = -2.5
		targetText += "90%"
	elif (vol >= -2.6):
		targetVol = -5.0
		targetText += "80%"
	elif (vol >= -5.1):
		targetVol = -7.5
		targetText += "70%"
	elif (vol >= -7.6):
		targetVol = -10.0
		targetText += "60%"
	elif (vol >= -10.1):
		targetVol = -12.5
		targetText += "50%"
	elif (vol >= -12.6):
		targetVol = -15.0
		targetText += "40%"
	elif (vol >= -15.1):
		targetVol = -17.5
		targetText += "30%"
	elif (vol >= -17.6):
		targetVol = -20.0
		targetText += "20%"
	elif (vol >= -20.1):
		targetVol = -22.5
		targetText += "10%"
	else:
		if (!textOnly):
			AudioServer.set_bus_mute(busID, true)
		targetText += "0%"
	
	if (!textOnly):
		AudioServer.set_bus_volume_db(busID, targetVol)
	
	match busID:
		0: $WindowMainRound/MenuOptionsSound/Button1.text = targetText
		1: $WindowMainRound/MenuOptionsSound/Button3.text = targetText
		2, 3: $WindowMainRound/MenuOptionsSound/Button2.text = targetText
		4: $WindowMainRound/MenuOptionsSound/Button4.text = targetText

func SetLevelIcon():
	var LevelID = RehabSceneRoot.Game.LevelID
	var IconPath = ""
	
	match LevelID:
		0: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub01.res"
		1: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level01.res"
		3: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level02.res"
		4: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level03.res"
		6: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub02.res"
		7: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level04.res"
		9: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level05.res"
		10: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level06.res"
		13: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub03.res"
		15: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level08.res"
		17: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level09.res"
		18: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level10.res"
		20: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Hub04.res"
		21: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level11.res"
		22: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level12.res"
		23: IconPath = RehabGame.AssetsPath + "Textures/Language/Titles/English/Level13.res"
		_: pass
	
	if (IconPath != "" and ResourceLoader.exists(IconPath)):
		LevelIcon.texture = load(IconPath)
	else:
		LevelIcon.texture = null

func UpdateLives():
	var iconID = 0
	if (AgentCharacter.activeCharacter != null):
		iconID = AgentCharacter.activeCharacter.RegInt[0]
	if (ResourceLoader.exists(LivesIconPaths[iconID])):
		LivesIcon.texture = load(LivesIconPaths[iconID])
	
	if (ResourceLoader.exists(EmptyGemIconPath)):
		GemIcon1.texture = load(EmptyGemIconPath)
		GemIcon2.texture = load(EmptyGemIconPath)
		GemIcon3.texture = load(EmptyGemIconPath)
		GemIcon4.texture = load(EmptyGemIconPath)
		GemIcon5.texture = load(EmptyGemIconPath)
		GemIcon6.texture = load(EmptyGemIconPath)
	if (RehabSceneRoot.Game.Gems.size() != 0):
		if (RehabSceneRoot.Game.Gems.has(RehabSceneRoot.Game.LevelID)):
			if (RehabSceneRoot.Game.Gems[RehabSceneRoot.Game.LevelID].has(0) and ResourceLoader.exists(GemIconPaths[0])):
				GemIcon1.texture = load(GemIconPaths[0])
			if (RehabSceneRoot.Game.Gems[RehabSceneRoot.Game.LevelID].has(1) and ResourceLoader.exists(GemIconPaths[1])):
				GemIcon2.texture = load(GemIconPaths[1])
			if (RehabSceneRoot.Game.Gems[RehabSceneRoot.Game.LevelID].has(2) and ResourceLoader.exists(GemIconPaths[2])):
				GemIcon3.texture = load(GemIconPaths[2])
			if (RehabSceneRoot.Game.Gems[RehabSceneRoot.Game.LevelID].has(3) and ResourceLoader.exists(GemIconPaths[3])):
				GemIcon4.texture = load(GemIconPaths[3])
			if (RehabSceneRoot.Game.Gems[RehabSceneRoot.Game.LevelID].has(4) and ResourceLoader.exists(GemIconPaths[4])):
				GemIcon5.texture = load(GemIconPaths[4])
			if (RehabSceneRoot.Game.Gems[RehabSceneRoot.Game.LevelID].has(5) and ResourceLoader.exists(GemIconPaths[5])):
				GemIcon6.texture = load(GemIconPaths[5])

