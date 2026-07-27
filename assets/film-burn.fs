/*{
  "DESCRIPTION": "Warm light bleeding in from the frame edges, breathing slowly \u2014 the burnt-film transition look, running continuously.",
  "CATEGORIES": ["Guillotine", "Stylize"],
  "INPUTS": [
  {
    "NAME": "inputImage",
    "TYPE": "image"
  },
  {
    "NAME": "intensity",
    "TYPE": "float",
    "DEFAULT": 0.55,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "NAME": "speed",
    "TYPE": "float",
    "DEFAULT": 0.2,
    "MIN": 0.02,
    "MAX": 1.0
  }
]
}*/
void main() {
  vec2 uv = isf_FragNormCoord;
  vec4 c = IMG_THIS_PIXEL(inputImage);

  // Distance to the nearest edge drives the burn; corners get it worst, like real film.
  float edge = 1.0 - min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y)) * 2.0;
  edge = clamp(edge, 0.0, 1.0);

  // Two out-of-phase breaths so the bleed never looks like a static vignette.
  float breathe = 0.6 + 0.4 * sin(TIME * speed * 2.0) * cos(TIME * speed * 1.3 + uv.x * 3.0);
  float burn = pow(edge, 2.2) * breathe * intensity;

  // Hot orange core fading to deep red at the extreme edge.
  vec3 fire = mix(vec3(1.0, 0.55, 0.15), vec3(1.0, 0.2, 0.05), edge);
  vec3 col = 1.0 - (1.0 - c.rgb) * (1.0 - fire * burn);
  gl_FragColor = vec4(col, c.a);
}
