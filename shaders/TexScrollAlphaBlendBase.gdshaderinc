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
	if (!OUTPUT_IS_SRGB) {
		COLOR.rgb = mix(pow((COLOR.rgb + vec3(0.055)) * (1.0 / (1.0 + 0.055)), vec3(2.4)), COLOR.rgb * (1.0 / 12.92), lessThan(COLOR.rgb, vec3(0.04045)));
	}
	#ifdef DOUBLE_COLOR
	    COLOR.rgb = COLOR.rgb * 2.0;
	#endif
}

void fragment() 
{
	vec2 uvs = speed * vec2(TIME) + UV;
	vec4 tex = texture(albedo_texture, uvs);
	ALBEDO = tex.xyz * COLOR.rgb;
	ALPHA = tex.w * COLOR.a;
	
	#ifdef IS_REFLECT
		vec4 current_texture = texture(SCREEN_TEXTURE, vec2(SCREEN_UV.x + reflectDist, 1.0 - SCREEN_UV.y - (0.5 - 0.5) * 2.0 + reflectDist));
		ALBEDO = mix(ALBEDO, current_texture.xyz, 0.5);
	#endif
	
	if (envmap)
	{
		//vec4 albedo_tex = textureProj(albedo_texture,INV_PROJECTION_MATRIX * VIEW_MATRIX * vec4(VIEW,0));
		//ALBEDO = albedo_tex.rgb * COLOR.rgb;
		//ALPHA = albedo_tex.w * COLOR.a;
		
		//vec3 normal = (VIEW_MATRIX * vec4(VIEW, 0)).xyz * vec3(-1, -1, 1);
		//vec3 normal = (MODEL_MATRIX * vec4(VIEW, 0)).xyz * vec3(-1, -1, 1);
		vec3 normal = (MODEL_MATRIX * vec4(VIEW, 0)).xyz * vec3(-1, -1, 1);
		vec4 tex = texturePanorama(normal, albedo_texture);
    	ALBEDO = tex.xyz * COLOR.rgb;
		ALPHA = tex.w * COLOR.a;
		
		//IRRADIANCE = albedo_tex.rgba;
	}
	
	if (swapcolors)
	{
		ALBEDO.rgb = vec3(-ALBEDO.r, -ALBEDO.g, -ALBEDO.b);
	}

    METALLIC = metallic_specular;
    BACKLIGHT = vec3(1, 1, 1);
	
}