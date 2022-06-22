#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;

void main()
{
	//VIGNETTING
	float lightintensity = 30;
	float vignettescatter = 0.25;
	vec2 uv1 = uv*( 1 - uv);
	float vignet = pow(uv1.x*uv1.y*lightintensity, vignettescatter);

	//CHROMATIC ABBERATION
	float abberationintensity = 0.007;
	vec3 abberationColor;
	abberationColor.x = texture(pixels, vec2(uv.x+abberationintensity, uv.y)).x;
	abberationColor.y = texture(pixels, vec2(uv.x, uv.y)).y;
	abberationColor.z = texture(pixels, vec2(uv.x-abberationintensity, uv.y)).z;
	abberationColor *= 1 - abberationintensity;


	//RESULT
	//--retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;

	//--Abberation
	outputColor = abberationColor;

	//--Vignetting
	outputColor *= vignet;
}

// EOF