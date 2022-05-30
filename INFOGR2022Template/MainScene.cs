using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Forms;

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
		public Camera camera;
		public Ray ray;
		public Raytracer raytracer;
		public List<Primitive> primitives;
		Light light1;
		Light light2;
		public List<Light> lights;
		public Application app;
		public Scene scene;

		public const int rayLength = 5000;
		// initialize
		public void Init()
		{
			scene = new Scene();
			
			primitives = new List<Primitive>();
			sphere1 = new Sphere(new Vector3(0, 0, 2f), .1f, new Material(0, 0, 255));
			sphere2 = new Sphere(new Vector3(-1, 0, 1.5f), .1f, new Material(255, 0, 0));
			sphere3 = new Sphere(new Vector3(2, 0, 2f), .4f, new Material(0, 0, 255));
			plane1 = new Plane(new Vector3(0, 1, 0), -1, new Material(255, 0, 255, false, true));

			scene.primitives.Add(sphere1);
			scene.primitives.Add(sphere2);
			scene.primitives.Add(sphere3);
			scene.primitives.Add(plane1);

			light1 = new Light(new Vector3(.2f, 1f, 2f), new Color4(255, 255, 255, 2f));
			light2 = new Light(new Vector3(-.9f, -.5f, 2f), new Color4(255, 255, 255, 1f));
			lights = new List<Light>();

			scene.lights.Add(light1);
			scene.lights.Add(light2);

			camera = new Camera();
			ray = new Ray(Vector3.Zero, Vector3.One, rayLength);
			
			raytracer = new Raytracer(scene, camera, screen);

			
			

			//app = new Application(raytracer);
			
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

		public int GetColor(float u, float v, float red, float green, float blue, float energy)
        {
			int color = ((int)u + (int)v) & 1;
			if (color == 0)
            {
				return GetColor(red, green, blue, energy);
            }
            else
            {
				return GetColor(128, 128, 128, energy);
            }
        }
		public int GetColor(float red, float green, float blue, float energy)
        {
			return MainScene.MixColor((int)(red * energy), (int)(green * energy), (int)(blue * energy));
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
			float t = -(Vector3.Dot(plane.normal, new Vector3(-plane.distance) - ray.origin)) / (Vector3.Dot(plane.normal, ray.direction));
			if (t < 0) // the ray does not hit the surface, that is, the surface is "behind" the ray
				return;

			if (t < ray.length)
            {
				ray.length = t;
				nearestPrim = plane;
				normal = plane.normal;
			}
		}

		//normaallijn moet nog berekend worden bij een intersect
		//Intersect werkt niet als de ray in de sphere begint
		//Plane intersect moet nog worden gemaakt
		//Distance werkt niet vgm
	}
	class Raytracer
	{
		public Scene scene;
		public Camera cam;
		public Surface surface;

		public Raytracer(Scene scene, Camera cam, Surface surface)
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
			for (int y = 0; y < surface.height; y++)
			{
				for (int x = surface.width - surface.width / 2; x < surface.width; x++) //2e helft van het scherm tot eind
				{
					Ray ray = new Ray(cam.position, cam.lookingDirection, 5000);
						ray.direction = Vector3.Normalize(
						new Vector3(cam.scrnTL.X - (cam.scrnTL.X - cam.scrnBR.X) * (((float)x - surface.width / 2) / (surface.width / 2)),
						cam.scrnTL.Y - (cam.scrnTL.Y - cam.scrnBR.Y) * ((float)y / surface.height),
						cam.scrnTL.Z)
						); //find ray direction
					Intersection intersection1 = CheckCollisions(ray); //check if ray intersects with an object in the scene

					if (intersection1.nearestPrim != null)
					{
						Vector3 intersectionPoint = ray.origin + ray.direction * ray.length;
						float reflectedEnergy = .2f;
						float red = 0;
						float green = 0;
						float blue = 0;
						foreach (Light light in scene.lights)
						{
							Vector3 direction = Vector3.Normalize(new Vector3(light.position - intersectionPoint));
							float length = new Vector3(light.position - intersectionPoint).Length;
							Ray reflectionRay = new Ray(intersectionPoint, direction, length);
							Intersection intersection2 = CheckCollisions(reflectionRay);
							if(intersection2.nearestPrim == null)
                            {
								reflectedEnergy += Math.Max(0, 1 / (float)Math.Pow(length, 2) * light.intensity.A * Vector3.Dot(Vector3.Normalize(intersection1.normal), direction));
								red = Math.Min(light.intensity.R + red, intersection1.nearestPrim.mat.red);
								green = Math.Min(light.intensity.G + green, intersection1.nearestPrim.mat.green);
								blue = Math.Min(light.intensity.B + blue, intersection1.nearestPrim.mat.blue);
							}
						}
						reflectedEnergy = Math.Min(reflectedEnergy, 1);
						if (!intersection1.nearestPrim.mat.isCheckered)
						{
							surface.pixels[x + y * surface.width] = intersection1.nearestPrim.GetColor(red, green, blue, reflectedEnergy);
						}
						else
						{
							surface.pixels[x + y * surface.width] = intersection1.nearestPrim.GetColor(intersectionPoint.X, intersectionPoint.Z, red, green, blue, reflectedEnergy);
						}
					}
					//DEBUG OUTPUT:
					//Project rays
					if (ray.direction.Y == 0)
					{
						if (x % 30 == 0)
						{
							float raylength = ray.length; //5000
							if (ray.origin.X + raylength * ray.direction.X > 2) 
								raylength = (2 - ray.origin.X) / ray.direction.X; 
							if (ray.origin.Z + raylength * ray.direction.Z > 3) //3 verwijst naar het coordinatensysteem van TY, dat kan netter
								raylength = (3 - ray.origin.Z) / ray.direction.Z;

							surface.Line(
								TX(ray.origin.X), 
								TY(ray.origin.Z), 
								TX(ray.origin.X + raylength * ray.direction.X), 
								TY(ray.origin.Z + raylength * ray.direction.Z),
								MainScene.MixColor(255, 255,0));
						}

					}
					ray.length = MainScene.rayLength;
				}
			}
			//MORE DEBUG OUTPUT
			//Project camera position
			surface.Box(TX(cam.position.X), TY(cam.position.Z), TX(cam.position.X), TY(cam.position.Z), 255);
			//Project screen plane
			surface.Line(TX(cam.scrnTL.X), TY(cam.scrnTL.Z), TX(cam.scrnBR.X), TY(cam.scrnBR.Z), MainScene.MixColor(255,255,255));

			//Project spheres
			float pistep = (float)Math.PI / 100;
			foreach (Primitive prim in scene.primitives)
			{
				if (prim is Sphere)
				{
					for (float i = 0; i < 2 * Math.PI; i = i + pistep)
					{
						if (TX((float)(((Sphere)prim).position.X + Math.Cos(i) * ((Sphere)prim).radius)) < (surface.width / 2))
						{
							surface.Line(
							TX((float)(((Sphere)prim).position.X + Math.Cos(i) * ((Sphere)prim).radius)),
							TY((float)(((Sphere)prim).position.Z + Math.Sin(i) * ((Sphere)prim).radius)),
							TX((float)(((Sphere)prim).position.X + Math.Cos(i + pistep) * ((Sphere)prim).radius)),
							TY((float)(((Sphere)prim).position.Z + Math.Sin(i + pistep) * ((Sphere)prim).radius)),
							MainScene.MixColor( (int)prim.mat.red, (int)prim.mat.green, (int)prim.mat.blue));
						}
					}
				}
			}
		}

		public int TX(float point)
		{
			float shift = point + 2; //make -2:2 into 0:4
			float scale = (shift * (surface.width/2))/4; //make 0:4 into 0:512
			return Convert.ToInt32(scale);
		}

		public int TY(float point)
		{
			float invert = -point;
			float shift = invert + 3;
			float scale = shift * (surface.width / 8);
			
			//4*160;4*100
			return Convert.ToInt32(scale);
		}

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