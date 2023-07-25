extends RefCounted
class_name ALab_Percept
	
static func run(par : int, inter : float, thres : float, caller : Agent):
	return Result(par, inter, thres, caller)
	
static func Result(Param : int, Interval : float, Threshold : float, Caller : Agent):
	return false
