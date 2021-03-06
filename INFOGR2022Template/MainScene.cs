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
		public Camera camera;
		public Raytracer raytracer;
		public Scene scene;

		//declare primitives
		Sphere sphere1;
		Sphere sphere2;
		Sphere sphere3;
		Plane plane1;
		Triangle triangle1;

		//declare lights
		Light light1;
		Light light2;
		
		//set standard raylength (for debugger)
		public const int rayLength = 5000;
		// initialize
		public void Init()
		{
			scene = new Scene();
			camera = new Camera();

			sphere1 = new Sphere(new Vector3(0, 0, 2f), .2f, new Material(new Color4(0, 0, 255, .9f), true));
			sphere2 = new Sphere(new Vector3(-1, 0, 1.5f), .2f, new Material(new Color4(255, 0, 0, .9f), true));
			sphere3 = new Sphere(new Vector3(2, 0, 2f), .2f, new Material(new Color4(0, 0, 255, .9f), true));
			plane1 = new Plane(new Vector3(0, 1, 0), -.2f, new Material(new Color4(255, 255, 255, .9f), true, true));
			triangle1 = new Triangle(new Vector3(.2f, 0, 2f), new Vector3(.2f, .2f, 2f), new Vector3(0, 0, 2f), new Material(new Color4(255, 0, 255, .9f)));

			scene.primitives.Add(sphere1);
			scene.primitives.Add(sphere2);
			scene.primitives.Add(sphere3);
			scene.primitives.Add(plane1);
			scene.primitives.Add(triangle1);

			light1 = new Light(new Vector3(.2f, 1f, 2f), new Color4(255, 255, 255, 2f));
			light2 = new Light(new Vector3(-.9f, -.5f, 2f), new Color4(255, 255, 255, 1f));

			scene.lights.Add(light1);
			scene.lights.Add(light2);

			//initialise the raytracer
			raytracer = new Raytracer(scene, camera, screen);
		}
		// tick: renders one frame
		public void Tick()
		{
			screen.Clear( 0 );
			raytracer.Render(); //render on every tick
		}

		public int TX(float point) //To transform the X axis for the debugger
		{
			float shift = point + 2;
			float scale = shift * (screen.width / 4);
			return Convert.ToInt32(scale);
		}

		public int TY(float point) //To transform the Y axis for the debugger
		{
			float invert = -point;
			float scale = invert * (screen.width / 4);
			float shift = scale + (screen.height / 2);
			return Convert.ToInt32(shift);
		}
		public static int MixColor(int red, int green, int blue) //To mix RGB values
		{
			return (red << 16) + (green << 8) + blue;
		}
	}
	class Ray
	{
		public Vector3 origin;
		public Vector3 direction;
		public float length;

		public Ray(Vector3 origin, Vector3 direction, float length) //Initialise Ray object
		{
			this.origin = origin;
			this.direction = direction;
			this.length = length;
		}
	}

	class Camera
    {
		//Pitch & yaw, used for mouse movement
		public float yaw = 90;
		public float pitch;

		//Camera position and sky position
		public Vector3 position;
		public Vector3 up;
		
		//Camera directions
		public Vector3 lookingDirection;
		public Vector3 upDirection;
		public Vector3 rightDirection;

		//Plane Position, Top Left and Bottom right
		public Vector3 planeCenter;
		public Vector3 scrnTL;
		public Vector3 scrnBR;
		

		//field of view
		public float fov = 1f;

		public Camera() //Initialise and compute standard camera values
		{
			up = Vector3.UnitY;
			position = new Vector3(0f, 0f, 1f);
			lookingDirection = new Vector3(0f, 0f, 1f);
			rightDirection = Vector3.Normalize(Vector3.Cross(up, lookingDirection));
			upDirection = Vector3.Cross(lookingDirection, rightDirection);

			planeCenter = position + fov * lookingDirection;
			scrnTL = planeCenter + upDirection - rightDirection;
			scrnBR = planeCenter - upDirection + rightDirection;
		}

		public void Update() //Update camera values
        {
			rightDirection = Vector3.Normalize(Vector3.Cross(up, lookingDirection));
			upDirection = Vector3.Normalize(Vector3.Cross(lookingDirection, rightDirection));
			planeCenter = position + fov * lookingDirection;
			scrnTL = planeCenter + upDirection - rightDirection;
			scrnBR = planeCenter - upDirection + rightDirection;
		}
    }

	class Primitive
	{
		public Material mat; //stores material properties

		public Color4 GetColor(float x, float z, Color4 color, float energy)
        {
			float u = x * 6f;
			float v = z * 6f;
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

	class Sphere : Primitive //stores sphere properties
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

	class Plane : Primitive //stores plane properties
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
	class Triangle : Primitive //stores triangle properties
	{
		public Vector3 p0;
		public Vector3 p1;
		public Vector3 p2;

		public Triangle(Vector3 p0, Vector3 p1, Vector3 p2, Material color)
		{
			this.p0 = p0;
			this.p1 = p1;
			this.p2 = p2;
			this.mat = color;
		}
	}

	class Light //stores light properties
    {
		public Vector3 position;
		public Color4 intensity; 

		public Light(Vector3 position, Color4 intensity)
        {
			this.position = position;
			this.intensity = intensity;
		}
    }

	class Scene //stores all primitives and lights in the scene
	{
		public List<Primitive> primitives = new List<Primitive>();
		public List<Light> lights = new List<Light>();

		//Scene level intersect
	}
	class Intersection //used to calculate and store intersections of rays and primitives
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
		public void IntersectTriangle(Ray ray, Triangle triangle)
		{
			float hit;
			Vector3 barycentricCoord;
			Vector3 triangleNormal;
			Vector3 e0 = triangle.p1 - triangle.p0;
			Vector3 e1 = triangle.p0 - triangle.p2;
			triangleNormal = Vector3.Cross(e1, e0);

			Vector3 e2 = (1f / (float)Vector3.Dot(triangleNormal, ray.direction)) * (triangle.p0 - ray.origin);
			Vector3 i = Vector3.Cross(ray.direction, e2);

			barycentricCoord.Y = Vector3.Dot(i, e1);
			barycentricCoord.Z = Vector3.Dot(i, e0);
			barycentricCoord.X = 1f - (barycentricCoord.Z + barycentricCoord.Y);
			hit = Vector3.Dot(triangleNormal, e2);

			if ((hit > 0.0001f) && barycentricCoord.X > 0 && barycentricCoord.Y > 0 && barycentricCoord.Z > 0)
			{
				ray.length = hit;
				nearestPrim = triangle;
				normal = triangleNormal;
			}
		}

	}
	class Raytracer 
	{
		public Scene scene;
		public Camera cam;
		public Surface surface;
		public Vector3 p0;
		public Vector3 p1;
		public Vector3 p2;
		public Vector3 u;
		public Vector3 v;
		public Raytracer(Scene scene, Camera cam, Surface surface)
		{
			this.scene = scene;
			this.cam = cam;
			this.surface = surface;
		}

		public Intersection CheckCollisions(Ray ray, int i) //used to encapsulate the different types of intersections
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
				if (primitive.GetType() == typeof(Triangle))
                {
					intersection.IntersectTriangle(ray, (Triangle)primitive);
				}
			}
			if (intersection.nearestPrim != null)
            {
				intersection.color = intersection.nearestPrim.mat.color;
				if (intersection.nearestPrim.mat.isMirror) 
                {
					Ray reflectionRay = new Ray(Vector3.Zero, Vector3.Zero, MainScene.rayLength); //if the material is reflective, create a new ray
					reflectionRay.direction = ray.direction - 2 * Vector3.Dot(ray.direction , Vector3.Normalize(intersection.normal)) * Vector3.Normalize(intersection.normal);
					reflectionRay.origin = ray.origin + ray.direction * ray.length;
					Intersection reflectionIntersection = CheckCollisions(reflectionRay, i - 1);
					if (reflectionIntersection.nearestPrim != null && i > 0)
                    {
                        if (reflectionIntersection.nearestPrim.mat.isCheckered)
                        {
							Vector3 intersectionPoint = reflectionRay.origin + reflectionRay.direction * reflectionRay.length;
							reflectionIntersection.color = reflectionIntersection.nearestPrim.GetColor(intersectionPoint.X, intersectionPoint.Z, reflectionIntersection.color, 1);
                        }
						float alpha = Math.Min(1, 1 / (float)Math.Pow(reflectionRay.length, 2)) * 0.4f;
						intersection.color.R = intersection.color.R * (1 - alpha) + reflectionIntersection.color.R * alpha; //find new colors
						intersection.color.G = intersection.color.G * (1 - alpha) + reflectionIntersection.color.G * alpha;
						intersection.color.B = intersection.color.B * (1 - alpha) + reflectionIntersection.color.B * alpha;
					}
                }
            }
			return intersection;
		}
		public void Render() //Renders the pixels on the plane surface
		{
			p0 = cam.planeCenter + cam.upDirection - cam.rightDirection;
			p1 = cam.planeCenter + cam.upDirection + cam.rightDirection;
			p2 = cam.planeCenter - cam.upDirection - cam.rightDirection;
			u = p1 - p0;
			v = p2 - p0;
			float a;
			float b;

			for (int y = 0; y < surface.height; y++)
			{
				for (int x = surface.width - surface.width / 2; x < surface.width; x++) //Renders on second half of the screen
				{
					a = (float)(x - surface.width / 2) / (float)(surface.width / 2); //a = [0:1]
					b = (float)y / (float)surface.height; //b = [0:1]

					Vector3 screenPoint = p0 + (a * u) + (b * v); //Calculate point on the plane of the current ray
					Ray ray = new Ray(cam.position, Vector3.Normalize(screenPoint - cam.position), MainScene.rayLength); //calculate the direction of the ray going through the point in the plane


					Intersection intersection1 = CheckCollisions(ray, 7); //check if ray intersects with an object in the scene

					if (intersection1.nearestPrim != null) //If there is an intersection: 
					{
						Vector3 intersectionPoint = ray.origin + ray.direction * ray.length;
						float reflectedEnergy = .2f;
						float red = 0;
						float green = 0;
						float blue = 0;
						foreach (Light light in scene.lights) //Calculate lighting on primitives
						{
							Vector3 direction = Vector3.Normalize(new Vector3(light.position - intersectionPoint));
							float length = new Vector3(light.position - intersectionPoint).Length;
							Ray reflectionRay = new Ray(intersectionPoint, direction, length);
							Intersection intersection2 = CheckCollisions(reflectionRay, 1);
							if(intersection2.nearestPrim == null)
                            {
								reflectedEnergy += Math.Max(0, 1 / (float)Math.Pow(length, 2) * light.intensity.A * Vector3.Dot(Vector3.Normalize(intersection1.normal), direction)); //find amount of energy reflected
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
					if (ray.direction.Y < 0.001 && ray.direction.Y > -0.001)
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

	class Material //Stores material properties
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