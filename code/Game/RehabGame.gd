class_name RehabGame

var ChunkData : Dictionary # string ChunkName : list of persistent flags of instances
var Progress : int = 0
var Lives : int = 4
var Fruit : int = 0
var Crystals : int = 0
var Gems : Dictionary #int levelID : list of gems?
var PlayerMode
var PlayerCharacterType
var SavePointChunk
var SavePointPos
var SavePointRot
var CheckPointChunk
var CheckPointPos
var CheckPointRot
static var AssetsPath : String = "res://import/"

func ResetGame():
	Fruit = 0
	Lives = 4
	Progress = 0
	Crystals = 0

func AddWumpa(amount : int):
	Fruit += amount;
	if (Fruit > 99):
		Fruit = 0
		AddLives(amount)
	if (Fruit < 0):
		Fruit = 0
	RehabSceneRoot.Root.GameHUD.UpdateWumpa()

func AddLives(amount: int):
	Lives += amount;
	if (Lives > 99):
		Lives = 99
	if (Lives < 0):
		Lives = 0
	RehabSceneRoot.Root.GameHUD.UpdateLives()
