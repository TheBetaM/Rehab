shader_type canvas_item;
uniform sampler2D gradient;
void vertex()
{
	float uv = mod(VERTEX.y, 65.0) / 65.0;
	if (uv > 0.04 && uv < 0.1)
	{
		uv = uv * 50.0;
	}
	COLOR.rgb = (COLOR.r * (texture(gradient, vec2(uv, 0.0)).rgb)) + (COLOR.g * 1.0) + (COLOR.b * 0.0);
}