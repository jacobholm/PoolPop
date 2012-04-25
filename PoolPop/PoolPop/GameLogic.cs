using System;
using System.Collections.Generic;
using System.Text;


namespace PoolPop
{  
    class GameLogic
    {
      List<ball> m_balls;
      List<Game1.hole> m_holes;
      List<Player> m_players;
      GameRules
        m_gr;

      public GameLogic(ref List<Game1.hole> holes, ref List<ball> balls, ref List<Player> players, ref GameRules gr)
        {
            m_balls = balls;
            m_holes = holes;
            m_players = players;
            m_gr = gr;
        }

        public void update(
        )
        {
            // Update the balls positions based on their velocities
            for (int i = 0; i < m_balls.Count; i++)
            {
                m_balls[i].updatePos();
            }

            // Collisions
            checkCollBallBall();
            checkCollBallWall();
        }

        protected void checkCollBallBall(  // Traverse the m_balls List and check for collisions
        )
        {
            // Check for ball-ball collision
            for (int i = 0; i < m_balls.Count; i++)
            {
                for (int k = i + 1; k < m_balls.Count; k++)
                {
                    // Check to see if the m_balls collide
                    float distance = (float)Math.Sqrt(Math.Pow(m_balls[k].getPosX() - m_balls[i].getPosX(), 2) + Math.Pow(m_balls[k].getPosZ() - m_balls[i].getPosZ(), 2));

                    if (!m_balls[i].getInsideBall(k))
                    {
                      if (distance == (m_balls[i].getRadius() + m_balls[k].getRadius()))
                      {
                        collision(m_balls[i], m_balls[k]);
                      }
                      else if (distance < (m_balls[i].getRadius() + m_balls[k].getRadius()))
                      {
                        collision(m_balls[i], m_balls[k]);
                        m_balls[i].setInsideBall(k);
                        m_balls[k].setInsideBall(i);
                      }
                    }
                    else if (m_balls[i].getInsideBall(k) && distance > (m_balls[i].getRadius() + m_balls[k].getRadius()) * 1.1f)
                    {
                      m_balls[i].removeInsideBall(k);
                      m_balls[k].removeInsideBall(i);
                    }
                }
            }   // End for, ball-ball collision
        }

        protected void checkCollBallWall(  // Collision between m_balls and walls
        )
        {
            // Check for ball-wall collision
            // Define the walls
            for (int i = 0; i < m_balls.Count; i++)
            {
                if ((m_balls[i].getPosX() + m_balls[i].getRadius()) >= m_holes[1].x || (m_balls[i].getPosX() - m_balls[i].getRadius()) <= m_holes[0].x)
                {
                    m_balls[i].setVelX(-m_balls[i].getVelX());
                    checkCollBallHole(ref i);
                }
                if ((m_balls[i].getPosZ() + m_balls[i].getRadius()) >= m_holes[0].z || (m_balls[i].getPosZ() - m_balls[i].getRadius()) <= m_holes[4].z)
                {
                    m_balls[i].setVelZ(-m_balls[i].getVelZ());
                    checkCollBallHole(ref i);
                }
            }
        }   // End for, ball-wall collision

        protected void checkCollBallHole(   // Check for collisions between ball and m_holes
            ref int index                       // Index of the ball to check for collision with m_holes
        )
        {
            // Check for ball-hole collision
            for (int k = 0; k < m_holes.Count; k++)
            {
                float distance = (float)Math.Abs(Math.Sqrt(Math.Pow(m_holes[k].x - m_balls[index].getPosX(), 2) + Math.Pow(m_holes[k].z - m_balls[index].getPosZ(), 2)));

                // Remember to use mass as radius, since we used it this way earlier
                // If the distance is less than the radius of the hole, the ball goes in
                if (distance < m_holes[k].radius+0.06f)
                {
                  m_balls[index].setPosX(-3f);         // Get the ball out of the way
                  m_balls[index].setVelX(0.0f);        // Set x velocity to zero
                  m_balls[index].setVelZ(0.0f);        // Set z velocity to zero

                  //m_balls[index].setVisible(false);    // Make the ball invisible

                  // Add the ball to the active players ball list
                  for (int i = 0; i < m_players.Count; i++)
                    if (m_players[i].getActive())
                    {
                      if (m_balls[index].getNumber() == 0)  // White ball
                        m_gr.vWhiteBallPocketed(true);
                      else
                      {
                        /**** FIRST POCKETED *****************************/
                        if (m_players[i].numBalls() == 0)
                        {
                          if (m_balls[index].getSuit() == 0)
                          {
                            m_players[i].setSuit(0);
                            m_players[m_players.Count - (i + 1)].setSuit(1);
                          }
                          else
                          {
                            m_players[i].setSuit(1);
                            m_players[m_players.Count - (i + 1)].setSuit(0);
                          }
                          /*************************************************/
                        }
                        else
                        {
                          // Check whether it was the players suit or not
                          if (m_players[i].getSuit() != m_balls[index].getSuit()) // It wasn't the active players ball
                          {
                            // Switch active players
                            m_players[i].setActive(false);
                            m_players[m_players.Count - (i + 1)].setActive(true);
                          }
                        }
                      }
                        
                      m_players[i].addBall(m_balls[index]); // Add ball to players "pocket list"
                      m_balls.RemoveAt(index);              // Remove ball from main ball list
                      index--;
                    }
                }
            }
        }

