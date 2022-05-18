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

		Primitive.Sphere sphere1;
		Primitive.Sphere sphere2;
		Primitive.Sphere sphere3;
		Camera camera;
		Ray ray;
		Intersection intersection;
		List<Primitive> primitives;
		// initialize
		public void Init()
		{
			sphere1 = new Primitive.Sphere(new Vector3(-.5f, .5f, 0), .3f);
			sphere2 = new Primitive.Sphere(new Vector3(0, .5f, 0), .3f);
			sphere3 = new Primitive.Sphere(new Vector3(.5f, .5f, 0), .3f);
			primitives.Add(sphere1);
			primitives.Add(sphere2);
			primitives.Add(sphere3);
			camera = new Camera();
			ray = new Ray(Vector3.Zero, Vector3.One, 100);
			intersection = new Intersection();
		}
		// tick: renders one frame
		public void Tick()
		{
			screen.Clear( 0 );

			for(int i = 0; i < 8; i++)
            {
				ray.direction = new Vector3(-1 + i / 4, 1, 0);
				foreach(Primitive primitive in primitives)
                {
					if(primitive.GetType() == typeof(Sphere))
                    {
						intersection.Intersect((Sphere)primitive, ray);
                    }
                }
            }

			screen.Line( 2, 20, 160, 20, 0xff0000 );
		}

		int TX(float point)
		{
			float shift = point + 2;
			float scale = shift * (screen.width / 4);
			return Convert.ToInt32(scale);
		}

		int TY(float point)
		{
			float invert = -point;
			float scale = invert * (screen.width / 4);
			float shift = scale + (screen.height / 2);
			//4*160;4*100
			return Convert.ToInt32(shift);
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

		public Camera()
		{
			position = new Vector3(0f, 0f, 0f);
			lookingDirection = new Vector3(0f, 0f, 1f);
			upDirection = new Vector3(0f, 1f, 0f);
			viewSpace = Matrix4.LookAt(position, lookingDirection, upDirection);
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

		public void Intersect(Primitive.Sphere sphere, Ray ray)
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

		public void Intersect(Primitive.Plane plane, Ray ray) 
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
	class Raytracer { }

	class Application { }

}