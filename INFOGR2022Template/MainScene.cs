using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;

namespace Template
{
	class MainScene
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

		public const int rayLength = int.MaxValue;
		// initialize
		public void Init()
		{
			primitives = new List<Primitive>();
			sphere1 = new Sphere(new Vector3(0, .1f, 1f), .1f, new Material(0, 0, 255));
			sphere2 = new Sphere(new Vector3(-1, 1, 1.5f), .1f, new Material(255, 0, 0));
			sphere3 = new Sphere(new Vector3(-1, 1, 2f), .4f, new Material(0, 0, 255));
			plane1 = new Plane(new Vector3(0, 1, 0), 2, new Material(255, 0, 255, false, false));
			primitives.Add(sphere1);
			//primitives.Add(sphere2);
			//primitives.Add(sphere3);
			primitives.Add(plane1);
			camera = new Camera();
			ray = new Ray(Vector3.Zero, Vector3.One, rayLength);
			raytracer = new Raytracer(this, camera, screen);
			light1 = new Light(new Vector3(1f, 1f, .5f), new Color4(1f, 0, 0, 1f));
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
		public static int MixColor(int red, int green, int blue)
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
		public Material mat;

		public int GetColor(float u, float v)
        {
			int color = ((int)u + (int)v) & 1;
			if (color == 0)
            {
				return MainScene.MixColor(255, 255, 255);
            }
            else
            {
				return 0;
            }
        }
		public int GetColor(float energy, Intersection intersection)
        {
			return MainScene.MixColor((int)(intersection.nearestPrim.mat.red * energy), (int)(intersection.nearestPrim.mat.green * energy), (int)(intersection.nearestPrim.mat.blue * energy));
		}
	}

	class Sphere : Primitive
	{
		public Vector3 position;
		public float radius;

		public Sphere(Vector3 position, float radius, Material mat)
		{
			this.position = position;
			this.radius = radius;
			this.mat = mat;
		}
	}

	class Plane : Primitive
	{
		public Vector3 normal;
		public float distance;

		public Plane(Vector3 normal, float distance, Material color)
		{
			this.normal = normal;
			this.distance = distance;
			this.mat = color;
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

		public void IntersectSphere(Sphere sphere, Ray ray)
        {
			Vector3 center = sphere.position - ray.origin;
			float t = Vector3.Dot(center, ray.direction);
			Vector3 q = center - t * ray.direction;
			float p2 = q.LengthSquared;
			float r2 = (float)Math.Pow(sphere.radius, 2);
			if (p2 > r2) return;
			t -= (float)Math.Sqrt(r2 - p2);
			if ((t > 1e-4f) && t < ray.length - 1e-4f)
            {
				ray.length = t;
				nearestPrim = sphere;
				normal = (ray.origin + ray.length * ray.direction) - sphere.position;
			}
        }

		public void IntersectPlane(Plane plane, Ray ray) 
		{
			float denom = Vector3.Dot(plane.normal, ray.direction);

			if (Math.Abs(denom) <= 1e-4f)
            {
				return;
            }

			float t = -(float)(Vector3.Dot(plane.normal, ray.origin) + plane.distance) / Vector3.Dot(plane.normal, ray.direction);

			if (t <= 1e-4)
            {
				return;
            }
			ray.length = (plane.normal * ray.direction).Length;
			nearestPrim = plane;
			normal = plane.normal;
		}

		//normaallijn moet nog berekend worden bij een intersect
		//Intersect werkt niet als de ray in de sphere begint
		//Plane intersect moet nog worden gemaakt
	}
	class Raytracer 
	{
		MainScene scene;
		Camera cam;
		Surface surface;

		public Raytracer(MainScene scene, Camera cam, Surface surface)
        {
			this.scene = scene;
			this.cam = cam;
			this.surface = surface;
        }

		public Intersection CheckCollisions(Ray ray)
        {
			Intersection intersection = new Intersection();
			foreach (Primitive primitive in scene.primitives)
			{
				if (primitive.GetType() == typeof(Sphere))
				{
					intersection.IntersectSphere((Sphere)primitive, ray);
				}
				if (primitive.GetType() == typeof(Plane))
				{
					intersection.IntersectPlane((Plane)primitive, ray);
				}
			}
			return intersection;
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
					Intersection intersection1 = CheckCollisions(scene.ray);
					
					if (intersection1.nearestPrim != null)
                    {
						Vector3 intersectionPoint = scene.ray.origin + scene.ray.direction * scene.ray.length;
						foreach (Light light in scene.lights)
                        {
							Vector3 direction = Vector3.Normalize(new Vector3(light.position - intersectionPoint));
							float length = new Vector3(light.position - intersectionPoint).Length;
							Ray reflectionRay = new Ray(intersectionPoint, direction, length);
							Intersection intersection2 = CheckCollisions(reflectionRay);
							
							float reflectedEnergy = Math.Min(1, Math.Max(0.1f, 1 / (float)Math.Pow(length, 2) * 1f * Vector3.Dot(Vector3.Normalize(intersection1.normal), direction)));

                            if (!intersection1.nearestPrim.mat.isCheckered)
                            {
								surface.pixels[x + y * scene.screen.width] = intersection1.nearestPrim.GetColor(reflectedEnergy, intersection1);
							}
                            else
                            {
								surface.pixels[x + y * scene.screen.width] = intersection1.nearestPrim.GetColor(intersectionPoint.X, intersectionPoint.Y);
							}
							
							scene.ray.length = MainScene.rayLength;
						}
					}
				}
			}
		}
	}

	class Application 
	{ 

	}

	class Material
    {
		public float red;
		public float green;
		public float blue;
		public bool isMirror;
		public bool isCheckered;
		public Material(float red, float green, float blue, bool isMirror = false, bool isCheckered = false)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
			this.isMirror = isMirror;
			this.isCheckered = isCheckered;
		}
	}
}