        // This method updates the vectors of each ball when they collide
        // This method contains all the math behind the 2D collisions between the m_balls
        protected void collision(
            ball ball1,
            ball ball2
        )
        {
            // The normal vector between the colliding surfaces of the m_balls
            float[] unitNormalVec = { ball2.getPosX() - ball1.getPosX(), ball2.getPosZ() - ball1.getPosZ() };

            // Normalize the vector
            float magnitude = (float)Math.Sqrt(Math.Pow(unitNormalVec[0], 2) + Math.Pow(unitNormalVec[1], 2));
            unitNormalVec[0] /= magnitude;
            unitNormalVec[1] /= magnitude;

            // The tangent vector between the colliding surfaces of the m_balls
            float[] unitTangentVec = { -unitNormalVec[1], unitNormalVec[0] };

            // Resolve the SCALAR tangential and normal components from the original velocity vectors
            float V1nB = (unitNormalVec[0] * ball1.getVelX()) + (unitNormalVec[1] * ball1.getVelZ());      // Normal vector for ball1
            float V1tB = (unitTangentVec[0] * ball1.getVelX()) + (unitTangentVec[1] * ball1.getVelZ());    // Tangent vector for ball1
            float V2nB = (unitNormalVec[0] * ball2.getVelX()) + (unitNormalVec[1] * ball2.getVelZ());      // Normal vector for ball2
            float V2tB = (unitTangentVec[0] * ball2.getVelX()) + (unitTangentVec[1] * ball2.getVelZ());    // Tangent vector for ball2

            // Calculate velocity scalar AFTER the collision for the normal and tangent vectors
            float V1tA = V1tB, V2tA = V2tB;     // The tangent vector after are the same as the ones before, since there is no force in tangential direction between the m_balls

            // Calculate the normal scalars after collision for both m_balls
            float V1nA = ((V1nB * (ball1.getMass() - ball2.getMass())) + (2 * ball2.getMass() * V2nB)) / (ball1.getMass() + ball2.getMass());   // Normal velocity scalar after collision for ball1
            float V2nA = ((V2nB * (ball2.getMass() - ball1.getMass())) + (2 * ball1.getMass() * V1nB)) / (ball1.getMass() + ball2.getMass());   // Normal velocity scalar after collision for ball2

            // Convert scalar normal/tangential velocities into vectors by multiplying them with the initial normal/tangential vectors
            float[] V1nVecA = { V1nA * unitNormalVec[0], V1nA * unitNormalVec[1] };
            float[] V1tVecA = { V1tA * unitTangentVec[0], V1tA * unitTangentVec[1] };
            float[] V2nVecA = { V2nA * unitNormalVec[0], V2nA * unitNormalVec[1] };
            float[] V2tVecA = { V2tA * unitTangentVec[0], V2tA * unitTangentVec[1] };

            // Add the normal/tangential vectors for each object to find the final vectors
            float[] V1A = { V1nVecA[0] + V1tVecA[0], V1nVecA[1] + V1tVecA[1] };
            float[] V2A = { V2nVecA[0] + V2tVecA[0], V2nVecA[1] + V2tVecA[1] };

            // Assign the new vectors to each ball
            ball1.setVelX(V1A[0]);
            ball1.setVelZ(V1A[1]);

            ball2.setVelX(V2A[0]);
            ball2.setVelZ(V2A[1]);
        }
    }
}
