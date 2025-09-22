#include "StandardVars.gdshaderinc"

void vertex()
{
	if (deform)
	{
		float vy = sin(TIME + float(VERTEX.y)) * deformspeed.y * 0.1;
		float vx = sin(TIME + float(VERTEX.y)) * deformspeed.x * 0.1;
		float vz = sin(TIME + float(VERTEX.y)) * deformspeed.x * 0.1;
		VERTEX *= vec3(vx + 1.0, vy + 1.0, vz + 1.0);
	}
	if (billboard)
	{
		// Face towards point (0,0,0) of chunk
		vec3 nodepos = vec3(NODE_POSITION_WORLD.x, 0, NODE_POSITION_WORLD.z);
		vec3 nodeoffset = vec3(0);
		vec3 w = normalize(nodepos + nodeoffset);
		vec3 v = normalize(cross(w, vec3(0,0,1)));
		v = vec3(v.x, abs(v.y), v.z);
		vec3 u = cross(w, v);
		mat3 rot_mat = transpose(mat3(-u, v, w));
		VERTEX *= rot_mat;
		//MODELVIEW_MATRIX = VIEW_MATRIX * mat4(vec4(normalize(cross(vec3(0.0, 1.0, 0.0), INV_VIEW_MATRIX[2].xyz)), 0.0), vec4(0.0, 1.0, 0.0, 0.0), vec4(normalize(cross(INV_VIEW_MATRIX[0].xyz, vec3(0.0, 1.0, 0.0))), 0.0), MODEL_MATRIX[3]);
		//MODELVIEW_NORMAL_MATRIX = mat3(MODELVIEW_MATRIX);
	}
	if (!OUTPUT_IS_SRGB) {
		COLOR.rgb = mix(pow((COLOR.rgb + vec3(0.055)) * (1.0 / (1.0 + 0.055)), vec3(2.4)), COLOR.rgb * (1.0 / 12.92), lessThan(COLOR.rgb, vec3(0.04045)));
	}
	#ifdef DOUBLE_COLOR
	    COLOR.rgb = COLOR.rgb * 2.0;
	#endif
	UV = UV * uv1_scale + uv1_offset + speed * vec2(TIME);
}

void fragment()
{
	vec4 tex = texture(albedo_texture, UV);
	ALBEDO = tex.xyz * COLOR.rgb * albedo_color.xyz;
	#ifdef IS_ALPHA_TEST
	ALPHA = tex.w * COLOR.a * albedo_color.w;
	#endif
	#ifdef IS_ALPHA_BLEND
	ALPHA = tex.w * COLOR.a * albedo_color.w;
	#endif


	#ifdef IS_REFLECT
		vec4 current_texture = texture(SCREEN_TEXTURE, vec2(SCREEN_UV.x + reflectDist, SCREEN_UV.y - (0.5 - 0.5) * 2.0 + reflectDist));
		ALBEDO = current_texture.xyz * COLOR.rgb * tex.xyz;
	#endif

	if (envmap)
	{
		//vec4 albedo_tex = textureProj(albedo_texture,INV_PROJECTION_MATRIX * VIEW_MATRIX * vec4(VIEW,0));
		//ALBEDO = albedo_tex.rgb * COLOR.rgb;
		//ALPHA = albedo_tex.w * COLOR.a;
		vec3 normal = (MODEL_MATRIX * vec4(VIEW, 0)).xyz * vec3(-1, -1, 1);
    	vec4 tex = texturePanorama(normal, albedo_texture);
		ALBEDO = tex.xyz * COLOR.rgb;
		#ifdef IS_ALPHA_TEST
		ALPHA = tex.w * COLOR.a;
		#endif
		#ifdef IS_ALPHA_BLEND
		ALPHA = tex.w * COLOR.a;
		#endif
	}

    METALLIC = metallic_specular;
    BACKLIGHT = vec3(1);
	#ifdef IS_ALPHA_TEST
	ALPHA_SCISSOR_THRESHOLD = 0.5;
	//ALPHA_ANTIALIASING_EDGE = 0.1;
	#endif
}