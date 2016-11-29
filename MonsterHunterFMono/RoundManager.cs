using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public class RoundManager
    {
        int MaxRoundWins { get; set; }
        public int Player1RoundWins { get; set; }
        public int Player2RoundWins { get; set; }

        int Timer { get; set; }
        int TimeLimit { get; set; }

        Player player1;
        Player player2;
        float currentTime;

        int wrapUpTimer;
        float fadeOutAmount;

        public RoundManager(Player player1, Player player2)
        {
            TimeLimit = 99;
            MaxRoundWins = 2;
            Timer = TimeLimit;
            this.player1 = player1;
            this.player2 = player2;
            currentTime = 0f;
            fadeOutAmount = 0.0f;
        }

        public void roundEnd(int playerNumber)
        {
            ResetRound = false;
            if (playerNumber == 1)
            {
                Player1RoundWins++;              
            }
            else
            {
                Player2RoundWins++;
            }
            wrapUpTimer = 100;
        }

        public Boolean isMatchOver()
        {
            if (Player1RoundWins == 2 || Player2RoundWins == 2)
            {
                return true;
            }
            return false;
        }

        public void handleRoundEnd(ProjectileManager projectileManager)
        {
            wrapUpTimer--;
            if (wrapUpTimer >= 10 && wrapUpTimer <= 20)
            {     
                fadeOutAmount = 1.0f - ((wrapUpTimer - 10f)/10f);
            }
            else if (wrapUpTimer < 10 && !isMatchOver())
            {
                fadeOutAmount = wrapUpTimer / 10f;
            }

            if (fadeOutAmount == 1.0f) 
            {
                reset();
                projectileManager.clearAllProjectiles();
            }
            if (wrapUpTimer <= 0)
            {
                ResetRound = true;
            }
        }

        public void handleMatchEnd(ProjectileManager projectileManager)
        {
            

            if (fadeOutAmount == 1.0f)
            {
                reset();
                projectileManager.clearAllProjectiles();
            }
            if (fadeOutAmount > 0f)
            {
                fadeOutAmount -= .1f;
            }
            if (wrapUpTimer <= 0)
            {
                ResetRound = true;
            }
        }

        public void reset()
        {
            player1.resetRound();
            player2.resetRound();
            Timer = TimeLimit;
        }
        public float FadeAmount
        {
            get { return fadeOutAmount; }           
        }

        public void keepFadedOut()
        {
            fadeOutAmount = 1f;
        }

        public void decrementTimer(GameTime gameTime)
        {

            float countDuration = 2f; //every  2s.
            

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

            if (currentTime >= countDuration)
            {
                Timer--;
                currentTime -= countDuration; // "use up" the time
                //any actions to perform
            }    
        }

        public String displayTime()
        {
            return string.Format("{0}", Timer); 
        }
        public Boolean ResetRound { get; set; }
        public Boolean isTimeOut()
        {
            if (Timer <= 0)
            {
                return true;
            }
            else return false;
        }

        public void timeOut()
        {
            if (player1.CurrentHealth < player2.CurrentHealth)
            {
                roundEnd(1);
            }
            else
            {
                roundEnd(2);
            }
        }

    }
}
