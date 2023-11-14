extends Agent
class_name AgentFurniture

func _ready():
	super()
	
	DoAnimation(0, true)

func OnChunkEnter():
	if (name == "DJ"):
		RehabSceneRoot.Root.PlayMusic(RegInt[0])
	elif (name == "Global_Ambient_Sound"):
		RehabSceneRoot.Root.PlayAmbience(RegInt[2])
