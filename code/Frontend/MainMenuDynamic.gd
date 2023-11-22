extends Control

@onready var Root3D : Node3D = $ViewHolder/SubViewportContainer/SubViewport/FE_ROOT
@onready var CamRoot3D : Camera3D = $ViewHolder/SubViewportContainer/SubViewport/Camera3D
var ActorPath = "res://assets/frontend/dynamic/FE_Actors.tscn"
var ActorsExist : bool = false
var ActorScene : PackedScene
var ActorNode : Node3D
var AudioPath = RehabGame.AssetsPath + "Sounds/VO/Cortex_Panic_08.res"
var RequiredAssets = [
	RehabGame.AssetsPath + "Rigs/Rig_Crash.tscn",
	RehabGame.AssetsPath + "Rigs/Rig_Cortex.tscn",
	RehabGame.AssetsPath + "Rigs/RigRESET_Crash.tres",
	RehabGame.AssetsPath + "Rigs/RigRESET_Cortex.tres",
	RehabGame.AssetsPath + "Animations/Crash_SkateKickflip.res",
	RehabGame.AssetsPath + "Animations/Cortex_SkateFall.res",
]

func _ready():
	ActorsExist = true
	for i in RequiredAssets:
		if (!ResourceLoader.exists(i)):
			ActorsExist = false
			break;
	
	if (ActorsExist):
		ActorScene = ResourceLoader.load(ActorPath)
		ActorNode = ActorScene.instantiate()
		Root3D.add_child(ActorNode)

func StartAnim():
	$AnimationPlayer.play("menu_start")
	if (ActorsExist):
		ActorNode.get_node("AnimationPlayer").play("scene/menu_start")
		if (ResourceLoader.exists(AudioPath)):
			$AudioStreamPlayer.stream = load(AudioPath)
			$AudioStreamPlayer.play()

func Activate():
	$Button1.visible = false
	$Button2.visible = false
	$Button3.visible = false
	$Button4.visible = false
	$ColorRectCircle.pivot_offset = $ColorRectCircle.size / 2
	$ColorRectCircle2.pivot_offset = $ColorRectCircle2.size / 2
	$ColorRectCircle3.pivot_offset = $ColorRectCircle3.size / 2
	$ColorRectCircle4.pivot_offset = $ColorRectCircle4.size / 2
	$ColorRectCircle5.pivot_offset = $ColorRectCircle5.size / 2
	CamRoot3D.visible = true
	CamRoot3D.process_mode = Node.PROCESS_MODE_INHERIT
	Root3D.visible = true
	Root3D.process_mode = Node.PROCESS_MODE_INHERIT
	$AnimationPlayer.play("RESET")
	if (ActorsExist):
		ActorNode.get_node("AnimationPlayer").play("RESET")
	process_mode = Node.PROCESS_MODE_INHERIT
	await get_tree().process_frame
	visible = true
	StartAnim()
	RehabSceneRoot.Root.PlayMusic(54)
	await get_tree().create_timer(1.8).timeout
	$Button1.quiet = true
	$Button1.grab_focus()
	$Button1.quiet = false
	$Button1.visible = true
	$Button2.visible = true
	$Button3.visible = true
	$Button4.visible = true

func Go_LevelSelect():
	process_mode = Node.PROCESS_MODE_DISABLED
	RehabSceneRoot.Root.StartLevelSelect()
	await get_tree().create_timer(0.5).timeout
	visible = false
	CamRoot3D.visible = false
	CamRoot3D.process_mode = Node.PROCESS_MODE_DISABLED
	Root3D.visible = false
	Root3D.process_mode = Node.PROCESS_MODE_DISABLED

func Go_Options():
	RehabSceneRoot.Root.StartPauseMenu(true)

func Go_Credits():
	RehabSceneRoot.Root.PlayCredits()

func Go_QuitGame():
	get_tree().quit()
