extends Control

@onready var Icon : TextureRect = $TextureRect
var IconPaths = [
	RehabGame.AssetsPath + "Textures/Language/GameOver/Crash.res", 
	RehabGame.AssetsPath + "Textures/Language/GameOver/Cortex.res",
	RehabGame.AssetsPath + "Textures/Language/GameOver/CrashAndCortex.res", 
	RehabGame.AssetsPath + "Textures/Language/GameOver/Nina.res", 
	RehabGame.AssetsPath + "Textures/Language/GameOver/Crash.res",
	RehabGame.AssetsPath + "Textures/Language/GameOver/Mecha.res"]

func Activate():
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_DISABLED
	var iconID = 0
	if (AgentCharacter.activeCharacter != null):
		iconID = AgentCharacter.activeCharacter.RegInt[0]
	if (ResourceLoader.exists(IconPaths[iconID])):
		Icon.texture = load(IconPaths[iconID])
	modulate.a = 0.0
	$Button1.quiet = true
	$Button1.grab_focus()
	var fade = create_tween()
	fade.tween_property(self, "modulate:a", 1.0, 1.0)
	visible = true
	process_mode = Node.PROCESS_MODE_ALWAYS

func Go_Continue():
	visible = false
	process_mode = Node.PROCESS_MODE_DISABLED
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.ExitLevel(false)

func Go_MainMenu():
	visible = false
	process_mode = Node.PROCESS_MODE_DISABLED
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.ExitLevel(true)
