using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Graphics;

namespace PoolPop
{
    class GameInput
    {
        bool
            m_boShotActive,
            m_boMenu,
            m_boEscReleased,
            m_boMenuReleased;
        float
            m_fMouseYLastFrame,
            m_fMouseXLastFrame,
            m_fDegreeY,          // Degree variables for controlling our view matrix
            m_fDegreeX,
            m_fInitialPos,
            m_fCueOffset,        // Used to offset the cue when pulling it back to generate power for the shot
            m_fSelected;
        GraphicsDeviceManager
            m_graphics;
        List<ball>
            m_lBalls;
        List<Player>
            m_players;

        public GameInput(ref GraphicsDeviceManager gdm, ref List<ball> balls, ref List<Player> players)
        {
            m_lBalls = balls;
            m_players = players;
            m_graphics = gdm;

            m_fDegreeY = 65;
            m_fDegreeX = -20;
            m_fInitialPos = 0;
            m_fCueOffset = 0;
            m_fSelected = 1;
            m_boMenu = true;
        }

        public void update()
        {
          // Check for ESC press
          if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !m_boMenu && m_boEscReleased)
          {
            m_boMenu = true;
            m_boEscReleased = false;
          }
          else if (Keyboard.GetState().IsKeyDown(Keys.Escape) && m_boMenu && m_boEscReleased)
          {
            m_boMenu = false;
            m_boEscReleased = false;
          }
            

          if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            m_boEscReleased = true;

          if (!m_boMenu)
          {
            orientation();
            shot();
          }
          else
          {
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && m_boMenu && m_boMenuReleased)
            {
              if (m_fSelected > 1)
                m_fSelected--;

              m_boMenuReleased = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && m_boMenu && m_boMenuReleased)
            {
              if (m_fSelected < 4)
                m_fSelected++;

              m_boMenuReleased = false;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
              m_boMenuReleased = true;
          }   
        }

        // Returns true if there are any balls in motion, false if not
        public bool inMotion()
        {
            // Check to see if there are any balls in motion
            for (int i = 0; i < m_lBalls.Count; i++)
            {
              if ((m_lBalls[i].getVelX() != 0) || (m_lBalls[i].getVelZ() != 0))
                return true;
            }

            return false;
        }

        protected void orientation()
        {
            // Input
            // Update rotation of the cue, based on the x- and y-axis of the mouse
            if (!m_boShotActive)
            {
              // Rotation around y-axis
              if (Mouse.GetState().X > m_fMouseXLastFrame)   // Rotate to the right
                m_fDegreeY += Math.Abs(Mouse.GetState().X - m_fMouseXLastFrame) / 10;
              if (Mouse.GetState().X < m_fMouseXLastFrame)   // Rotate to the left
                m_fDegreeY -= Math.Abs(m_fMouseXLastFrame - Mouse.GetState().X) / 10;

              // Rotation around x-axis
              if (Mouse.GetState().Y > m_fMouseYLastFrame && m_fDegreeX < 45)   // Rotate down
                m_fDegreeX += Math.Abs(Mouse.GetState().Y - m_fMouseYLastFrame) / 10;
              if (Mouse.GetState().Y < m_fMouseYLastFrame && m_fDegreeX > -25)   // Rotate up
                m_fDegreeX -= Math.Abs(m_fMouseYLastFrame - Mouse.GetState().Y) / 10;

              if (m_fMouseXLastFrame != Mouse.GetState().X || m_fMouseYLastFrame != Mouse.GetState().Y)
                Mouse.SetPosition(m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight / 2);   // Reset the mouse position to the center of screen

              m_fMouseXLastFrame = Mouse.GetState().X;   // Store the mouse position
              m_fMouseYLastFrame = Mouse.GetState().Y;   // Store the mouse position
            }
        }   // End function, orientation

        protected void shot()
        {
            // Input
            // Shoot
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !inMotion())
            {
              if (!m_boShotActive)    // If we are not currently performing a shot
              {
                m_fInitialPos = Mouse.GetState().Y; // Store the current mouse position
                m_boShotActive = true;              // We have started to perform a shot at this point
              }
              // Update the cueOffset(used to move the cue when moving the mouse) IF we are inside a range of -9 to 100 on the y-axis
              if ((Mouse.GetState().Y - m_fInitialPos >= -9) && (Mouse.GetState().Y - m_fInitialPos <= 100))
              {
                m_fCueOffset = (Mouse.GetState().Y - m_fInitialPos);  // Calculate the cueOffset
                m_fCueOffset = -m_fCueOffset / 100;                   // Make it negative, and divide by 100 to make it small enough to use

                m_fMouseYLastFrame = Mouse.GetState().Y;   // Store the mouses' y-coordinate
              }
              else if (Mouse.GetState().Y - m_fInitialPos < -9)  // The ball was hit
              {
                // Determine the speed at which the ball was hit
                float speed = Mouse.GetState().Y - m_fMouseYLastFrame;

                // Assign the velocities to each axis
                m_lBalls[0].setVelX(((float)Math.Cos(MathHelper.ToRadians(m_fDegreeY)) * speed) / -500);
                m_lBalls[0].setVelZ(((float)Math.Sin(MathHelper.ToRadians(m_fDegreeY)) * speed) / -500);

                m_boShotActive = false; // Shot has been executed, and is no longer active

                // Player fired his first shot
                m_players[m_players[0].getActive() == true ? 0 : 1].vFirstShot(true);
              }
              else
              {
                // Are we moving the cue in positive or negative direction(backwards or forwards)?
                if (Mouse.GetState().Y > m_fMouseYLastFrame)    // Negative(backwards)
                  m_fInitialPos += Mouse.GetState().Y - (m_fInitialPos + 100);
                if (Mouse.GetState().Y < m_fMouseYLastFrame)    // Positive(forwards)
                  m_fInitialPos -= m_fInitialPos - Mouse.GetState().Y - 9;

                m_fMouseYLastFrame = Mouse.GetState().Y;   // Store the mouses' y-coordinate
              }
            }

            // Player releases the button and thus cancels the shot
            //if (Mouse.GetState().LeftButton == ButtonState.Released)
            else
            {
              m_boShotActive = false; // The shot is no longer active
              m_fCueOffset = 0;      // Reset the cues offset
            }
        }   // End function, shot

      public float getMenuSelected()
      {
        if (m_boMenu)
        {
          return m_fSelected;
        }

        return 0; // Nothing was selected
      }

        public float getDegreeX()
        {
          return m_fDegreeX;
        }

        public float getDegreeY()
        {
          return m_fDegreeY;
        }

        public float getCueOffset()
        {
          return m_fCueOffset;
        }

        public bool getMenuState()
        {
          return m_boMenu;
        }
      public void setMenuState(bool state)
      {
        m_boMenu = state;
      }
    }
}       
