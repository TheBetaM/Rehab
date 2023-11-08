shader_type spatial;
render_mode cull_disabled, depth_prepass_alpha, depth_draw_always, unshaded;
#define DOUBLE_COLOR
#define IS_REFLECT
#include "TexScrollAlphaTestBase.gdshaderinc"