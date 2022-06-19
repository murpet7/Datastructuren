#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
in vec3 fragPos;
uniform sampler2D pixels;	// texture sampler

uniform vec3 lightColor;
uniform vec3 lightPos;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    //AMBIENT
    float ambientstrength = 0.2;
    vec3 ambient = ambientstrength * lightColor;

    //DIFFUSE
    vec3 norm = normalize(normal.xyz);
    vec3 lightDir = normalize(lightPos - fragPos);
    float diffuse = max(dot(norm, lightDir), 0.0);
    vec3 diffusedColor = diffuse * lightColor;
    
    //OUTPUT
    vec3 result = texture( pixels, uv ).xyz * (ambient+diffusedColor);
    outputColor = vec4(result, 1.0);


}