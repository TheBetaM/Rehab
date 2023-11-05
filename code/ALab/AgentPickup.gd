extends Agent
class_name AgentPickup

func _ready():
	super()
	
	SubModels[0].rotation_degrees = Vector3(0, (randf() - 0.5) * 360.0, 0);

func _physics_process(delta):
	SubModels[0].rotate_y(3.0 * delta)
