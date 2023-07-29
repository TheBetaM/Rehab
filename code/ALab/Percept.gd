extends RefCounted
class_name ALab_Percept

var Param : int
var Interval : float
var Threshold : float
var Invert : bool
var Caller : Agent
var Callback

var LastResult : bool

func _init(inv : bool, par : int, inter : float, thres : float, caller : Agent, f):
	Invert = inv
	Param = par
	Interval = inter
	Threshold = thres
	Caller = caller
	Callback = f

func run():
	var res = Result()
	LastResult = res
	if (res):
		Callback.call()
	return res

func Result():
	return false
