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

func _ready():
	pass

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
