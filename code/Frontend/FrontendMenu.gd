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
	$WindowRound/MenuNotice.visible = true
	$WindowRound/MenuNotice.get_child(0).grab_focus()
	MenuActive = true

func Start_Message(text : String):
	Full_AnimIn()
	process_mode = Node.PROCESS_MODE_ALWAYS
	HeaderHolder.visible = false
	FooterHolder.visible = true
	MainLabel.text = text
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
