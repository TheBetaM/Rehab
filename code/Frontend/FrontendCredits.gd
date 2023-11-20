extends Control

@onready var ImageRect : TextureRect = $ImageRect
@onready var CreditsLabel : Label = $CreditsLabel
var ImagePath = RehabGame.AssetsPath + "Textures/Language/Credits/CreditNew.res"
var CreditsText : String
var CreditsActive : bool = false
var CreditsLoaded : bool = false

func _ready():
	if (ResourceLoader.exists(ImagePath)):
		ImageRect.texture = load(ImagePath)
	var file = FileAccess.open("res://assets/lang/credits.txt", FileAccess.READ)
	CreditsActive = false
	CreditsText = file.get_as_text()
	CreditsLabel.position = Vector2(0, 720.0)

func _process(delta):
	if !CreditsActive:
		return
	if Input.is_action_just_pressed("ui_select"):
		CreditsActive = false;
		EndCredits()
		return
	CreditsLabel.position += Vector2.UP * delta * 64.0
	if (CreditsLabel.position.y < -CreditsLabel.size.y):
		CreditsActive = false;
		EndCredits()

func StartCredits():
	CreditsActive = false;
	#modulate.a = 1.0
	CreditsLabel.position = Vector2(0, 720.0)
	CreditsLabel.visible = true
	ImageRect.modulate.a = 0.0
	process_mode = Node.PROCESS_MODE_ALWAYS
	var mTween = create_tween();
	mTween.tween_property(ImageRect, "modulate:a", 1.0, 0.5)
	CreditsLabel.position = Vector2(0, 720.0)
	visible = true
	if (CreditsLoaded):
		CreditsActive = true;
		StartMusic()
		return
	await get_tree().process_frame
	CreditsLabel.text = CreditsText
	await get_tree().process_frame
	CreditsLabel.position = Vector2(0, 720.0)
	await get_tree().create_timer(0.5).timeout
	CreditsLabel.visible = true
	CreditsActive = true;
	CreditsLoaded = true;
	StartMusic()

func StartMusic():
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
	CreditsLabel.visible = false
	process_mode = Node.PROCESS_MODE_DISABLED
	visible = false
	RehabSceneRoot.Root.process_mode = Node.PROCESS_MODE_INHERIT
	RehabSceneRoot.Root.AudioMusic.process_mode = Node.PROCESS_MODE_INHERIT
