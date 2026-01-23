shader_type spatial;

//void vertex() {
	// Called for every vertex the material is visible on.
//}

//void fragment() {
	// Called for every pixel the material is visible on.
//}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}

render_mode cull_back, depth_draw_opaque;

uniform vec3 grass_color : source_color = vec3(0.2, 0.7, 0.3);
uniform vec3 dirt_color  : source_color = vec3(0.45, 0.3, 0.15);

uniform float blend_strength = 2.5;

void fragment() {
    // Normal en espacio mundial
    vec3 n = normalize(NORMAL);

    // NORMAL.y = 1  → totalmente hacia arriba
    // NORMAL.y = 0  → lateral
    float slope = clamp(pow(n.y, blend_strength), 0.0, 1.0);

    vec3 color = mix(dirt_color, grass_color, slope);

    ALBEDO = color;
}
