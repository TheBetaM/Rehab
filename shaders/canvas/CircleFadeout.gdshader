shader_type canvas_item;

void fragment() {
	vec2 uv = (UV * 2.0) - 1.0;
	float c = length(uv);
	COLOR.rgb = vec3(COLOR.rgb);
	COLOR.a = (COLOR.a * 1.0) - c;
}
