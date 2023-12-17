# Godot data container class for Camera
extends TriggerVolume
class_name CameraTriggerVolume
@export var Camera1Type : int
@export var Camera2Type : int
@export var CamHeader : int
@export var CamHeader2 : int
@export var UnkShort : int
@export var UnkFloat1 : float
@export var UnkCoords1 : Vector2
@export var UnkCoords2 : Vector2
@export var UnkCoords3 : Vector2
@export var UnkCoords4 : Vector2
@export var UnkFloat2 : float
@export var UnkFloat3 : float
@export var UnkUInt1 : int
@export var UnkUInt2 : int
@export var UnkUInt3 : int
@export var UnkUInt4 : int
@export var UnkInt5 : int
@export var UnkInt6 : int
@export var UnkFloat4 : float
@export var UnkFloat5 : float
@export var UnkFloat6 : float
@export var UnkFloat7 : float
@export var UnkUInt7 : int
@export var UnkInt8 : int
@export var UnkUInt9 : int
@export var UnkFloat8 : float
@export var UnkByte : int

func _ready():
	body_entered.connect(OnEnter)
	body_exited.connect(OnExit)

func OnEnter(body):
	if (body != AgentCharacter.activeCharacter): return;
	#if !(Mask[body.RegInt[AgentCharacter.CharISlot.AgentType]]): return;
	RehabSceneRoot.Root.PlayerCam.CameraTriggerEntered(self)

func OnExit(body):
	if (body != AgentCharacter.activeCharacter): return;
	#if !(Mask[body.RegInt[AgentCharacter.CharISlot.AgentType]]): return;
	RehabSceneRoot.Root.PlayerCam.CameraTriggerExited(self)
