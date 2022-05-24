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
		Camera camera;
		public Ray ray;
		Raytracer raytracer;
		public List<Primitive> primitives;
		// initialize
		public void Init()
		{
			primitives = new List<Primitive>();
			sphere1 = new Sphere(new Vector3(-1, .5f, 5f), 1.1f);
			sphere2 = new Sphere(new Vector3(0, -1, 1.5f), .2f);
			sphere3 = new Sphere(new Vector3(0, 0, 2f), .4f);
			primitives.Add(sphere1);
			primitives.Add(sphere2);
			primitives.Add(sphere3);
			camera = new Camera();
			ray = new Ray(new Vector3(0, -1, 0), Vector3.One, 100);
			raytracer = new Raytracer(this, camera, screen);
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

		void DrawCircle(float cx, float cy, float r, int num_segments)
		{
			for (int i = 0; i < num_segments; i++)
			{
				float theta = 2f * (float)Math.PI * (float)i / (float)num_segments;//get the current angle 
				float x = r * (float)Math.Cos(theta);//calculate the x component 
				float y = r * (float)Math.Sin(theta);//calculate the y component 
				screen.Line(TX(cx), TY(cy), TX(x + cx), TY(y + cy), 0xff0000);
			}
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

	class Primitive{}

	class Sphere : Primitive
	{
		public Vector3 position;
		public float radius;

		public Sphere(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
		
	}

	class Plane : Primitive
	{
		public Vector3 normal;
		public float distance;

		public Plane(Vector3 normal, float distance)
		{
			this.normal = normal;
			this.distance = distance;
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

		public static void IntersectSphere(Sphere sphere, Ray ray)
        {
			Vector3 center = sphere.position - ray.origin;
			float t = Vector3.Dot(center, ray.direction);
			Vector3 q = center - t * ray.direction;
			float p2 = q.LengthSquared;
			float r2 = (float)Math.Pow(sphere.radius, 2);
			if (p2 > r2) return;
			t -= (float)Math.Sqrt(r2 - p2);
			if ((t > 0) && t < (ray.length))
				ray.length = t;
        }

		public static void IntersectPlane(Plane plane, Ray ray) 
		{
			Vector3 p0 = ray.origin + plane.distance * plane.normal;
			float t = Vector3.Dot(plane.normal, ray.direction);
			if (t > 0.0001f) //epsilon
			{
				
			}
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
					foreach (Primitive primitive in scene.primitives)
					{
						if (primitive.GetType() == typeof(Sphere))
						{
							Intersection.IntersectSphere((Sphere)primitive, scene.ray);
						}
					}
					if (scene.ray.length < 100)
                    {
						surface.pixels[x + y * scene.screen.width] = (int)(255 - scene.ray.length * scene.ray.length * 30);
					}
					scene.ray.length = 100;
				}
			}
		}
	}

	class Application 
	{ 

	}

}