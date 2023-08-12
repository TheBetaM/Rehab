#include "TexScrollVars.gdshaderinc"

void vertex()
{
	if (deform)
	{
		float vy = sin(TIME + float(VERTEX_ID)) * deformspeed.y * 0.1;
		float vx = sin(TIME + float(VERTEX_ID)) * deformspeed.x * 0.1;
		float vz = sin(TIME + float(VERTEX_ID)) * deformspeed.x * 0.1;
		VERTEX *= vec3(vx + 1.0, vy + 1.0, vz + 1.0);
	}
	if (billboard_mode == 2)
	{
		MODELVIEW_MATRIX = VIEW_MATRIX * mat4(vec4(normalize(cross(vec3(0.0, 1.0, 0.0), INV_VIEW_MATRIX[2].xyz)), 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(normalize(cross(INV_VIEW_MATRIX[0].xyz, vec3(0.0, 1.0, 0.0))), 0.0), MODEL_MATRIX[3]);
		MODELVIEW_NORMAL_MATRIX = mat3(MODELVIEW_MATRIX);
	}
}

void fragment() 
{
	vec2 uvs = speed * vec2(TIME) + UV;
	vec4 tex = texture(albedo_texture, uvs);
	ALBEDO = tex.xyz * COLOR.rgb;
	ALPHA = tex.w * COLOR.a;
	
	if (reflects)
	{
		vec4 current_texture = texture(SCREEN_TEXTURE, vec2(SCREEN_UV.x + reflectDist, 1.0 - SCREEN_UV.y - (0.5 - 0.5) * 2.0 + reflectDist));
		ALBEDO = mix(ALBEDO, current_texture.xyz, 0.5);
		ALPHA *= 0.85;
	}
	
	if (envmap)
	{
		vec4 albedo_tex = textureProj(albedo_texture,INV_PROJECTION_MATRIX * VIEW_MATRIX * vec4(VIEW,0));
		ALBEDO = albedo_tex.rgb * COLOR.rgb;
		ALPHA = albedo_tex.w * COLOR.a;
	}
	
	if (swapcolors)
	{
		ALBEDO.rgb = vec3(-ALBEDO.r, -ALBEDO.g, -ALBEDO.b);
	}

    METALLIC = metallic_specular;
    BACKLIGHT = vec3(1, 1, 1);
	ALPHA_SCISSOR_THRESHOLD = 0.5;
	
}