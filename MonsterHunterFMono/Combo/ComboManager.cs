using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace MonsterHunterFMono
{
    public class ComboManager
    {
        private int player1ComboNumber = 0;
        private int player2ComboNumber = 0;

        private int TIME_TO_DISPLAY_COMBO = 50;
        private int comboTimer = 0;

        ProrationStrategy ProrationStrategy { get; set; }

        SpriteFont spriteFont;

        public ComboManager(SpriteFont SpriteFont)
        {
            // By default we'll use a basic one. A better one can be supplied if needed
            //
            ProrationStrategy = new BasicProrationStrategy();
            spriteFont = SpriteFont;
        }

        public void registerHit(HitInfo hitInfo)
        {
            ProrationStrategy.registerHit(hitInfo);
        }

        public int calculateProratedDamage(HitInfo hitInfo)
        {
            return ProrationStrategy.calculateProratedDamage(hitInfo);
        }

        public int calculateProratedHitStun(HitInfo hitInfo)
        {
            return ProrationStrategy.calculateProratedHitStun(hitInfo);
        }

        public int Player1ComboNumber
        {
            get { return player1ComboNumber; }
        }

        public int Player2ComboNumber
        {
            get { return player2ComboNumber; }
        }

        public void player1LandedHit(CharacterState hitPlayersState)
        {
            playerLandedHit(hitPlayersState, 1);
        }
        public void player2LandedHit(CharacterState hitPlayersState)
        {
            playerLandedHit(hitPlayersState, 2);
        }
        public Boolean displayCombo()
        {
            return (comboTimer > 0 && (player2ComboNumber > 1 || player1ComboNumber > 1));
        }

        public void displayComboMessage(SpriteBatch spriteBatch)
        {
            if (displayCombo())
            {
                if (player1ComboNumber > 0)
                {
                    spriteBatch.DrawString(spriteFont, Player1ComboNumber + "", new Vector2(33, 300), Color.Black, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                    spriteBatch.DrawString(spriteFont, Player1ComboNumber + "", new Vector2(32, 300), Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                }
                else if (player2ComboNumber > 0)
                {
                    spriteBatch.DrawString(spriteFont, Player2ComboNumber + "", new Vector2(33, 300), Color.Black, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                    spriteBatch.DrawString(spriteFont, Player2ComboNumber + "", new Vector2(32, 300), Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                }
            }
        }

        public void decrementComboTimer()
        {
            if (comboTimer > 0)
            {
                comboTimer--;
            }
        }

        public void playerLandedHit(CharacterState hitPlayersState, int playerNumber)
        {
            // No object reference for integers and I don't wanna anything goofy so have repetition;
            //
            if (hitPlayersState != CharacterState.HIT)
            {
                player1ComboNumber = 0;
                player2ComboNumber = 0;
                ProrationStrategy.startCombo();
            }
            if (playerNumber == 1)
            {  
                if (player1ComboNumber == 0 || hitPlayersState == CharacterState.HIT)
                {
                    player1ComboNumber++;
                }
            }
            else
            {                
                if (player2ComboNumber == 0 || hitPlayersState == CharacterState.HIT)
                {
                    player2ComboNumber++;
                }
            }
            comboTimer = TIME_TO_DISPLAY_COMBO;
        }       
    }  
}
