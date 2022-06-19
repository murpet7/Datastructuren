using System.Diagnostics;
using OpenTK;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
		Entity scene, teapot, floor;
		const float PI = 3.1415926535f;         // PI
		float a = 0;                            // teapot rotation angle
		Stopwatch timer;                        // timer for measuring frame duration
		public Shader shader;                          // shader to use for rendering
		public Shader postproc;                        // shader to use for post processing
		public Texture wood;                           // texture to use for rendering
		public RenderTarget target;                    // intermediate render target
		public ScreenQuad quad;                        // screen filling quad for post processing
		public bool useRenderTarget = true;


		// initialize
		public void Init()
		{
			// initialize stopwatch
			timer = new Stopwatch();
			timer.Reset();
			timer.Start();
			// create shaders
			shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
			postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
			// load a texture
			wood = new Texture( "../../assets/wood.jpg" );
			// create the render target
			target = new RenderTarget( screen.width, screen.height );
			quad = new ScreenQuad();

			scene = new Entity(null);
			teapot = new Entity(new Mesh("../../assets/teapot.obj", Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), wood), scene);
			floor = new Entity(new Mesh("../../assets/floor.obj", new Vector3(0, 1, 0), Vector3.Zero, new Vector3(4, 1, 0.5f), wood), teapot);
		}

		// tick for background surface
		public void Tick()
		{
			screen.Clear( 0 );
			screen.Print( "hello world", 2, 2, 0xffff00 );
		}

		// tick for OpenGL rendering code
		public void RenderGL()
		{
			// measure frame duration
			float frameDuration = timer.ElapsedMilliseconds;
			timer.Reset();
			timer.Start();

			teapot.mesh.rot = new Vector3(0, a, 0);

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

			//Matrix4 Tpot = Matrix4.CreateScale( 0.5f ) * Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );
			//Matrix4 Tfloor = Matrix4.CreateScale( 4.0f ) * Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );

			
			
			//Matrix4 Tcamera = Matrix4.CreateTranslation( new Vector3(0, -10.5f, -10f ) ) * Matrix4.CreateFromAxisAngle( new Vector3( 1, 0, 0 ), angle90degrees );
			

			// update rotation
			//a += 0.001f * frameDuration;
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
		public static Vector3 lookingDirection = Vector3.Normalize(position - target);

		public static Matrix4 view = Matrix4.LookAt(position, lookingDirection, up);

	}
}