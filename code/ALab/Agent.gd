extends PhysicsBody3D
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
var ParentScene : ChunkScene
var ActiveModel : int = 0
var ActiveAnim : int = -1
var ActiveSkeleton : Skeleton3D
var JointsConst : Array[int] #Joint-ID ones
var ExitPoints : Array[Node3D]
var ColShapes : Dictionary #int submodel index, Array[CollisionShape3D] shapes

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
	AudioSource.bus = &"SFX"
	if (get_node_or_null("Children")):
		for i in $Children.get_children():
			if (i is Agent):
				SubActors.append(i)
	if (get_node_or_null("Models")):
		for i in $Models.get_children():
			for a in i.get_child(0).get_children():
				if (a is CollisionShape3D):
					if (SubModels.size() != 0):
						a.disabled = true
						a.process_mode = Node.PROCESS_MODE_DISABLED
					if (!ColShapes.has(SubModels.size())):
						ColShapes[SubModels.size()] = [ a ]
					else:
						ColShapes[SubModels.size()].append(a)
					a.reparent(self)
			SubModels.append(i)
	for i in range(0, JointIDCount):
		JointsConst.append(-1)
	for i in range(0, ExitPointCount):
		ExitPoints.append(null)
	if (SubModels.size() > 0):
		ActiveSkeleton = SubModels[ActiveModel].get_child(0).get_child(0)
	
	var parent = get_parent()
	while ParentScene == null and parent != null:
		if (parent is ChunkScene):
			ParentScene = parent
		else:
			parent = parent.get_parent()
	
	if (ParentScene != null):
		UpdateLayers(ParentScene.ChunkLayer)
	UpdateActiveModel()
	#if (!Engine.is_editor_hint()):
	#	Scripts[SSlot.Spawn].run(0)

func _process(delta):
	if (ActiveState):
		ActiveState.run(delta)
	
func OnMessage(message : int):
	if Messages.find_key(message):
		Messages[message].run()

func DoAnimation(slot : int, loop : bool):
	if (slot >= ModelActions.size()):
		return;
	if (ModelActions[slot].keys()[0] == null):
		return;
	var ogi = ModelActions[slot].keys()[0]
	var animName = ModelActions[slot].values()[0]
	if (ActiveAnim == slot && ogi == ActiveModel):
		return;
	if (ogi != ActiveModel):
		for i in SubModels:
			i.visible = false
			i.process_mode = Node.PROCESS_MODE_DISABLED
		#SubModels[ogi].get_child(0).global_position = SubModels[ActiveModel].get_child(0).global_position
		#SubModels[ogi].get_child(0).global_rotation_degrees = SubModels[ActiveModel].get_child(0).global_rotation_degrees
		#SubModels[ogi].get_child(0).global_scale = SubModels[ActiveModel].get_child(0).global_scale
		SubModels[ogi].visible = true
		SubModels[ogi].process_mode = Node.PROCESS_MODE_INHERIT
		if (ColShapes.has(ActiveModel)):
			for i in ColShapes[ActiveModel]:
				i.disabled = true
				i.process_mode = Node.PROCESS_MODE_DISABLED
		if (ColShapes.has(ogi)):
			for i in ColShapes[ogi]:
				i.process_mode = Node.PROCESS_MODE_INHERIT
				i.disabled = false
		ActiveModel = ogi
		ActiveSkeleton = SubModels[ogi].get_child(0).get_child(0)
		UpdateActiveModel()
	if (animName != null):
		var animPlayer : AnimationPlayer = SubModels[ogi].get_node("AnimationPlayer")
		if (loop):
			animPlayer.get_animation(animName).loop_mode = Animation.LOOP_LINEAR
		else:
			animPlayer.get_animation(animName).loop_mode = Animation.LOOP_NONE
		if (ActiveAnim == -1):
			animPlayer.play(animName)
		else:
			animPlayer.play(animName, 0.25)
		ActiveAnim = slot

