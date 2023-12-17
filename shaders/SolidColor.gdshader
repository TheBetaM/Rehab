shader_type spatial;
render_mode cull_disabled, depth_prepass_alpha, unshaded;
uniform vec3 color : source_color;

void fragment() 
{
	ALBEDO.rgb = color;
}
