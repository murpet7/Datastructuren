Student names / IDs: Yannick den Boer (6803113), Brian van den Berg (8487685)

We have implemented all of the minimum requirements, exept that we can't change the FOV in the application. 
The resolution can be changed in tempplate.cs OnLoad().
The camera can be moved around with W, A, S, D, Space, Shift. 
The debug output does not have shadow and secondary rays.
We have also implemented triangles in our program.
The raytracer makes a ray for every pixel of the screen, the rays look for intersections with objects and remebers the object it intersects with first.
If the object is reflective, another ray is made for the reflection, which will repeat if the reflected to object is reflective too. 
The colors of the objects that it reflected will be combined. Then, the raytracer makes the material diffuse. 
Then we match the pixel to the right color.

Materials used:
https://stackoverflow.com/questions/54564286/triangle-intersection-test-in-opengl-es-glsl 
(and other stackoverflow pages)

https://opentk.net/learn/chapter1/9-camera.html
