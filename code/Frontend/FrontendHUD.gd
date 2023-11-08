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
var WumpaIconPath = RehabGame.AssetsPath + "Textures/Icons/wumpa_icon.tres"
var LivesIconPaths = [RehabGame.AssetsPath + "Textures/Icons/1up-crash.tres", RehabGame.AssetsPath + "Textures/Icons/1up-cortex.tres",
 RehabGame.AssetsPath + "Textures/Icons/1up-coco.tres", RehabGame.AssetsPath + "Textures/Icons/1up-nina.tres", 
RehabGame.AssetsPath + "Textures/Icons/1up-evilcrash.tres", RehabGame.AssetsPath + "Textures/Icons/1up-mechabandicoot.tres"]

func _ready():
	if (ResourceLoader.exists(WumpaIconPath)):
		var iconTex = ImageTexture.new()
		iconTex.image = load(WumpaIconPath)
		WumpaIcon.texture = iconTex
	if (ResourceLoader.exists(LivesIconPaths[0])):
		var iconTex = ImageTexture.new()
		iconTex.image = load(LivesIconPaths[0])
		LivesIcon.texture = iconTex

func _process(delta):
	if (WumpaTimer > 0.0):
		WumpaTimer -= delta
		if (WumpaTimer <= 0.0):
			WumpaHolder.visible = false
	
	if (LivesTimer > 0.0):
		LivesTimer -= delta
		if (LivesTimer <= 0.0):
			LivesHolder.visible = false

func UpdateWumpa():
	
	WumpaHolder.visible = true
	WumpaTimer = 5.0
	WumpaLabel.text = str(RehabSceneRoot.Root.Game.Fruit)

func UpdateLives():
	if (!LivesHolder.visible):
		var iconID = 0
		if (AgentCharacter.activeCharacter != null):
			iconID = AgentCharacter.activeCharacter.RegInt[0]
		if (ResourceLoader.exists(LivesIconPaths[iconID])):
			var iconTex = ImageTexture.new()
			iconTex.image = load(LivesIconPaths[iconID])
			LivesIcon.texture = iconTex
	LivesHolder.visible = true
	LivesTimer = 5.0
	LivesLabel.text = str(RehabSceneRoot.Root.Game.Lives)

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
