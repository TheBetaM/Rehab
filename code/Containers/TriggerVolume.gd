# Godot data container class for Trigger
extends Area3D
class_name TriggerVolume
@export var Messages : Dictionary # int, bool
@export var InstanceRefs : Array[NodePath]
@export var SomeFloat: float
@export var SectionHead: int
@export var Mask : Array[bool]

func _ready():
	body_entered.connect(OnTrigger)

func OnTrigger(body):
	if !(body is AgentCharacter): return;
	#if !(Mask[body.RegInt[AgentCharacter.CharISlot.AgentType]]): return;
