shader_type spatial;
render_mode cull_disabled, depth_draw_opaque, blend_mul, unshaded;
//render_mode cull_disabled, depth_prepass_alpha, depth_draw_always, blend_add, unshaded;
#define DOUBLE_COLOR
#define IS_REFLECT
#include "TexScrollAlphaBlendBase.gdshaderinc"