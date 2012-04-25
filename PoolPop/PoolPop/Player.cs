using System;
using System.Collections.Generic;
using System.Text;

namespace PoolPop
{
    class Player
    {
      string
        m_name;  // Player name
      int
        m_suit;     // Ball suit -> 0 = solid, 1 = striped
      bool
        m_active,    // Is this player active?
        m_boPlayerWon,
        m_boFirstShot;  // Has the first shot been fired?
      List<ball>
        m_pocketedBalls; // Which balls the player has pocketed

      public Player(string name)
      {
        m_name = name;
        m_pocketedBalls = new List<ball>();
        m_boPlayerWon = false;
        m_boFirstShot = false;
      }

      public void setSuit(int suit)
      {
        m_suit = suit;
      }
      public int getSuit()
      {
        return m_suit;
      }

      public void addBall(ball pocketedBall)
      {
        m_pocketedBalls.Add(pocketedBall);
      }
      public int numBalls()
      {
        return m_pocketedBalls.Count;
      }

      public void setActive(bool active)
      {
        m_active = active;
      }

      public bool getActive()
      {
        return m_active;
      }

      public void vPlayerWon(
      )
      {
        m_boPlayerWon = true;
      }

      public bool boPlayerWon(
      )
      {
        return m_boPlayerWon;
      }

      public void vFirstShot(
        bool boFirstShot
      )
      {
        m_boFirstShot = boFirstShot;
      }

      public bool boFirstShot(
      )
      {
        return m_boFirstShot;
      }
    }
}
