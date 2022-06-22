#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
in vec3 fragPos;
uniform sampler2D pixels;	// texture sampler

uniform vec3 lightColor;
uniform vec3 lightPos;
uniform vec3 viewPos;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    //AMBIENT
    float ambientstrength = 0.2;
    vec3 ambientColor = ambientstrength * lightColor;

    //DIFFUSE
    vec3 norm = normalize(normal.xyz);
    vec3 lightDir = normalize(lightPos - fragPos);
    float diffuse = max(dot(norm, lightDir), 0.0);
    vec3 diffusedColor = diffuse * lightColor;

    //SPECULAR
    float specstrength = 0.6;
    float scatter = 32;
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, normal.xyz);
    float specular = pow(max(dot(viewDir, reflectDir), 0.0), scatter);
    vec3 specularColor = specstrength * specular * lightColor;
    
    //OUTPUT
    //vec3 result = texture( pixels, uv ).xyz * (ambientColor+diffusedColor+specularColor);
    vec3 result = texture( pixels, uv ).xyz * (ambientColor+diffusedColor+specularColor);
    outputColor = vec4(result, 1.0);


}