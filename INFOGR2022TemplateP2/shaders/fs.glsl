#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
uniform sampler2D pixels;	// texture sampler

uniform vec3 lightColor;


// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    float ambientstrength = 0.9;
    vec3 ambient = ambientstrength * lightColor;
    vec3 result = texture( pixels, uv ).xyz * ambient;
    
    //outputColor = texture( pixels, uv ) + 0.5f * vec4( normal.xyz, 1 );
    outputColor = vec4(result, 1.0);

}