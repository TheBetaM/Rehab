class_name RehabGame

var Dev : bool = false # development mode flag
var ChunkData : Dictionary # string ChunkName : list of persistent flags of instances
var Progress : int = 0
var Lives : int = 4
var Fruit : int = 0
var Crystals : int = 0
var LevelID : int = -1
var Gems : Dictionary #int levelID : list of gems?
var PlayerMode
var PlayerCharacterType
var SavePointChunk
var SavePointPos
var SavePointRot
var CheckPointChunk
var CheckPointPos
var CheckPointRot
static var InvertCameraX : bool = false
static var InvertCameraY : bool = false
static var UseMouseCamera : bool = true
static var AssetsPath : String = "res://import/"

func ResetGame():
	Fruit = 0
	Lives = 4
	Progress = 0
	Crystals = 0
	LevelID = -1

func AddWumpa(amount : int):
	Fruit += amount;
	if (Fruit > 99):
		Fruit = 0
		AddLives(amount)
	if (Fruit < 0):
		Fruit = 0
	RehabSceneRoot.Root.GameHUD.AnimateWumpa()
	RehabSceneRoot.Root.GameHUD.UpdateWumpa()

func AddLives(amount: int):
	Lives += amount;
	if (Lives > 99):
		Lives = 99
	if (Lives < 0):
		Lives = 0
		RehabSceneRoot.Root.ForceGameOver()
	RehabSceneRoot.Root.GameHUD.AnimateLife()
	RehabSceneRoot.Root.GameHUD.UpdateLives()

func AddGem(gem : int):
	RehabSceneRoot.Root.GameHUD.AnimateGem(gem)
	if (Gems.has(LevelID)):
		if (!Gems[LevelID].has(gem)):
			Gems[LevelID].append(gem)
	else:
		Gems[LevelID] = [gem]

func AddCrystal():
	Crystals += 1;
	RehabSceneRoot.Root.GameHUD.AnimateCrystal()


func DisplayHUD():
	RehabSceneRoot.Root.GameHUD.UpdateWumpa()
	RehabSceneRoot.Root.GameHUD.UpdateLives()

func SetLevelID(id : int):
	LevelID = id
