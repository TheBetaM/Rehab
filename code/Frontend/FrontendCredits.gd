extends Control

@onready var ImageRect : TextureRect = $ImageRect
@onready var LabelScene : PackedScene = load("res://assets/frontend/RehabLabel.tscn")
var ImagePath = RehabGame.AssetsPath + "Textures/Language/Credits/CreditNew.res"
var CreditsText : String
var CreditsActive : bool = false
var LineCount : int = 445

func _ready():
	if (ResourceLoader.exists(ImagePath)):
		ImageRect.texture = load(ImagePath)
	CreditsActive = false
	$VBox.position = Vector2(0, 720.0)

func _process(delta):
	if !CreditsActive:
		return
	if Input.is_action_just_pressed("ui_select"):
		CreditsActive = false;
		EndCredits()
		return
	$VBox.position += Vector2.UP * delta * 64.0
	if ($VBox.position.y < -$VBox.size.y):
		CreditsActive = false;
		EndCredits()

func StartCredits():
	var file = FileAccess.open("res://assets/lang/credits.txt", FileAccess.READ)
	#file.get_line()
	CreditsActive = false;
	#modulate.a = 1.0
	$VBox.position = Vector2(0, 720.0)
	$VBox.visible = true
	ImageRect.modulate.a = 0.0
	process_mode = Node.PROCESS_MODE_ALWAYS
	var mTween = create_tween();
	mTween.tween_property(ImageRect, "modulate:a", 1.0, 0.5)
	$VBox.position = Vector2(0, 720.0)
	visible = true
	for i in $VBox.get_children():
		i.queue_free()
	$VBox.size.y = 40 * LineCount
	await get_tree().process_frame
	
	for i in range(0, LineCount - 1):
		var label = LabelScene.instantiate()
		label.text = file.get_line()
		$VBox.add_child(label)
	
	await get_tree().process_frame
	$VBox.position = Vector2(0, 720.0)
	StartMusic()
	await get_tree().create_timer(0.5).timeout
	$VBox.visible = true
	$VBox.position = Vector2(0, 720.0)
	CreditsActive = true;

func StartMusic():
	RehabSceneRoot.Root.AudioMusic.get_parent().get_node("AudioMusic1").process_mode = Node.PROCESS_MODE_ALWAYS
	RehabSceneRoot.Root.AudioMusic.get_parent().get_node("AudioMusic2").process_mode = Node.PROCESS_MODE_ALWAYS
	RehabSceneRoot.Root.PlayMusic(58)
	await get_tree().create_timer(20.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(28)
	await get_tree().create_timer(20.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(136)
	await get_tree().create_timer(18.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(30)
	await get_tree().create_timer(20.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(35)
	await get_tree().create_timer(18.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(37)
	await get_tree().create_timer(20.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(41)
	await get_tree().create_timer(20.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(54)
	await get_tree().create_timer(18.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(60)
	await get_tree().create_timer(18.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(61)
	await get_tree().create_timer(20.0).timeout
	if (!CreditsActive): return
	RehabSceneRoot.Root.PlayMusic(27)


func EndCredits():
	#var mTween = create_tween();
	#mTween.tween_property(self, "modulate:a", 0.0, 0.5)
	#await get_tree().create_timer(0.5).timeout
	
	CreditsActive = false;
	$VBox.visible = false
	for i in $VBox.get_children():
		i.queue_free()
	process_mode = Node.PROCESS_MODE_DISABLED
	visible = false
	RehabSceneRoot.Root.AudioMusic.get_parent().get_node("AudioMusic1").process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.AudioMusic.get_parent().get_node("AudioMusic2").process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.AudioMusic.process_mode = Node.PROCESS_MODE_INHERIT
