using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

// The template provides you with a window which displays a 'linear frame buffer', i.e.
// a 1D array of pixels that represents the graphical contents of the window.

// Under the hood, this array is encapsulated in a 'Surface' object, and copied once per
// frame to an OpenGL texture, which is then used to texture 2 triangles that exactly
// cover the window. This is all handled automatically by the template code.

// Before drawing the two triangles, the template calls the Tick method in MyApplication,
// in which you are expected to modify the contents of the linear frame buffer.

// After (or instead of) rendering the triangles you can add your own OpenGL code.

// We will use both the pure pixel rendering as well as straight OpenGL code in the
// tutorial. After the tutorial you can throw away this template code, or modify it at
// will, or maybe it simply suits your needs.

namespace Template
{
	public class OpenTKApp : GameWindow
	{
		static int screenID;            // unique integer identifier of the OpenGL texture
		static MainScene app;       // instance of the application
		static bool terminated = false; // application terminates gracefully when this is true

		float sensitivity = 0.05f; //Mouse sensitivity
		float speed = 0.1f;	//Walking speed

		Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
		bool firstmove = true;
		Vector2 lastpos;
		protected override void OnLoad( EventArgs e )
		{
			// called during application initialization
			GL.ClearColor( 0, 0, 0, 0 );
			GL.Enable( EnableCap.Texture2D );
			GL.Disable( EnableCap.DepthTest );
			GL.Hint( HintTarget.PerspectiveCorrectionHint, HintMode.Nicest );
			ClientSize = new Size(1024, 512); //resolution
			app = new MainScene();
			app.screen = new Surface( Width, Height );
			Sprite.target = app.screen;
			screenID = app.screen.GenTexture();
			app.Init();
			CursorVisible = false;
			CursorGrabbed = true;
		}
		protected override void OnUnload( EventArgs e )
		{
			// called upon app close
			GL.DeleteTextures( 1, ref screenID );
			Environment.Exit( 0 );      // bypass wait for key on CTRL-F5
		}
		protected override void OnResize( EventArgs e )
		{
			// called upon window resize. Note: does not change the size of the pixel buffer.
			GL.Viewport( 0, 0, Width, Height );
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();
			GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );
		}
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			// called once per frame; app logic
			var keyboard = OpenTK.Input.Keyboard.GetState();
			if (keyboard[OpenTK.Input.Key.Escape]) terminated = true;

			if (!Focused) //When screen is focussed exit this code
			{
				return;
			}

			if (keyboard.IsKeyDown(OpenTK.Input.Key.W)) //forward movement
			{
				app.raytracer.cam.position += app.raytracer.cam.lookingDirection * speed * (float)e.Time;

			}
			if (keyboard.IsKeyDown(OpenTK.Input.Key.A)) //left movement
			{
				app.raytracer.cam.position += Vector3.Normalize(Vector3.Cross(app.raytracer.cam.lookingDirection, up)) * speed * (float)e.Time;

			}

			if (keyboard.IsKeyDown(OpenTK.Input.Key.S)) //backward movement
			{
				app.raytracer.cam.position -= app.raytracer.cam.lookingDirection * speed * (float)e.Time;

			}
			if (keyboard.IsKeyDown(OpenTK.Input.Key.D)) //right movement
			{
				app.raytracer.cam.position -= Vector3.Normalize(Vector3.Cross(app.raytracer.cam.lookingDirection, up)) * speed * (float)e.Time;

			}
			if (keyboard.IsKeyDown(OpenTK.Input.Key.Space)) //up movement
			{
				app.raytracer.cam.position += up * speed * (float)e.Time;

			}
			if (keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft)) //down movement
			{
				app.raytracer.cam.position -= up * speed * (float)e.Time;

			}
			var mouse = Mouse.GetState();

			float deltaX, deltaY;

			if (firstmove) //Save current mouse position as last position if this was the first move
			{
				lastpos = new Vector2(mouse.X, mouse.Y);
				firstmove = false;
			}
			else //Find difference between current mouse position and last mouse position
			{
				deltaX = mouse.X - lastpos.X;
				deltaY = mouse.Y - lastpos.Y;
				lastpos = new Vector2(mouse.X, mouse.Y);

				app.raytracer.cam.yaw -= deltaX * sensitivity; //yaw = look left/look right

				if (app.raytracer.cam.pitch > 89.0f)
				{
					app.raytracer.cam.pitch = 89.0f;
				}
				else if (app.raytracer.cam.pitch < -89.0f)
				{
					app.raytracer.cam.pitch = -89.0f;
				}
				else
				{
					app.raytracer.cam.pitch -= deltaY * sensitivity; //pitch = look up/look down
				}

				//Change lookingdirection according to new pitch and yaw values
				app.raytracer.cam.lookingDirection.X = (float)Math.Cos(MathHelper.DegreesToRadians(app.raytracer.cam.pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(app.raytracer.cam.yaw));
				app.raytracer.cam.lookingDirection.Y = (float)Math.Sin(MathHelper.DegreesToRadians(app.raytracer.cam.pitch));
				app.raytracer.cam.lookingDirection.Z = (float)Math.Cos(MathHelper.DegreesToRadians(app.raytracer.cam.pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(app.raytracer.cam.yaw));
				app.raytracer.cam.lookingDirection = Vector3.Normalize(app.raytracer.cam.lookingDirection);

				app.raytracer.cam.Update(); //update the camera variables
			}
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
        {
			
			Mouse.SetPosition(app.screen.width/2, app.screen.height/2); //move mouse to middle of the screen
			

			base.OnMouseMove(e);
		}

		protected override void OnRenderFrame( FrameEventArgs e )
		{
			// called once per frame; render
			app.Tick();
			if( terminated )
			{
				Exit();
				return;
			}
			// convert MyApplication.screen to OpenGL texture
			GL.BindTexture( TextureTarget.Texture2D, screenID );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
						   app.screen.width, app.screen.height, 0,
						   OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
						   PixelType.UnsignedByte, app.screen.pixels
						 );
			// draw screen filling quad
			GL.Begin( PrimitiveType.Quads );
			GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2( 1.0f, -1.0f );
			GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2( 1.0f, 1.0f );
			GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -1.0f, 1.0f );
			GL.End();
			// tell OpenTK we're done rendering
			SwapBuffers();
		}
		public static void Main( string[] args )
		{
			// entry point
			using( OpenTKApp app = new OpenTKApp() ) { app.Run( 30.0, 0.0 ); }
		}
	}
}