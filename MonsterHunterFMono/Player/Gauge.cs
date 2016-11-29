using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public class Gauge : SpriteAnimation, ICloneable
    {
        private Texture2D barTexture;
        private int healthBarMargin;
        private int playerNumber;

        private int currentAmount;
        private int maxAmount;
        int height;

        private Texture2D outerFrame;

        public Gauge(Texture2D healthBar, int height, int healthBarMargin, int playerNumber, int frameCount, Rectangle initialFrame)
        {
            BarTexture = healthBar;
            this.height = height;
            this.healthBarMargin = healthBarMargin;
            this.playerNumber = playerNumber;
            FrameCount = frameCount;
            RectInitialFrame = initialFrame;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if (OuterBarTexture != null)
            {

            }
            if (playerNumber == 1)
            {
                int extra = (int)((FrameWidth - (int)(FrameWidth * ((double)currentAmount / maxAmount))) * .45f);
                if (OuterBarTexture != null)
                {
                    spriteBatch.Draw(OuterBarTexture, new Vector2(HealthBarMargin,
                                        height), new Rectangle(0, 0, FrameWidth , FrameHeight), Color.White, 0, new Vector2(0, 0), .45f, SpriteEffects.None, 0);
                }
                
                spriteBatch.Draw(BarTexture, new Vector2(HealthBarMargin + extra,
                    height), new Rectangle((int)(FrameWidth * (1 - ((double)currentAmount / maxAmount))), FrameHeight * CurrentFrame, (int)(FrameWidth * ((double)currentAmount / maxAmount)), FrameHeight), Color.White, 0, new Vector2(0, 0), .45f, SpriteEffects.None, 0);
            }
            else
            {
                if (OuterBarTexture != null)
                {
                    spriteBatch.Draw(OuterBarTexture, new Vector2(HealthBarMargin,
                                       height), new Rectangle(0, 0, FrameWidth, FrameHeight), Color.White, 0, new Vector2(0, 0), .45f, SpriteEffects.FlipHorizontally, 0);
                }
                spriteBatch.Draw(BarTexture, new Vector2(HealthBarMargin,
                    height), new Rectangle((int)(FrameWidth * (1 - ((double)currentAmount / maxAmount))), FrameHeight * CurrentFrame, (int)(FrameWidth * ((double)currentAmount / maxAmount)), FrameHeight), Color.White, 0, new Vector2(0, 0), .45f, SpriteEffects.FlipHorizontally, 0);
            }
            

        }

        public Texture2D OuterBarTexture
        {
            get { return outerFrame; }
            set { outerFrame = value; }
        }

        public Texture2D BarTexture
        {
            get { return barTexture; }
            set { barTexture = value; }
        }

        public int HealthBarMargin
        {
            get { return healthBarMargin; }
            set { healthBarMargin = value; }
        }

        public int CurrentAmount
        {
            get { return currentAmount; }
            set { currentAmount = value; }
        }

        public int MaxAmount
        {
            get { return maxAmount; }
            set { maxAmount = value; }
        }

        public int Width
        {
            get { return barTexture.Width; }
        }
    }
}
