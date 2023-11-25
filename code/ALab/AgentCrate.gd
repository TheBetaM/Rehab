extends Agent
class_name AgentCrate


enum CrateFSlot {
	UnkFloat1 = 0,
	UnkFloat2 = 1,
	UnkFloat3 = 2,
}


func _ready():
	super()
	
	CreateShadow(1, Vector2.ONE, 0)
