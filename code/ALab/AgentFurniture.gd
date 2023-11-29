extends Agent
class_name AgentFurniture

var Doors = [
	"Boiler_DoubleDoor_Anim",
	"Door_Cortex_Lab_Big",
	"AltEarth_AntCaves_GiantBlastDoor",
	"AltEarth_Core_BlaseDoor_Small",
	"Battleship_Door_B",
	"Battleship_IronDoor_B",
	"Generic_GreyStoneDoor",
	"Global_StreamingDoor",
	"Ice_CortexDoorCockBlocker",
	"School_Classroom_Door",
	"School_OneWayActiveDoor",
	"School_OneWayDoor",
	"Village_Stockade_Gate",
]


func _ready():
	super()
	
	DoAnimation(0, true)
	
	# temp to make exploring in explorer easier
	if (Doors.has(name)):
		set("contact_monitor", true)
		set("max_contacts_reported", 256)
		set("freeze_mode",RigidBody3D.FREEZE_MODE_KINEMATIC)
		collision_layer = 0
		connect("body_entered", OnDoorTouch)

func OnChunkEnter():
	if (name == "DJ"):
		RehabSceneRoot.Root.PlayMusic(RegInt[0])
		RehabSceneRoot.Game.SetLevelID(RegInt[2])
	elif (name == "Global_Ambient_Sound"):
		RehabSceneRoot.Root.PlayAmbience(RegInt[2])
	elif (name == "Util_JungleHousekeeping"):
		RehabSceneRoot.Root.PlayMusic(27)
		RehabSceneRoot.Game.SetLevelID(0)
	elif (name == "Util_CavernHousekeeping"):
		RehabSceneRoot.Root.PlayMusic(28)
		RehabSceneRoot.Game.SetLevelID(3)
	elif (name == "Util_IceHousekeeping"):
		RehabSceneRoot.Root.PlayMusic(27)
		RehabSceneRoot.Game.SetLevelID(6)

func OnDoorTouch(body):
	if (body is CharacterBody3D and body is AgentCharacter):
		if (AgentCharacter.activeCharacter == body):
			#collision_layer = 0
			visible = false
			call_deferred("DisableDoor")

func DisableDoor():
	process_mode = Node.PROCESS_MODE_DISABLED
