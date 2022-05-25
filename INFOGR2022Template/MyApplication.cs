using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;

		Sphere sphere1;
		Sphere sphere2;
		Sphere sphere3;
		Plane plane1;
		Camera camera;
		public Ray ray;
		Raytracer raytracer;
		public List<Primitive> primitives;
		Light light1;
		public List<Light> lights;
		// initialize
		public void Init()
		{
			primitives = new List<Primitive>();
			sphere1 = new Sphere(new Vector3(0, 1.2f, 5f), 1.1f, new Clr(255, 0, 0));
			sphere2 = new Sphere(new Vector3(1, 1, 1.5f), .2f, new Clr(0, 255, 0));
			sphere3 = new Sphere(new Vector3(-1, 1, 2f), .4f, new Clr(0, 0, 255));
			plane1 = new Plane(new Vector3(0, 1, 0), 2, new Clr(255, 0, 255));
			primitives.Add(sphere1);
			primitives.Add(sphere2);
			primitives.Add(sphere3);
			primitives.Add(plane1);
			camera = new Camera();
			ray = new Ray(Vector3.Zero, Vector3.One, 100);
			raytracer = new Raytracer(this, camera, screen);
			light1 = new Light(new Vector3(10, 10, 10), new Color4(1f, 0, 0, 1f));
			lights = new List<Light>();
			lights.Add(light1);
		}
		// tick: renders one frame
		public void Tick()
		{
			screen.Clear( 0 );

			raytracer.Render();
		}

		public int TX(float point)
		{
			float shift = point + 2;
			float scale = shift * (screen.width / 4);
			return Convert.ToInt32(scale);
		}

		public int TY(float point)
		{
			float invert = -point;
			float scale = invert * (screen.width / 4);
			float shift = scale + (screen.height / 2);
			//4*160;4*100
			return Convert.ToInt32(shift);
		}
		public int MixColor(int red, int green, int blue)
		{
			return (red << 16) + (green << 8) + blue;
		}
	}
	class Ray
	{
		public Vector3 origin;
		public Vector3 direction;
		public float length;

		public Ray(Vector3 origin, Vector3 direction, float length)
		{
			this.origin = origin;
			this.direction = direction;
			this.length = length;
		}
	}

	class Camera
    {
		//FOV moet worden toegevoegd
		public Vector3 position;
		public Vector3 lookingDirection;
		public Vector3 upDirection;
		public Matrix4 viewSpace;
		public Vector3 scrnTL;
		public Vector3 scrnBR;

		public Camera()
		{
			position = new Vector3(0f, 0f, 0f);
			lookingDirection = new Vector3(0f, 0f, 1f);
			upDirection = new Vector3(0f, 1f, 0f);
			viewSpace = Matrix4.LookAt(position, lookingDirection, upDirection);
			scrnTL = new Vector3(-1, 1, 1);
			scrnBR = new Vector3(1, -1, 1);
		}
    }

	class Primitive
	{
		public Clr color;
	}

	class Sphere : Primitive
	{
		public Vector3 position;
		public float radius;

		public Sphere(Vector3 position, float radius, Clr color)
		{
			this.position = position;
			this.radius = radius;
			this.color = color;
		}
	}

	class Plane : Primitive
	{
		public Vector3 normal;
		public float distance;

		public Plane(Vector3 normal, float distance, Clr color)
		{
			this.normal = normal;
			this.distance = distance;
			this.color = color;
		}
	}

	class Light
    {
		public Vector3 position;
		public Color4 intensity; 

		public Light(Vector3 position, Color4 intensity)
        {
			this.position = position;
			this.intensity = intensity;
		}
    }

	class Scene 
	{
		public List<Primitive> primitives = new List<Primitive>();
		public List<Light> lights = new List<Light>();

		//Scene level intersect
	}
	class Intersection 
	{
		public Primitive nearestPrim;
		public Vector3 normal;

		public static Sphere IntersectSphere(Sphere sphere, Ray ray)
        {
			Vector3 center = sphere.position - ray.origin;
			float t = Vector3.Dot(center, ray.direction);
			Vector3 q = center - t * ray.direction;
			float p2 = q.LengthSquared;
			float r2 = (float)Math.Pow(sphere.radius, 2);
			if (p2 > r2) return null;
			t -= (float)Math.Sqrt(r2 - p2);
			if ((t > -1e-4f) && t < ray.length + 1e-4f)
            {
				ray.length = t;
				return sphere;
			}
			return null;
        }

		public static Plane IntersectPlane(Plane plane, Ray ray) 
		{
			float denom = Vector3.Dot(plane.normal, ray.direction);

			if (Math.Abs(denom) <= 1e-4f)
            {
				return null;
            }

			float t = -(float)(Vector3.Dot(plane.normal, ray.origin) + plane.distance) / Vector3.Dot(plane.normal, ray.direction);

			if (t <= 1e-4)
            {
				return null;
            }
			ray.length = (plane.normal * ray.direction).Length;
			return plane;
		}

		//normaallijn moet nog berekend worden bij een intersect
		//Intersect werkt niet als de ray in de sphere begint
		//Plane intersect moet nog worden gemaakt
	}
	class Raytracer 
	{
		MyApplication scene;
		Camera cam;
		Surface surface;

		public Raytracer(MyApplication scene, Camera cam, Surface surface)
        {
			this.scene = scene;
			this.cam = cam;
			this.surface = surface;
        }

		public Primitive CheckCollisions(Ray ray)
        {
			Primitive collidedPrimitive = null;
			foreach (Primitive primitive in scene.primitives)
			{
				Primitive currentPrimitive = null;
				if (primitive.GetType() == typeof(Sphere))
				{
					currentPrimitive = Intersection.IntersectSphere((Sphere)primitive, ray);
				}
				if (primitive.GetType() == typeof(Plane))
				{
					currentPrimitive = Intersection.IntersectPlane((Plane)primitive, ray);
				}
				if (currentPrimitive != null)
                {
					collidedPrimitive = currentPrimitive;
                }
			}
			return collidedPrimitive;
		}

		public void Render()
        {
			for (int y = 0; y < scene.screen.height; y++)
			{
				for (int x = 0; x < scene.screen.width; x++)
				{
					scene.ray.direction = Vector3.Normalize(
						new Vector3(cam.scrnTL.X - (cam.scrnTL.X - cam.scrnBR.X) * ((float)x / scene.screen.width),
						cam.scrnTL.Y - (cam.scrnTL.Y - cam.scrnBR.Y) * ((float)y / scene.screen.height), 
						cam.scrnTL.Z)
						);
					Primitive collidedPrimitive = CheckCollisions(scene.ray);
					
					if (collidedPrimitive != null)
                    {
						Vector3 intersectionPoint = scene.ray.origin + scene.ray.direction * scene.ray.length;
						scene.ray.length = 100;
						surface.pixels[x + y * scene.screen.width] = scene.MixColor(collidedPrimitive.color.red / 2, collidedPrimitive.color.green / 2, collidedPrimitive.color.blue / 2);
						foreach (Light light in scene.lights)
                        {
							Vector3 direction = Vector3.Normalize(new Vector3(light.position - intersectionPoint));
							float length = new Vector3(light.position - intersectionPoint).Length;
							Ray reflectionRay = new Ray(intersectionPoint, direction, length);
							CheckCollisions(reflectionRay);
							if (reflectionRay.length == length)
                            {
								surface.pixels[x + y * scene.screen.width] = scene.MixColor(collidedPrimitive.color.red, collidedPrimitive.color.green, collidedPrimitive.color.blue); ;
							}
						}
					}
				}
			}
		}
	}

	class Application 
	{ 

	}

	class Clr
    {
		public int red;
		public int green;
		public int blue;
		public Clr(int red, int green, int blue)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
		}
	}

	
}