using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoolPop
{
    public class ball
    {
      // Instance variables
      float[] mVelocity;                              // Set the starting velocity to <x=0, z=0>
      float   mRadius,                                // Set the radius to 0.5
              mPositionX,                             // Holds the x-position
              mPositionZ,                             // Holds the z-position
              mMass,                                  // Holds the mass
              mFrictionX, mFrictionZ, mQuotientBig;   // Friction constants, % of velocity.. And quotient between the two velocities
      int     mNumber;                                // Which number the ball has
      Matrix  mMatrix, mRotationX, mRotationZ;        // Each ball stores rotation in a matrix, so that it is possible to add to the rotations incrementally, and not set them directly
      bool    mVisible;                               // Wether the ball is visible or not
      bool    m_boInsideBall;                         // Is the ball inside another ball?
      TextureCube mCMap;                              // The balls cubemap
      RenderTargetCube mRTC;                          // The rendertarget
      Texture2D ballTexture;
      List<int> m_lInsideBalls;                       // Which balls THIS ball is currently inside
      int m_suit;                                     // Suit of the ball
        

        // Parametric constructor
        public ball(float positionX, float positionZ, float mass, int number)
        {
            // Assign the parameters to the member variables
            mVelocity = new float[2];
            mVelocity[0]    = 0;
            mVelocity[1]    = 0;
            mRadius         = 0.1f;
            mPositionX      = positionX;
            mPositionZ      = positionZ;
            mMass           = mass;
            mNumber         = number;
            mVisible        = true;
            mFrictionX      = 0;
            mFrictionZ      = 0;
            mQuotientBig    = 0;

            mMatrix         = Matrix.Identity;
            mRotationX      = Matrix.Identity;
            mRotationZ      = Matrix.Identity;

            m_lInsideBalls = new List<int>();

            if (mNumber <= 7)
              m_suit = 0;
            else
              m_suit = 1;
        }

        // Set and get position coordinates
        public void setPosX(float X)
        {
            mPositionX = X;
        }
        public float getPosX()
        {
            return mPositionX;
        }

        public void setPosZ(float Z)
        {
            mPositionZ = Z;
        }
        public float getPosZ()
        {
            return mPositionZ;
        }

        // Set and get the velocities
        public void setVelX(float velX)
        {
            mVelocity[0] = velX;
        }
        public float getVelX()
        {
            return mVelocity[0];
        }

        public void setVelZ(float velZ)
        {
            mVelocity[1] = velZ;

            calcFriction();
        }
        public float getVelZ()
        {
            return mVelocity[1];
        }

        // Set and get the cubemap
        public void setTextureCube(TextureCube cMap)
        {
            mCMap = cMap;
        }
        public TextureCube getTextureCube()
        {
            return mCMap;
        }

        // Set and get the rendertarget
        public void setRenderTarget(RenderTargetCube RTC)
        {
            mRTC = RTC;
        }
        public RenderTargetCube getRenderTarget()
        {
            return mRTC;
        }
        

        // Get the mass of the ball
        public float getMass()
        {
            return mMass;
        }

        // Get the radius of the ball
        public float getRadius()
        {
            return mRadius;
        }

        // Calculate the friction for each axis
        private void calcFriction()
        {
            // The quotient between x and z velocity
            if (Math.Abs(mVelocity[0]) >= Math.Abs(mVelocity[1]))
            {
                mQuotientBig = Math.Abs(mVelocity[0]) / Math.Abs(mVelocity[1]);

                mFrictionX = 0.0002f;
                mFrictionZ = 0.0002f / mQuotientBig;
            }
            else
            {
                mQuotientBig = Math.Abs(mVelocity[1]) / Math.Abs(mVelocity[0]);

                mFrictionZ = 0.0002f;
                mFrictionX = 0.0002f / mQuotientBig;
            }
        }

        // Update position and rotation according to the active velocity vector
        public void updatePos()
        {
            // Update position based on the balls velocity
            mPositionX += mVelocity[0];
            mPositionZ += mVelocity[1];

            // Friction - reduce each balls' speed with a constant
            if (mVelocity[0] != 0)
            {
                // Store the balls velocities before the change and compare them to the ones after the change
                float[] velBefore = {mVelocity[0], mVelocity[1]};

                // Reduce the velocity based on the friction
                mVelocity[0] -= mFrictionX * (mVelocity[0] / Math.Abs(mVelocity[0]));
                mVelocity[1] -= mFrictionZ * (mVelocity[1] / Math.Abs(mVelocity[1]));

                // Set the velocity to zero if the balls starts to go "backwards"
                if(velBefore[0] > 0 && mVelocity[0] <= 0)
                {
                    mVelocity[0] = 0;
                    mVelocity[1] = 0;
                }
                if(velBefore[0] < 0 && mVelocity[0] >= 0)
                {
                    mVelocity[0] = 0;
                    mVelocity[1] = 0;
                }
            }
            
        }

        // Set and get the matrix
        public void setMatrix(Matrix matrix)
        {
            mMatrix = matrix;
        }
        public Matrix getMatrix()
        {
            return mMatrix;
        }

        // Set and get the rotation matrices
        public void setRotationX(Matrix rotationX)
        {
            mRotationX = rotationX;
        }
        public Matrix getRotationX()
        {
            return mRotationX;
        }

        public void setRotationZ(Matrix rotationZ)
        {
            mRotationZ = rotationZ;
        }
        public Matrix getRotationZ()
        {
            return mRotationZ;
        }

        // Set and get mVisible variable
        public void setVisible(bool visible)
        {
            mVisible = visible;
        }
        public bool getVisible()
        {
            return mVisible;
        }

        // Set and get mNumber
        public void setNumber(int number)
        {
            mNumber = number;
        }
        public int getNumber()
        {
            return mNumber;
        }

        // Set and get m_boInsideBall
        public void setInsideBall(int ball)
        {
          m_lInsideBalls.Add(ball);
        }
        public bool getInsideBall(int ball)
        {
          // Return true if the ball is in the list
          for (int i = 0; i < m_lInsideBalls.Count; i++)
            if (m_lInsideBalls[i] == ball)
              return true;

          // If not, return false
          return false;
        }
      public bool removeInsideBall(int ball)
      {
        // Remove the given ball if it exists, and return true
        return m_lInsideBalls.Remove(ball);
      }

      public void setTexture(Texture2D texture)
      {
        ballTexture = texture;
      }
      public Texture2D getTexture()
      {
        return ballTexture;
      }

      public int getSuit()
      {
        return m_suit;
      }
    }
}
