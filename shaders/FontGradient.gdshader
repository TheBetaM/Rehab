shader_type canvas_item;
uniform sampler2D gradient;
uniform float gradient_offset = 0.0;
uniform bool multiline = false;

void vertex()
{
	float uv = 0.0;
	if (multiline)
	{
		uv = mod(VERTEX.y, 66.0) - 16.0;
	}
	else
	{
		uv = mod(VERTEX.y, 104.0) - gradient_offset;
	}
	COLOR.rgb = (COLOR.r * (texture(gradient, vec2(uv, 0.0)).rgb)) + (COLOR.g * 1.0) + (COLOR.b * 0.0);
}


