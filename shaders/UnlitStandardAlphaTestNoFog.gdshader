shader_type spatial;
render_mode cull_disabled, depth_prepass_alpha, depth_draw_always, unshaded, fog_disabled;
#define IS_ALPHA_TEST
#include "StandardBase.gdshaderinc"