using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PoolPop
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class Game1 : Microsoft.Xna.Framework.Game
  {
    // Struct hole
    public struct hole
    {
      public float x, z, radius;
      public ball targetBall;

      public hole(float x, float z, float radius, ball targetBall)
      {
        this.x = x;
        this.z = z;
        this.radius = radius;
        this.targetBall = targetBall;
      }
    };  // End struct hole

    GraphicsDeviceManager
      graphics;
    SpriteBatch
      spriteBatch;
    // Effects
    BasicEffect
      effectBasic;     // Our basic shader effect
    Effect
      effectBall;

    List<TextureCube>
      cubeMaps; // All our cubemaps for each ball
    Texture2D
      player1Tex,          // This is the texture that will show the active player
      player2Tex;          // This is the texture that will show the active player
    SpriteFont
      SpriteFont1;

    // Model Resources
    Model
      m,      // Our ball model
      table,  // Our table model
      cue;    // Our cue model

    // Matrices
    Matrix
      world,      // Our world matrix
      view,       // Our view matrix
      projection; // Our projection matrix

    Matrix[]
      transformsTable,  // The tables local coordinate system/matrix
      transformsBall,   // The balls local coordinate system/matrix
      transformsCue;

    // Lists
    List<hole>
      holes;      // A list containing all the holes in the table
    List<ball>
      balls;      // A list containing all the balls in the game
    List<Player>
      m_players;  // List for holding one or two players

    // Camera vectors
    float[]
      viewPos;    // The x and z position of our view(Updated from the white balls' position)
    Vector3
      viewTarget,
      viewVector; // The vector between the camera and the ball

    // Game utility classes
    GameLogic
      gl;
    GameInput
      gi;
    GameRules
      gr;

    float
      menuRotater;
    int
      iWindowWidth = 1680,
      iWindowHeight = 1050;

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      // Lists
      cubeMaps = new List<TextureCube>();
      balls = new List<ball>();
      holes = new List<hole>();
      m_players = new List<Player>();

      // Matrices & vectors
      world = Matrix.Identity;
      projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), (float)iWindowWidth / (float)iWindowHeight, 0.001f, 100);
      viewPos = new float[2];
      viewTarget = new Vector3();
      viewVector = new Vector3();

      // Utility classes
      gi = new GameInput(ref graphics, ref balls, ref m_players);
      gr = new GameRules(ref gi, ref balls, ref m_players);
      gl = new GameLogic(ref holes, ref balls, ref m_players, ref gr);

      menuRotater = 0f;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      // GraphicsDevice settings
      //graphics.ToggleFullScreen();
      graphics.PreferredBackBufferWidth = iWindowWidth;
      graphics.PreferredBackBufferHeight = iWindowHeight;
      Window.Title = "PoolPop";
      graphics.ApplyChanges();

      // Initialize our basic effect
      effectBasic = new BasicEffect(graphics.GraphicsDevice);

      newGame(2);
      //addBalls();

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      // Load Models and Textures
      m = Content.Load<Model>("ball");              // Load our ball model
      table = Content.Load<Model>("table");             // Load our table model
      cue = Content.Load<Model>("cue");               // Load our cue model
      effectBall = Content.Load<Effect>("EnvironmentMap");   // Load our reflection environment map for use with the cubemap
      player1Tex = Content.Load<Texture2D>("textures/player1Tex");
      player2Tex = Content.Load<Texture2D>("textures/player2Tex");
      //SpriteFont1 = Content.Load<SpriteFont>("Arial");



      // Adding the effect to our ball
      foreach (ModelMesh modmesh in m.Meshes)
        foreach (ModelMeshPart modmeshpart in modmesh.MeshParts)
          modmeshpart.Effect = effectBall;

      // MODEL TRANSFORMS
      // Copy bone transforms for table
      transformsTable = new Matrix[table.Bones.Count];
      table.CopyAbsoluteBoneTransformsTo(transformsTable);

      // Copy bone transforms for ball
      transformsBall = new Matrix[m.Bones.Count];
      m.CopyAbsoluteBoneTransformsTo(transformsBall);

      transformsCue = new Matrix[cue.Bones.Count];
      cue.CopyAbsoluteBoneTransformsTo(transformsCue);

      // Register all holes
      foreach (ModelMesh mesh in table.Meshes)
        if (mesh.Name.Contains("hole"))
        {
          hole newHole;
          newHole.x = mesh.BoundingSphere.Center.X;
          newHole.z = mesh.BoundingSphere.Center.Y;
          newHole.radius = mesh.BoundingSphere.Radius;
          newHole.targetBall = null;
          // Register all holes in the table
          holes.Add(newHole);
        }
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      gi.update();    // Update game input

      if (!gi.getMenuState())
      {
        gl.update();    // Update game logic
        gr.update();    // Update game rules
      }

      if (m_players[0].boPlayerWon() || m_players[1].boPlayerWon())
      {
        int iStopGame = 0;
      }
      
      // TODO: Add your update logic here

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      /************************** Generate the cubemap **************************/

      if (!gi.getMenuState())
      {
        // Save view matrix
        Matrix tmpView = view;

        // Set the field of view to 90 degrees(so that we can generate the cubemap properly)
        projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f), (float)iWindowWidth / (float)iWindowHeight, 0.001f, 100);

        // Generate a cubemap for each ball
        for (int i = 0; i < balls.Count; i++)
        {
          graphics.GraphicsDevice.SetRenderTarget(balls[i].getRenderTarget(), CubeMapFace.NegativeZ); // Forward
          // Face camera forward
          view = Matrix.CreateLookAt(new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ()), new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ() - 1f), new Vector3(0, 1, 0));
          drawScene(gameTime, 1);

          graphics.GraphicsDevice.SetRenderTarget(balls[i].getRenderTarget(), CubeMapFace.PositiveZ); // Backward
          // Face camera backward
          view = Matrix.CreateLookAt(new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ()), new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ() + 1f), new Vector3(0, 1, 0));
          drawScene(gameTime, 1);

          graphics.GraphicsDevice.SetRenderTarget(balls[i].getRenderTarget(), CubeMapFace.PositiveY); // Up
          // Face camera up
          view = Matrix.CreateLookAt(new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ()), new Vector3(balls[i].getPosX(), 1f, balls[i].getPosZ()), new Vector3(0, 0, 1));
          drawScene(gameTime, 1);

          graphics.GraphicsDevice.SetRenderTarget(balls[i].getRenderTarget(), CubeMapFace.NegativeY); // Down
          // Face camera down
          view = Matrix.CreateLookAt(new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ()), new Vector3(balls[i].getPosX(), -1f, balls[i].getPosZ()), new Vector3(0, 0, 1));
          drawScene(gameTime, 1);

          graphics.GraphicsDevice.SetRenderTarget(balls[i].getRenderTarget(), CubeMapFace.NegativeX); // Right
          // Face camera right
          view = Matrix.CreateLookAt(new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ()), new Vector3(balls[i].getPosX() + 1f, 0f, balls[i].getPosZ()), new Vector3(0, 1, 0));
          drawScene(gameTime, 1);

          graphics.GraphicsDevice.SetRenderTarget(balls[i].getRenderTarget(), CubeMapFace.PositiveX); // Left
          // Face camera left
          view = Matrix.CreateLookAt(new Vector3(balls[i].getPosX(), 0f, balls[i].getPosZ()), new Vector3(balls[i].getPosX() - 1f, 0f, balls[i].getPosZ()), new Vector3(0, 1, 0));
          drawScene(gameTime, 1);

          // Reset the renderTarget(Cannot be saved unless reset)
          graphics.GraphicsDevice.SetRenderTarget(null);

          // Assign the renderTargetCube texture to our textureCube object
          balls[i].setTextureCube(balls[i].getRenderTarget());
        }

        // Restore the old view matrix, and the projection matrix
        view = tmpView;
        projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), (float)iWindowWidth / (float)iWindowHeight, 0.001f, 100);

        /**************************************************************************/
      }
      if (!gi.getMenuState())
        drawScene(gameTime, 0);  // Draw the scene
      else
        drawMenu(gameTime);
    }

    protected void drawMenu(GameTime gameTime)
    {
      graphics.GraphicsDevice.Clear(Color.LightBlue);
      if (gi.getMenuState())
      {
        // Cues/Menu choices
        for (int i = 0; i < 4; i++)
          foreach (ModelMesh mesh in cue.Meshes)
          {
            // some models also might use different effects, such as mat car texture, and shiny windows (our monkey texture only have one)
            foreach (BasicEffect effect in mesh.Effects)
            {
              effect.EnableDefaultLighting();
              effect.View = Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f), Vector3.Up);
              effect.Projection = projection;
              effect.World = world * transformsCue[mesh.ParentBone.Index] * Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(new Vector3(-0.15f, (1.6f - i) / 6, 0.1f));
              // *transformsTable[mesh.ParentBone.Index];// *Matrix.CreateTranslation(new Vector3(0f, 0f, 0f));// *Matrix.CreateRotationY(MathHelper.ToRadians(90)); 
            }
            mesh.Draw();
          }

        // The selection ball
        menuRotater += 2f;
        if (menuRotater == 360)
          menuRotater = 0;
        foreach (ModelMesh mesh in m.Meshes)
        {
          // some models also might use different effects, such as mat car texture, and shiny windows (our monkey texture only have one)
          foreach (Effect effect in mesh.Effects)
          {
            // Set the texture
            effect.Parameters["Texture"].SetValue(balls[8].getTexture());
            effect.Parameters["TextureEnabled"].SetValue(true);

            effect.Parameters["EnvironmentMapTex"].SetValue(balls[0].getTextureCube());

            // Set the world coordinates
            effect.Parameters["World"].SetValue(transformsBall[mesh.ParentBone.Index] * world * Matrix.CreateRotationZ(MathHelper.ToRadians(45)) * Matrix.CreateRotationY(MathHelper.ToRadians(menuRotater)) * Matrix.CreateTranslation(new Vector3(-0.5f, .87f - (gi.getMenuSelected() / 3), -0.8f)));
            effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f), Vector3.Up));
            effect.Parameters["Projection"].SetValue(projection);
          }
          mesh.Draw();
        }
      }

      if (Keyboard.GetState().IsKeyDown(Keys.Enter))
      {
        // An option was selected
        switch ((int)gi.getMenuSelected())
        {
          case 1:
            // New Game 1-Player
            //newGame(1);
            break;
          case 2:
            // New Game 2-Player
            //newGame(2);
            break;
          case 3:
            // Options
            break;
          case 4:
            // Exit Game
            break;
        }

        gi.setMenuState(false);
      }

      base.Draw(gameTime);
    }

    protected void drawScene(GameTime gameTime, int currentRenderTarget)
    {
      graphics.GraphicsDevice.Clear(Color.LightBlue);

      // RenderTarget = 0 -> render to screen
      // RenderTarget = 1 -> render to cubemap
      if (currentRenderTarget == 0)
      {
        spriteBatch.Begin();
        if (m_players[0].getActive())
          spriteBatch.Draw(player1Tex, Vector2.Zero, Color.White);
        else
          spriteBatch.Draw(player2Tex, Vector2.Zero, Color.White);
        spriteBatch.End();

        if (!gi.inMotion())
        {
          viewPos[0] = balls[0].getPosX();
          viewPos[1] = balls[0].getPosZ();
          viewTarget = new Vector3(balls[0].getPosX() - viewVector.X, 0, balls[0].getPosZ() - viewVector.Z);
        }
        // Table overview
        //view = Matrix.CreateLookAt(new Vector3(0f, 10f, 0f), new Vector3(0f, 0f, 0f), Vector3.Backward);


        // Ball perspective
        view = Matrix.CreateLookAt(new Vector3(viewPos[0] + viewVector.X * 4f, 2f + viewVector.Y * 4f, viewPos[1] + viewVector.Z * 4f), viewTarget, Vector3.Up);

        // Closer ball perspective
        //view = Matrix.CreateLookAt(new Vector3(balls[0].getPosX() + viewVector.X, 0.5f, balls[0].getPosZ() + viewVector.Z), new Vector3(balls[0].getPosX() - viewVector.X, 0, balls[0].getPosZ() - viewVector.Z), Vector3.Up);
      }

      // Drawing 2d spritebatch messes with the graphicsdevice messing up renderstates and 3d drawing order
      // So we reset some values
      GraphicsDevice.BlendState = BlendState.Opaque;
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      // Vector between camera and ball
      viewVector = new Vector3(balls[0].getPosX() - (balls[0].getPosX() + (float)Math.Cos(MathHelper.ToRadians(gi.getDegreeY()))), (float)Math.Sin(MathHelper.ToRadians(gi.getDegreeX())), balls[0].getPosZ() - (balls[0].getPosZ() + (float)Math.Sin(MathHelper.ToRadians(gi.getDegreeY()))));

      // Draw all the balls
      for (int i = 0; i < balls.Count; i++)
      {
        // Check to see if we are rendering for the cubemap or for the actual scene,
        // if it's for the cubemap, we don't want to update the rotations on the balls
        if (currentRenderTarget == 0)
        {
          // Set the x and z rotation for each ball
          balls[i].setRotationZ(Matrix.CreateRotationZ(-MathHelper.ToRadians(balls[i].getVelX() / (((float)Math.PI * 2 * balls[i].getRadius())) * 360)));
          balls[i].setRotationX(Matrix.CreateRotationX(MathHelper.ToRadians(balls[i].getVelZ() / (((float)Math.PI * 2 * balls[i].getRadius())) * 360)));

          // Update the rotation-matrix
          balls[i].setMatrix(balls[i].getMatrix() * balls[i].getRotationX() * balls[i].getRotationZ());
        }

        // Draw the ball
        foreach (ModelMesh mesh in m.Meshes)
        {
          // some models also might use different effects, such as mat car texture, and shiny windows (our monkey texture only have one)
          foreach (Effect effect in mesh.Effects)
          {
            // Set the texture
            effect.Parameters["Texture"].SetValue(balls[i].getTexture());
            effect.Parameters["TextureEnabled"].SetValue(true);

            //effect.Parameters["EnvironmentMapTex"].SetValue(balls[i].getTextureCube());

            // Set the world coordinates
            effect.Parameters["World"].SetValue(balls[i].getMatrix() * Matrix.CreateTranslation(new Vector3(balls[i].getPosX(), 0.02f, balls[i].getPosZ())));
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
          }
          mesh.Draw();
        }
      }

      // Draw the table, except the dummy spheres
      foreach (ModelMesh mesh in table.Meshes)
      {
        if (!mesh.Name.Contains("hole"))
        {
          // some models also might use different effects, such as mat car texture, and shiny windows (our monkey texture only have one)
          foreach (BasicEffect effect in mesh.Effects)
          {
            effect.EnableDefaultLighting();
            effect.View = view;
            effect.Projection = projection;
            effect.World = transformsTable[mesh.ParentBone.Index] * world;
          }
        }
        // If a hole
        else
        {
          foreach (BasicEffect effect in mesh.Effects)
          {
            effect.EnableDefaultLighting();
            effect.View = view;
            effect.Projection = projection;
            effect.World = transformsTable[mesh.ParentBone.Index];
          }
        }

        // Draw all the meshes except the dummy-holes
        if (!mesh.Name.Contains("hole"))
          mesh.Draw();
      }

      // Draw the cue if the balls are not in motion
      if (!gi.inMotion())
      {
        foreach (ModelMesh mesh in cue.Meshes)
        {
          // some models also might use different effects, such as mat car texture, and shiny windows (our monkey texture only have one)
          foreach (BasicEffect effect in mesh.Effects)
          {
            effect.EnableDefaultLighting();
            effect.View = view;
            effect.Projection = projection;
            effect.World = Matrix.CreateRotationX(MathHelper.ToRadians(5)) * Matrix.CreateRotationY(-MathHelper.ToRadians(gi.getDegreeY() - 90)) * Matrix.CreateTranslation(new Vector3(balls[0].getPosX() + (gi.getCueOffset() * (float)Math.Cos(MathHelper.ToRadians(gi.getDegreeY()))), 0.05f, balls[0].getPosZ() + (gi.getCueOffset() * (float)Math.Sin(MathHelper.ToRadians(gi.getDegreeY())))));
          }
          mesh.Draw();
        }
      }
      base.Draw(gameTime);
    }

    protected void addBalls()
    {
      int
        iBallNum = 2;
      // The white ball
      balls.Add(new ball(0f, -2.0f, 1f, 0));

      balls.Add(new ball(0f, 0f, 1f, 1));

      for (int i = 1; i < 5; i++)
      {
        for (int k = 1; k < i + 2; k++)
        {
          if (i % 2 == 1)
            if (k == 1)
              balls.Add(new ball(balls[balls.Count - 1].getPosX() - balls[0].getRadius() * 1.01f, balls[0].getRadius() * 2 * i * 1.01f, 1f, iBallNum));
            else
              balls.Add(new ball(balls[balls.Count - 1].getPosX() + (balls[0].getRadius() * 2 * 1.01f), balls[0].getRadius() * 2 * i * 1.01f, 1f, iBallNum));
          else
            if (k == 1)
              balls.Add(new ball(balls[balls.Count - 1].getPosX() + balls[0].getRadius() * 1.01f, balls[0].getRadius() * 2 * i * 1.01f, 1f, iBallNum));
            else
              balls.Add(new ball(balls[balls.Count - 1].getPosX() - (balls[0].getRadius() * 2 * 1.01f), balls[0].getRadius() * 2 * i * 1.01f, 1f, iBallNum));

          iBallNum++;
        }
      }

      // Create the rendertargets for all the 16 balls
      for (int i = 0; i < balls.Count; i++)
        balls[i].setRenderTarget(new RenderTargetCube(graphics.GraphicsDevice, 256, true, new SurfaceFormat(), new DepthFormat()));

      // Load all the ball textures
      balls[0].setTexture(Content.Load<Texture2D>("textures/ballwhite"));     // Load our ball-texture
      for (int i = 1; i < 16; i++)
        balls[i].setTexture(Content.Load<Texture2D>("textures/ball" + i));
    }

    private void newGame(int numPlayers)
    {
      // Reset the balls
      balls.Clear();
      addBalls();

      for (int i = 0; i < numPlayers; i++)
        m_players.Add(new Player("Player " + i));

      m_players[0].setActive(true);

    }
  }
}
