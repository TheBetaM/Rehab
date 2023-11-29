shader_type spatial;
render_mode cull_disabled, depth_draw_opaque, blend_mix, unshaded;
//render_mode cull_disabled, depth_prepass_alpha, depth_draw_always, blend_add, unshaded;
#define IS_ALPHA_BLEND
#include "TexScrollBase.gdshaderinc"