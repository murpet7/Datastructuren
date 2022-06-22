﻿	#version 330
 
// shader input
in vec2 vUV;			// vertex uv coordinate
in vec3 vNormal;		// untransformed vertex normal
in vec3 vPosition;		// untransformed vertex position

// shader output
out vec4 normal;		// transformed vertex normal
out vec2 uv;				
out vec3 fragPos;
uniform mat4 transform;
uniform mat4 model;
 
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);
	fragPos = vec3(model * vec4(vPosition, 1.0));

	// forward normal and uv coordinate; will be interpolated over triangle
	//normal = transform * vec4( vNormal, 0.0f );
	//normal = vec4( vNormal * mat3(transpose( inverse( model))), 0.0f);
	normal = transpose(inverse(model)) * vec4( vNormal, 0.0f );
	uv = vUV;
}