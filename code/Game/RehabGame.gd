class_name RehabGame

static var Dev : bool = false # development mode flag
static var DemoMode : bool = true
var ChunkData : Dictionary # string ChunkName : list of persistent flags of instances
var Progress : int = 0
var Lives : int = 4
var Fruit : int = 0
var Crystals : int = 0
var LevelID : int = -1
var Gems : Dictionary #int levelID : list of gems?
var PlayerMode : int = 0
var PlayerCharacterType : int = 0
var SavePointChunk : String
var SavePointPos : Vector3
var SavePointRot : Vector3
var CheckPointChunk : String
var CheckPointPos : Vector3
var CheckPointRot : Vector3
static var InvertCameraX : bool = false
static var InvertCameraY : bool = false
static var UseMouseCamera : bool = true
static var AssetsPath : String = "res://import/"
static var ConfigPath : String = "user://rehab.cfg"
static var DataPath : String = OS.get_executable_path()

func Init():
	if (OS.get_name() == "Android"):
		#DataPath = OS.get_user_data_dir() + "/"
		DataPath = "/storage/emulated/0/Rehab/Packs/"
	else:
		var PathSplit = RehabGame.DataPath.split("/")
		var PacksPath = ""
		var PathID = 0
		for i in PathSplit:
			PathID += 1
			if (PathID < PathSplit.size()):
				PacksPath += i
				PacksPath += "/"
		PacksPath += "Packs/"
		DataPath = PacksPath
		if (!DirAccess.dir_exists_absolute(PacksPath)):
			DirAccess.make_dir_absolute(PacksPath)

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

func DisplayMessage(text : String):
	RehabSceneRoot.Root.GameHUD.FlashMessage(text)

func SetLevelID(id : int):
	LevelID = id
