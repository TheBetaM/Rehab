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
