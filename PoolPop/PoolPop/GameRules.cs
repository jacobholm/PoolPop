using System;
using System.Collections.Generic;
using System.Text;

namespace PoolPop
{
  class GameRules
  {
    GameInput
      m_gi;
    List<ball>
      m_balls;
    List<Player>
      m_players;
    bool
      m_boPocketWhiteBall;
    int
      m_iLastNrBalls;

    public GameRules(ref GameInput gi, ref List<ball> balls, ref List<Player> players)
    {
      m_gi = gi;
      m_balls = balls;
      m_iLastNrBalls = m_balls.Count;
      m_boPocketWhiteBall = false;
      m_players = players;
    }

    public void update()
    {
      if (!m_gi.inMotion())
      {
        int
          iActivePlayer = m_players[0].getActive() == true ? 0 : 1;

        // If the white ball is invisible, it has been pocketed.. Reset it when all balls stand still
        if (m_boPocketWhiteBall)
        {
          m_boPocketWhiteBall = false;  // Reset
          //m_balls[0].setVisible(true);
          m_balls[0].setPosX(0f);
          m_balls[0].setPosZ(-2f);

          // Change player
          m_players[iActivePlayer].setActive(false);
          m_players[m_players.Count - (iActivePlayer + 1)].setActive(true);
        }

        // No balls were pocketed, change player
        if (m_players[iActivePlayer].boFirstShot() && m_balls.Count == m_iLastNrBalls)
        {
          // Change player
          m_players[iActivePlayer].setActive(false);
          m_players[m_players.Count - (iActivePlayer + 1)].setActive(true);

          m_players[iActivePlayer].vFirstShot(false);
        }
        else
          m_iLastNrBalls = m_balls.Count;
      }

      // Check if the 8-ball was pocketed
      if (!boFindBall(8))
      {
        // If the player has no balls left, he wins, otherwise he looses
        int
          iActivePlayer = m_players[0].getActive() == true ? 0 : 1,
          iActiveSuit = m_players[iActivePlayer].getSuit();
        bool
          boActivePlayerWins = true;

        // Loop remaining balls to see if the player has any balls left in his suit
        for (int i = 0; i < m_balls.Count; i++)
        {
          if (m_balls[i].getSuit() == iActiveSuit)
          {
            boActivePlayerWins = false; // The player had balls left in his suit, he looses
            break;
          }
        }

        // End game
        if (boActivePlayerWins)
          m_players[iActivePlayer].vPlayerWon();
        else
          m_players[m_players.Count - (iActivePlayer + 1)].vPlayerWon();
      }
    }

    private bool boFindBall(
      int iNumber
    )
    {
      for (int i = 0; i < m_balls.Count; i++)
        if (m_balls[i].getNumber() == iNumber)
          return true;

      return false;
    }

    public void vWhiteBallPocketed(
      bool boPocketed
    )
    {
      m_boPocketWhiteBall = boPocketed;
    }
  }
}