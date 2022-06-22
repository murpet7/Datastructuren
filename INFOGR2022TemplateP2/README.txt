Student names / IDs: Yannick den Boer (6803113), Brian van den Berg (8487685)

We have implemented all basic requirements for this assignment. 
The bonus assignment included in the project is the addition of vignetting and chromatic aberration.

The camera can move in all required ways, using W, A, S, D, space, shift and the mouse.

Our scene graph can hold any number of meshes. 
We created an Entity class, which contains a mesh and can contain children and a parent.
These entities are recursively rendered in the Render function in the SceneGraph class.
The entities have all the transformations that its parents have and their own transformations.
Entities that don't have an entity with a mesh as parent, have the "scene" entity as parent.
After that, multiple matrices are combined to end up with the right matrix for the entity.

Phong shading consists of 3 components:
- Ambient lighting: which lights up all vertices equally
- Diffuse lighting: calculates the angle between the vertex normal and the lightdirection. The smaller the angle, the bigger the light intensity
- Specular lighting: calculates the angle between the viewing direction and the direction of the reflected light. The smaller the angle, the bigger the light intensity

Post processing:
Vignetting: Calculates intensity using the distance from the pixel to the middle of the screen.
Chromatic Abberation: Offsets RGB values on the x axis by a certain amount.

Variables that can be altered in Fragment shading:
- Ambient strength
- Specular strength
- Specular scattering

Variables that can be altered in Post process fragment shading:
- Vignetting light intensity
- Vignette scatter
- Abberation intensity
Vignetting and Abberation can be turned off seperately by commenting their respective line of code that changes the outputColor.

The demo includes a floor and three other objects. The floor has the scene as parent, the middle object has the scene as parent,
the object circling the middle object has the middle object as parent and the outer object has that object as parent.
All three objects have their own rotation and translation, so the second and third objects rotate around their parent in the way you would expect.

Materials used:
https://learnopengl.com/Guest-Articles/2021/Scene/Scene-Graph
https://www.youtube.com/watch?v=lH61rdpJ5v8
https://www.youtube.com/watch?v=KdDdljGtfeg
https://www.shadertoy.com/view/Mds3zn
https://www.shadertoy.com/view/lsKSWR
https://opentk.net/learn/chapter2/2-basic-lighting.html
https://opentk.net/learn/chapter1/9-camera.html
free3d.com