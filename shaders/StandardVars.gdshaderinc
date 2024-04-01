uniform vec2 speed;
uniform sampler2D albedo_texture : source_color;
uniform bool billboard;
#ifdef IS_REFLECT
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
#endif
uniform float reflectDist = 0.15;
uniform bool envmap = false;
uniform bool deform = false;
uniform vec2 deformspeed;
uniform vec2 deformspeed2;
uniform float metallic_specular = 0.0;

uniform vec4 albedo_color : source_color = vec4(1);
uniform vec2 uv1_scale = vec2(1);
uniform vec2 uv1_offset;

vec4 texturePanorama(vec3 normal, sampler2D pano) 
{
    vec2 st = vec2(
            atan(normal.x, normal.z),
            acos(normal.y));

    if (st.x < 0.0)
        st.x += PI * 2.0;

    st /= vec2(PI * 2.0, PI);

    return textureLod(pano, st, 0.0);
}