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
			sphere1 = new Sphere(new Vector3(0, 0, 2f), .1f, new Material(new Color4(0, 0, 255, .9f), true));
			sphere2 = new Sphere(new Vector3(-1, 0, 1.5f), .1f, new Material(new Color4(255, 0, 0, .9f), true));
			sphere3 = new Sphere(new Vector3(2, 0, 2f), .4f, new Material(new Color4(0, 0, 255, .9f), true));
			plane1 = new Plane(new Vector3(0, 1, 0), -0.75f, new Material(new Color4(255, 0, 255, .9f), true, true));

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
		public Vector3 target;
		public Vector3 lookingDirection;
		public Vector3 up;
		public Vector3 upDirection;
		public Vector3 rightDirection;
		public Matrix4 viewSpace;
		public Vector3 scrnTL;
		public Vector3 scrnBR;
		public Vector3 planeCenter;

		public Camera()
		{
			position = new Vector3(0f, 0f, 0.5f);
			target = Vector3.Zero;
			lookingDirection = new Vector3(0f, 0f, 1f);
			//lookingDirection = Vector3.Normalize(position - target);
			up = Vector3.UnitY;
			rightDirection = Vector3.Normalize(Vector3.Cross(up, lookingDirection));
			upDirection = Vector3.Cross(lookingDirection, rightDirection);
			viewSpace = Matrix4.LookAt(position, target, up);

			planeCenter = position + 1f * lookingDirection; //1f = distance
			scrnTL = planeCenter + upDirection - rightDirection;
			scrnBR = planeCenter - upDirection + rightDirection;
		}

		public void Update()
        {
			//lookingDirection = Vector3.Normalize(position - target);
			rightDirection = Vector3.Normalize(Vector3.Cross(up, lookingDirection));
			upDirection = Vector3.Cross(lookingDirection, rightDirection);
			planeCenter = position + 1f * lookingDirection;
			scrnTL = planeCenter + upDirection - rightDirection;
			scrnBR = planeCenter - upDirection + rightDirection;
		}
    }

	class Primitive
	{
		public Material mat;

		public Color4 GetColor(float x, float z, Color4 color, float energy)
        {
			float u = x * 2f;
			float v = z * 2f;
			int i = ((int)u + (int)v) & 1;
			if (i == 0)
            {
				return GetColor(color, energy);
            }
            else
            {
				return GetColor(new Color4(128, 128, 128, 0), energy);
            }
        }
		public Color4 GetColor(Color4 color, float energy)
        {
			return new Color4(color.R * energy, color.G * energy, color.B * energy, 0);
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
		public Color4 color;

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
			float t = (Vector3.Dot(plane.normal, new Vector3(plane.distance) - ray.origin)) / (Vector3.Dot(plane.normal, ray.direction));
			if (t < 0.0001) // the ray does not hit the surface, that is, the surface is "behind" the ray
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
			if (intersection.nearestPrim != null)
            {
				intersection.color = intersection.nearestPrim.mat.color;
				if (intersection.nearestPrim.mat.isMirror) 
                {
					Ray reflectionRay = new Ray(Vector3.Zero, Vector3.Zero, MainScene.rayLength); //if the material is reflective, create a new ray
					reflectionRay.direction = ray.direction - 2 * Vector3.Dot(ray.direction , Vector3.Normalize(intersection.normal)) * Vector3.Normalize(intersection.normal);
					Intersection reflectionIntersection = CheckCollisions(reflectionRay);
					if (reflectionIntersection.nearestPrim != null)
                    {
                        if (reflectionIntersection.nearestPrim.mat.isCheckered)
                        {
							Vector3 intersectionPoint = reflectionRay.origin + reflectionRay.direction * reflectionRay.length;
							reflectionIntersection.color = reflectionIntersection.nearestPrim.GetColor(intersectionPoint.X, intersectionPoint.Z, reflectionIntersection.color, 1);
                        }
						float alpha = 1 / reflectionRay.length;
						intersection.color.R = intersection.color.R * (1 - alpha) + reflectionIntersection.color.R * alpha;
						intersection.color.G = intersection.color.G * (1 - alpha) + reflectionIntersection.color.G * alpha;
						intersection.color.B = intersection.color.B * (1 - alpha) + reflectionIntersection.color.B * alpha;
					}
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
								red = Math.Min(light.intensity.R + red, intersection1.color.R);
								green = Math.Min(light.intensity.G + green, intersection1.color.G);
								blue = Math.Min(light.intensity.B + blue, intersection1.color.B);
							}
						}
						reflectedEnergy = Math.Min(reflectedEnergy, 1);
						Color4 color;
						if (!intersection1.nearestPrim.mat.isCheckered)
						{
							color = intersection1.nearestPrim.GetColor(new Color4(red, green, blue, 0), reflectedEnergy);
						}
						else
						{
							 color = intersection1.nearestPrim.GetColor(intersectionPoint.X, intersectionPoint.Z, new Color4(red, green, blue, 0), reflectedEnergy);
						}
						surface.pixels[x + y * surface.width] = MainScene.MixColor((int)color.R, (int)color.G, (int)color.B);
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
							MainScene.MixColor( (int)prim.mat.color.R, (int)prim.mat.color.G, (int)prim.mat.color.B));
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
		public bool isMirror;
		public bool isCheckered;
		public Color4 color;
		public Material(Color4 color, bool isMirror = false, bool isCheckered = false)
		{
			this.isMirror = isMirror;
			this.isCheckered = isCheckered;
			this.color = color;
		}
	}
}