func DoSound(slot : int, pitch : float, volume : float):
	if (slot >= Sounds.size()):
		return;
	if (Sounds[slot]):
		#AudioSource.reparent(SubModels[ActiveModel].get_child(0))
		AudioSource.volume_db = volume
		AudioSource.position = Vector3.ZERO
		AudioSource.process_mode = Node.PROCESS_MODE_ALWAYS
		AudioSource.stream = Sounds[slot]
		AudioSource.pitch_scale = pitch
		AudioSource.play()

func DoSoundPath(path : String, pitch : float, volume : float):
	if (!ResourceLoader.exists(path)):
		return
	#AudioSource.reparent(SubModels[ActiveModel].get_child(0))
	AudioSource.volume_db = volume
	AudioSource.position = Vector3.ZERO
	AudioSource.process_mode = Node.PROCESS_MODE_ALWAYS
	AudioSource.stream = ResourceLoader.load(path)
	AudioSource.pitch_scale = pitch
	AudioSource.play()

func DoSoundStream(stream : AudioStream, pitch : float, volume : float):
	#AudioSource.reparent(SubModels[ActiveModel].get_child(0))
	AudioSource.volume_db = volume
	AudioSource.position = Vector3.ZERO
	AudioSource.process_mode = Node.PROCESS_MODE_ALWAYS
	AudioSource.stream = stream
	AudioSource.pitch_scale = pitch
	AudioSource.play()


func UpdateLayers(layer : int):
	#Updating collision and light layers in child nodes
	UpdateLayersNested(self, layer)

func UpdateLayersNested(parent : Node, layer : int):
	for i in parent.get_children():
		UpdateLayersNested(i, layer)
		if (i is VisualInstance3D):
			i.set_layer_mask_value(1, false)
			i.set_layer_mask_value(layer, true)
			if (i is Light3D):
				i.light_cull_mask = i.light_cull_mask | (1 << (layer - 1)) 
		elif (i is CollisionObject3D):
			if (i.get_collision_layer_value(1) == false):
				return;
			i.set_collision_mask_value(1, false)
			i.set_collision_layer_value(1, false)
			i.set_collision_mask_value(layer, true)
			i.set_collision_layer_value(layer, true)

func UpdateActiveModel():
	for i in range(0, JointIDCount):
		JointsConst[i] = -1
	for i in range(0, JointIDCount):
		var JointID = SubModels[ActiveModel].find_child("JointID-" + str(i), true)
		if (JointID != null):
			var attach : BoneAttachment3D = JointID.get_parent()
			JointsConst[i] = attach.bone_idx
			var skeleton = attach.get_parent()
			ActiveSkeleton = skeleton
	for i in range(0, ExitPointCount):
		ExitPoints[i] = SubModels[ActiveModel].find_child("ExitPoint" + str(i), true)


func OnChunkEnter():
	pass

var ShadowPaths = [
	"res://assets/textures/shadow/clin.png",
	"res://assets/textures/shadow/cube.png",
	"res://assets/textures/shadow/rcub.png",
	"res://assets/textures/shadow/octo.png",
]

func CreateShadow(type : int, dsize : Vector2, boneAttach : int):
	if (SubModels.size() == 0): return
	var shad = Decal.new()
	#shad.script = load("res://code/Containers/DecalShadow.gd")
	shad.size = Vector3(dsize.x, 10, dsize.y)
	shad.texture_albedo = load(ShadowPaths[type])
	shad.upper_fade = 0
	shad.lower_fade = 0.5
	shad.distance_fade_enabled = true
	shad.distance_fade_begin = 40
	shad.layers = 1
	shad.modulate = Color(1.0, 1.0, 1.0, 0.5)
	#SubModels[ActiveModel].get_child(0).add_child(shad)
	get_node("Shadows").add_child(shad)
	shad.position.y = -4.99

func ControlPackUpdate():
	CTRLPACK.Update()

func ControlPackRun(delta):
	var cont : bool = CTRLPACK.Run(delta)
	return cont

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
	
	var DelayTimer : float = 0.0
	
	func Run(delta):
		if Delay != null:
			DelayTimer -= delta
			if (DelayTimer > 0.0):
				return false
		if Stalls:
			return false
		return true
	
	func Update():
		if Delay != null:
			DelayTimer = Delay
	
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
