extends Node3D
class_name Agent

# GameObject data
@export var UnkTypeValue : int
@export var JointIDCount : int
@export var ExitPointCount : int
@export var Messages : Dictionary #int message, ALabScript script
@export var Scripts : Array[ALabScript]
@export var ModelActions : Array[Dictionary] #int OGI index, string Animation name
@export var Sounds : Array[Resource]
var CTRLPACK : ControlPacket
var SubActors : Array[Agent]
var SubModels : Array[Node3D]
var AudioSource : AudioStreamPlayer3D

# Instance data
@export var InstanceScript : ALabScript
@export var RefList : int
@export var LinkInstance : Array[Agent]
@export var LinkPath : Array[Path3D]
@export var LinkPoint : Array[Marker3D]
@export var RegAngle : Array[int]
@export var RegFloat : Array[float]
@export var RegInt : Array[int]

# FSM
var ActiveState : ALabScript

enum FSlot {
	UnkFloat = 0,
}
enum ISlot {
	AgentType = 0,
	UnkInt = 1,
}
enum SSlot {
	Spawn = 0,
	Trigger = 1,
	Damaged = 2,
	Touched = 3,
	Headbutted = 4,
	LandedOn = 5,
	SpinAttacked = 6,
	Bodyslammed = 7,
	SlideAttacked = 8,
	PhysicsCollision = 9,
	Unk10 = 10,
}

func _ready():
	AudioSource = $AudioStreamPlayer3D
	if (get_node("Children")):
		for i in $Children.get_children():
			if (i is Agent):
				SubActors.append(i)
	if (get_node("Models")):
		for i in $Models.get_children():
			SubModels.append(i)
	#if (!Engine.is_editor_hint()):
	#	Scripts[SSlot.Spawn].run()

func _process(delta):
	if (ActiveState):
		ActiveState.run()
	
func OnMessage(message : int):
	if Messages.find_key(message):
		Messages[message].run()

func DoAnimation(slot : int):
	if (ModelActions[slot].keys()[0] == null):
		pass
	var ogi : int = ModelActions[slot].keys()[0]
	var animName : String = ModelActions[slot].values()[0]
	for i in SubModels:
		i.visible = false
		i.process_mode = Node.PROCESS_MODE_DISABLED
	SubModels[ogi].visible = true
	SubModels[ogi].process_mode = Node.PROCESS_MODE_INHERIT
	var animPlayer : AnimationPlayer = SubModels[ogi].get_node("AnimationPlayer")
	animPlayer.play(animName)

func DoSound(slot : int):
	if (Sounds[slot]):
		AudioSource.stream = Sounds[slot]
		AudioSource.play()

func ControlPackUpdate():
	pass

func ControlPackReset():
	CTRLPACK.Reset()

class ControlPacket:
	var SpaceType
	var MotionType
	var RotationType
	var NaturalType
	var AccelType
	var Translates : bool
	var Rotates : bool
	var UsesPhysics : bool
	var UsesRotator : bool
	var UsesInterpolator : bool
	var InterpolatesAngles : bool
	var TranslationContinues : bool
	var YawFaces : bool
	var PitchFaces : bool
	var OrientsPredicts : bool
	var TracksDestination : bool
	var KeyIsLocal : bool
	var ContRotates : bool
	var Stalls : bool
	var Selector_SyncIndex #int
	var KeyIndex_FocusData #int
	var MoveSpeed_RiseHeight #float
	var TurnSpeed #float (angle)
	var RawPosX #float
	var RawPosY #float
	var RawPosZ #float
	var RawAngsX_Pitch #float (angle)
	var RawAngsY_Yaw #float (angle)
	var RawAngsZ_Roll #float (angle)
	var Delay #float
	var Duration_Curvy_Homepower #float
	var TumbleData #float
	var SpinData #float
	var TwistData #float
	var SqrTolerance_RandRange #float
	var Power_Gravity_Banking #float
	var Damping_SpeedLim_Braking #float
	var ACDist_RTOpt_ShiftFreq #float
	var DecDist_PhysOpt_Shift #float
	var Bounce_BankLimit #float
	var SyncUnit #int
	var JointIndex #int
	
	func Reset():
		Translates = false
		Rotates = false
		UsesPhysics = false
		UsesRotator = false
		UsesInterpolator = false
		InterpolatesAngles = false
		TranslationContinues = false
		YawFaces = false
		PitchFaces = false
		OrientsPredicts = false
		TracksDestination = false
		KeyIsLocal = false
		ContRotates = false
		Stalls = false
		Selector_SyncIndex = null
		KeyIndex_FocusData = null
		MoveSpeed_RiseHeight = null
		TurnSpeed = null
		RawPosX = null
		RawPosY = null
		RawPosZ = null
		RawAngsX_Pitch = null
		RawAngsY_Yaw = null
		RawAngsZ_Roll = null
		Delay = null
		Duration_Curvy_Homepower = null
		TumbleData = null
		SpinData = null
		TwistData = null
		SqrTolerance_RandRange = null
		Power_Gravity_Banking = null
		Damping_SpeedLim_Braking = null
		ACDist_RTOpt_ShiftFreq = null
		DecDist_PhysOpt_Shift = null
		Bounce_BankLimit = null
		SyncUnit = null
		JointIndex = null
