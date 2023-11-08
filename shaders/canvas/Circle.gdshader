shader_type canvas_item;

void fragment() {
	vec2 uv = (UV * 2.0) - 1.0;
	float c = length(uv);
	COLOR.rgb = vec3(COLOR.rgb);
	if (c <= 1.0)
	{
		COLOR.a = COLOR.a * 1.0;
	}
	else
	{
		//COLOR.a = 0.0;
		discard;
	}
}
