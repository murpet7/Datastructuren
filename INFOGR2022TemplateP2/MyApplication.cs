using System.Diagnostics;
using OpenTK;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
		Entity scene, teapot1, teapot2, teapot3, floor;
		const float PI = 3.1415926535f;         // PI
		float a = 0;                            // teapot rotation angle
		Stopwatch timer;                        // timer for measuring frame duration
		public Shader shader;                          // shader to use for rendering
		public Shader postproc;                        // shader to use for post processing
		public Texture wood, metal, checker, stone;    // texture to use for rendering
		public RenderTarget target;                    // intermediate render target
		public ScreenQuad quad;                        // screen filling quad for post processing
		Light light;
		public bool useRenderTarget = true;

		// initialize
		public void Init()
		{
			// initialize stopwatch
			timer = new Stopwatch();
			timer.Reset();
			timer.Start();
			// create light
			light = new Light(new Vector3(1f,1f,1f), new Vector3(20.0f, 5.0f, 0.0f));
			// create shaders
			shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl", light);
			postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl", light);
			// load a texture
			wood = new Texture("../../assets/wood.jpg");
			metal = new Texture("../../assets/metal1.jpg");
			checker = new Texture("../../assets/checker.jpg");
			stone = new Texture("../../assets/stone.jpg");
			// create the render target
			target = new RenderTarget(screen.width, screen.height);
			quad = new ScreenQuad();

			// create entities
			scene = new Entity(null);
			teapot1 = new Entity(new Mesh("../../assets/teapot.obj", Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), wood), scene);
			teapot2 = new Entity(new Mesh("../../assets/teacup.obj", new Vector3(15, 0, 0), Vector3.Zero, new Vector3(1, 1, 1), metal), teapot1);
			teapot3 = new Entity(new Mesh("../../assets/eyeball.obj", new Vector3(30, 0, 0), Vector3.Zero, new Vector3(1, 1, 1), stone), teapot2);
			floor = new Entity(new Mesh("../../assets/floor.obj", new Vector3(0, 1, 0), Vector3.Zero, new Vector3(5), checker), scene);
		}

		// tick for background surface
		public void Tick()
		{
			screen.Clear(0);
			screen.Print("hello world", 2, 2, 0xffff00);
		}

		// tick for OpenGL rendering code
		public void RenderGL()
		{
			// measure frame duration
			float frameDuration = timer.ElapsedMilliseconds;
			timer.Reset();
			timer.Start();

			teapot1.mesh.rot = new Vector3(0, a, 0);
			teapot2.mesh.rot = new Vector3(0, a, 0);
			teapot3.mesh.rot = new Vector3(0, a, 0);

			a += 0.001f * frameDuration;
			
			if (useRenderTarget)
            {
				// enable render target
				target.Bind();
			}

			SceneGraph.Render(scene, this);

			if (useRenderTarget)
            {
				// render quad
				target.Unbind();
				quad.Render(postproc, target.GetTextureID());
			}

			if( a > 2 * PI ) a -= 2 * PI;
		}
	}

	public static class Camera
	{
		//Camera position and sky position
		public static Vector3 position = new Vector3(0, 0, 9f);
		public static Vector3 target = Vector3.Zero;
		public static Vector3 up = Vector3.UnitY;
		public static Vector3 front = new Vector3(0f, 0f, -1f);

		//Looking direction variables
		public static float pitch;
		public static float yaw = -90;
		public static Matrix4 view = Matrix4.LookAt(position, front, up);

	}

	public class Light
    {
		public Vector3 position;
		public Vector3 color;

		public Light(Vector3 color, Vector3 position) //initialise light object
        {
			this.color = color;
			this.position = position;
        }
    }
}