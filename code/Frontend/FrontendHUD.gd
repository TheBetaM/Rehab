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

var WumpaTimer = 0.0
var LivesTimer = 0.0
var WumpaIconPath = RehabGame.AssetsPath + "Textures/Icons/wumpa_icon.res"
var LivesIconPaths = [RehabGame.AssetsPath + "Textures/Icons/1up-crash.res", RehabGame.AssetsPath + "Textures/Icons/1up-cortex.res",
 RehabGame.AssetsPath + "Textures/Icons/1up-coco.res", RehabGame.AssetsPath + "Textures/Icons/1up-nina.res", 
RehabGame.AssetsPath + "Textures/Icons/1up-evilcrash.res", RehabGame.AssetsPath + "Textures/Icons/1up-mechabandicoot.res"]
var WumpaHolderAnim : Tween
var LivesHolderAnim : Tween

func _ready():
	await get_tree().process_frame
	await get_tree().process_frame
	await get_tree().process_frame
	if (ResourceLoader.exists(WumpaIconPath)):
		#var iconTex = ImageTexture.new()
		#iconTex.image = load(WumpaIconPath)
		WumpaIcon.texture = load(WumpaIconPath)
	if (ResourceLoader.exists(LivesIconPaths[0])):
		#var iconTex = ImageTexture.new()
		#iconTex.image = load(LivesIconPaths[0])
		LivesIcon.texture = load(LivesIconPaths[0])

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
	iconAnim.tween_property(WumpaIcon, "scale", Vector2(0.9, 1.2), 0.1)
	iconAnim.tween_property(WumpaIcon, "scale", Vector2(0.75, 1.0), 0.1).set_delay(0.1)

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
			#var iconTex = ImageTexture.new()
			#iconTex.image = load(LivesIconPaths[iconID])
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
