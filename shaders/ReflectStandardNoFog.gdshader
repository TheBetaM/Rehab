shader_type spatial;
render_mode cull_disabled, depth_prepass_alpha, unshaded, fog_disabled;
#define DOUBLE_COLOR
#define IS_REFLECT
#include "StandardBase.gdshaderinc"