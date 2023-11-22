extends Control

@onready var WumpaLabel : Label = $Wumpa/CountWumpa
@onready var LivesLabel : Label = $Lives/CountLives
@onready var BottomTextLabel : Label = $LabelBottom
@onready var TimerLabel : Label = $LabelTimer
@onready var CounterLabel : Label = $LabelCounter
@onready var WumpaHolder : Control = $Wumpa
@onready var LivesHolder : Control = $Lives
@onready var WumpaIcon : TextureRect = $Wumpa/IconWumpa
@onready var LivesIcon : TextureRect = $Lives/IconLives
@onready var GemIcon : TextureRect = $Gem/IconGem
@onready var CrystalIcon : TextureRect = $Crystal/IconCrystal

var WumpaTimer = 0.0
var LivesTimer = 0.0
var WumpaIconPath = RehabGame.AssetsPath + "Textures/Icons/wumpa_icon.res"
var LivesIconPaths = [RehabGame.AssetsPath + "Textures/Icons/1up-crash.res", RehabGame.AssetsPath + "Textures/Icons/1up-cortex.res",
 RehabGame.AssetsPath + "Textures/Icons/1up-coco.res", RehabGame.AssetsPath + "Textures/Icons/1up-nina.res", 
RehabGame.AssetsPath + "Textures/Icons/1up-evilcrash.res", RehabGame.AssetsPath + "Textures/Icons/1up-mechabandicoot.res"]
var WumpaHolderAnim : Tween
var LivesHolderAnim : Tween
var CrystalIconPath = RehabGame.AssetsPath + "Textures/Icons/Crystal_Single.res"
var GemIconPaths = [
	RehabGame.AssetsPath + "Textures/Icons/gem-blue.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-clear.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-green.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-purple.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-red.res",
	RehabGame.AssetsPath + "Textures/Icons/gem-yellow.res",
]

func _ready():
	await get_tree().process_frame
	await get_tree().process_frame
	await get_tree().process_frame
	await get_tree().process_frame
	await get_tree().process_frame
	if (ResourceLoader.exists(WumpaIconPath)):
		WumpaIcon.texture = load(WumpaIconPath)
	if (ResourceLoader.exists(LivesIconPaths[0])):
		LivesIcon.texture = load(LivesIconPaths[0])
	if (ResourceLoader.exists(CrystalIconPath)):
		CrystalIcon.texture = load(CrystalIconPath)
	if (ResourceLoader.exists(GemIconPaths[0])):
		GemIcon.texture = load(GemIconPaths[0])

func _process(delta):
	if (WumpaTimer > 0.0):
		WumpaTimer -= delta
		if (WumpaTimer <= 0.0):
			if (WumpaHolderAnim != null):
				WumpaHolderAnim.kill()
			WumpaHolderAnim = create_tween()
			WumpaHolderAnim.tween_property(WumpaHolder,"position:x", -300.0, 0.25)
			WumpaHolderAnim.tween_callback(func(): WumpaHolder.visible = false)
	
	if (LivesTimer > 0.0):
		LivesTimer -= delta
		if (LivesTimer <= 0.0):
			if (LivesHolderAnim != null):
				LivesHolderAnim.kill()
			LivesHolderAnim = create_tween()
			LivesHolderAnim.tween_property(LivesHolder,"position:x", size.x - 20.0, 0.25)
			LivesHolderAnim.tween_callback(func(): LivesHolder.visible = false)

func UpdateWumpa():
	WumpaLabel.text = str(RehabSceneRoot.Root.Game.Fruit)
	if (WumpaHolder.visible):
		WumpaTimer = 5.0
		return
	WumpaHolder.position.x = -300.0
	WumpaHolder.visible = true
	WumpaTimer = 5.0
	if (WumpaHolderAnim != null):
		WumpaHolderAnim.kill()
	WumpaHolderAnim = create_tween()
	WumpaHolderAnim.tween_property(WumpaHolder,"position:x", 0.0, 0.25)

func ForceAnimOut():
	WumpaTimer = 0.01
	LivesTimer = 0.01

func AnimateWumpa():
	var iconAnim = create_tween()
	iconAnim.tween_property(WumpaIcon, "scale", Vector2(0.9, 1.2), 0.05)
	iconAnim.tween_property(WumpaIcon, "scale", Vector2(0.75, 1.0), 0.05).set_delay(0.05)

func UpdateLives():
	LivesLabel.text = str(RehabSceneRoot.Root.Game.Lives)
	if (LivesHolder.visible):
		LivesTimer = 5.0
		return
	if (!LivesHolder.visible):
		var iconID = 0
		if (AgentCharacter.activeCharacter != null):
			iconID = AgentCharacter.activeCharacter.RegInt[0]
		if (ResourceLoader.exists(LivesIconPaths[iconID])):
			LivesIcon.texture = load(LivesIconPaths[iconID])
	LivesHolder.position.x = size.x - 20.0
	LivesHolder.visible = true
	LivesTimer = 5.0
	if (LivesHolderAnim != null):
		LivesHolderAnim.kill()
	LivesHolderAnim = create_tween()
	LivesHolderAnim.tween_property(LivesHolder,"position:x", size.x - 400.0, 0.25)

func AnimateLife():
	$AnimationPlayer.play("LifeIconANim")

func AnimateGem(gem : int):
	if (ResourceLoader.exists(GemIconPaths[gem])):
		GemIcon.texture = load(GemIconPaths[gem])
	
	GemIcon.scale = Vector2(0.0, 0.0)
	GemIcon.modulate.a = 0.0
	GemIcon.get_parent().visible = true
	var gemTween1 = create_tween()
	var gemTween2 = create_tween()
	gemTween1.tween_property(GemIcon, "scale", Vector2(1.0, 1.0), 0.5)
	gemTween2.tween_property(GemIcon, "modulate:a", 1.0, 0.5)
	await get_tree().create_timer(3.0).timeout
	gemTween1 = create_tween()
	gemTween2 = create_tween()
	gemTween1.tween_property(GemIcon, "scale", Vector2(0.0, 0.0), 0.5)
	gemTween2.tween_property(GemIcon, "modulate:a", 0.0, 0.5)
	await get_tree().create_timer(0.5).timeout
	GemIcon.get_parent().visible = false

func AnimateCrystal():
	CrystalIcon.scale = Vector2(0.0, 0.0)
	CrystalIcon.modulate.a = 0.0
	CrystalIcon.get_parent().visible = true
	var gemTween1 = create_tween()
	var gemTween2 = create_tween()
	gemTween1.tween_property(CrystalIcon, "scale", Vector2(1.0, 1.0), 0.5)
	gemTween2.tween_property(CrystalIcon, "modulate:a", 1.0, 0.5)
	await get_tree().create_timer(3.0).timeout
	gemTween1 = create_tween()
	gemTween2 = create_tween()
	gemTween1.tween_property(CrystalIcon, "scale", Vector2(0.0, 0.0), 0.5)
	gemTween2.tween_property(CrystalIcon, "modulate:a", 0.0, 0.5)
	await get_tree().create_timer(0.5).timeout
	CrystalIcon.get_parent().visible = false

func UpdateAll():
	WumpaLabel.text = str(RehabSceneRoot.Root.Game.Fruit)
	LivesLabel.text = str(RehabSceneRoot.Root.Game.Lives)

func Clear():
	TimerLabel.visible = false
	CounterLabel.visible = false
	BottomTextLabel.visible = false
	WumpaHolder.visible = false
	LivesHolder.visible = false
	LivesTimer = 0.0
	WumpaTimer = 0.0
	UpdateAll()
