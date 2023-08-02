shader_type canvas_item;
//render_mode blend_mix;
uniform sampler2D gradient;

void fragment() 
{
	//COLOR.a = texture(TEXTURE, UV).a;
	//COLOR.a = (1.0 - COLOR.r) * texture(TEXTURE, UV).a;
	vec2 uv = vec2(UV.y * 15.0, 0.0);
	COLOR.rgb = (COLOR.r * (texture(gradient, uv).rgb)) + (COLOR.g * 1.0) + (COLOR.b * 0.0);
}


