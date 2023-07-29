extends Node3D
class_name ChunkScene

@export_file("*.tscn") var SkydomePath : String
@export var WorldEnv : Environment
@export var ActiveScene : bool
@export var Links : Array[ChunkLink]
var ChunkLayer : int


func UpdateLayers(layer : int):
	ChunkLayer = layer
	#Updating collision and light layers in child nodes
	UpdateLayersNested(self)

func UpdateLayersNested(parent : Node):
	for i in parent.get_children():
		UpdateLayersNested(i)
		if (i is VisualInstance3D):
			i.set_layer_mask_value(1, false)
			i.set_layer_mask_value(ChunkLayer, true)
			if (i is Light3D):
				i.light_cull_mask = i.light_cull_mask | (1 << (ChunkLayer - 1)) 
				if (!ActiveScene):
					i.shadow_enabled = false
		if (i is CollisionObject3D):
			i.set_collision_mask_value(1, false)
			i.set_collision_layer_value(1, false)
			i.set_collision_mask_value(ChunkLayer, true)
			i.set_collision_layer_value(ChunkLayer, true)
