extends Control

@export var LabelTheme : Theme
@export var LabelMaterial : Material
@onready var HeaderHolder : Control = $WindowRoundHeader
@onready var HeaderLabel : Label = $WindowRoundHeader/HeaderLabel
@onready var MainLabel : Label = $WindowMainRound/RehabLabel
@onready var FooterHolder : Control = $WindowRound
var FadeTime = 0.25
var MenuActive = false

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

func Start_PauseMenu():
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
	$WindowRound/MenuPauseExplorer.get_child($WindowRound/MenuPauseExplorer.get_child_count() - 1).grab_focus()
	MenuActive = true

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
	RehabSceneRoot.Root.ExitLevel()

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
	HeaderLabel.text = "#FE-SFXOptions"
	$WindowMainRound/MenuOptionsMain.visible = false
	$WindowMainRound/MenuOptionsSound.visible = true
	$WindowMainRound/MenuOptionsSound.get_child(0).grab_focus()

func OptionsMain_ToPause():
	HeaderLabel.text = "#FE-Paused"
	$WindowMainRound/MenuOptionsMain.visible = false
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
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
	else:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)

func OptionsGame_ToggleVibrations():
	pass

