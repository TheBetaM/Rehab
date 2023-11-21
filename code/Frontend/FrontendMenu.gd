extends Control

@export var LabelTheme : Theme
@export var LabelMaterial : Material
@onready var HeaderHolder : Control = $WindowRoundHeader
@onready var HeaderLabel : Label = $WindowRoundHeader/HeaderLabel
@onready var MainLabel : Label = $WindowMainRound/RehabLabel
@onready var FooterHolder : Control = $WindowRound
var FadeTime = 0.25
var MenuActive = false
var OptionsOnly = false

func Full_AnimIn():
	visible = false
	pivot_offset = get_window().size / 2
	scale = Vector2.ZERO
	modulate.a = 0.0
	visible = true
	var anim = create_tween()
	anim.tween_property(self, "scale", Vector2.ONE, FadeTime)
	var anim1 = create_tween() # tweening both with one somehow bugged
	anim1.tween_property(self, "modulate:a", 1.0, FadeTime)

func Full_AnimOut():
	MenuActive = false
	pivot_offset = get_window().size / 2
	scale = Vector2.ONE
	modulate.a = 1.0
	var anim = create_tween()
	anim.tween_property(self, "scale", Vector2.ZERO, FadeTime)
	var anim1 = create_tween()
	anim1.tween_property(self, "modulate:a", 0.0, FadeTime)
	anim.tween_callback(AnimOutEnd)

func AnimOutEnd():
	visible = false

func Start_PauseMenu(optOnly : bool):
	OptionsOnly = optOnly
	Full_AnimIn()
	process_mode = Node.PROCESS_MODE_ALWAYS
	HeaderHolder.visible = true
	FooterHolder.visible = true
	HeaderLabel.text = "#FE-Paused"
	MainLabel.text = ""
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
	$WindowRound.visible = false
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
		RehabSceneRoot.Root.get_node("FE/FE_MainMenuDynamic/Button2").grab_focus()
		return;
	HeaderLabel.text = "#FE-Paused"
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
