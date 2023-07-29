extends Control

func _ready():
	$AnimationPlayer.play("TextAnim")

func UpdateVisuals():
	$LabelLevelName.text = RehabSceneRoot.Root.LoadingChunkName
