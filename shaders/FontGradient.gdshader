shader_type canvas_item;
//render_mode blend_mix;
uniform sampler2D gradient;
uniform float gradient_offset = 0.0;

void vertex()
{
	//vec2 uv = UV.yx;
	float uv = (VERTEX.y - gradient_offset); // -200.0 on upper, - 0.0 on button
	//COLOR.a = uv.y / 200.0;
	COLOR.rgb = (COLOR.r * (texture(gradient, vec2(uv, 0.0)).rgb)) + (COLOR.g * 1.0) + (COLOR.b * 0.0);
}

void fragment() 
{
	//COLOR.a = texture(TEXTURE, UV).a;
	//COLOR.a = (1.0 - COLOR.r) * texture(TEXTURE, UV).a;
	//vec2 uv = vec2(UV.y * 3.0, 0.0);
	//COLOR.rgb = (COLOR.r * (texture(gradient, uv).rgb)) + (COLOR.g * 1.0) + (COLOR.b * 0.0);
	
	//vec2 uv = UV.yx;
	//COLOR.rgb = (COLOR.r * (texture(gradient, uv).rgb)) + (COLOR.g * 1.0) + (COLOR.b * 0.0);
	
	//COLOR.rgb = texture(gradient, uv).rgb;
	//COLOR.a = 1.0;
}


