extends Control

func _ready():
	$AnimationPlayer.play("TextAnim")

func UpdateVisuals():
	$LabelLevelName.text = RehabSceneRoot.Root.LoadingChunkName.replace("_","/")

func AnimIn():
	UpdateVisuals()
	modulate.a = 1.0
	#modulate.a = 0.0
	#var mTween = create_tween();
	#mTween.tween_property(self, "modulate:a", 1.0, 0.5)
	visible = true

func AnimOut():
	modulate.a = 1.0
	var mTween = create_tween();
	mTween.tween_property(self, "modulate:a", 0.0, 0.5)
