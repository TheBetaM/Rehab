shader_type spatial;
render_mode cull_disabled, depth_prepass_alpha, vertex_lighting, fog_disabled;
#define DOUBLE_COLOR
#define IS_ALPHA_TEST
#include "StandardBase.gdshaderinc"