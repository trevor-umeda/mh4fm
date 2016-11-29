using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonsterHunterFMono
{
    class HitAnimation : Move
    {

        private int hitStunCounter = -1;

        private int totalTimePlaying = 0;

        private int midwayPauseTimer;

        public int HitStunCounter
        {
            get { return this.hitStunCounter; }
            set 
            { 
                this.hitStunCounter = value;
                if (value > FrameLength)
                {
                    midwayPauseTimer = value - ((int)FrameCount - StartFrame);
                }
            }
        }

        public HitAnimation(Texture2D texture, int X, int Y, int Width, int Height, int Frames, int Columns, float FrameLength, CharacterState CharacterState)
            : base(texture, X, Y, Width, Height, Frames, Columns, FrameLength, CharacterState)
        {
          
        }

     

        public void reset()
        {
            totalTimePlaying = 0;
        }

        public override void Update(GameTime gameTime)
        {
            FrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (FrameTimer > FrameLength)
            {
                FrameTimer = 0.0f;
                FrameLengthTimer++;
                if (CurrentFrameLengthInfo <= 0 || FrameLengthTimer >= CurrentFrameLengthInfo)
                {
                    FrameLengthTimer = 0;
                    if (midwayPauseTimer > 0 && CurrentFrame == StartFrame + 1)
                    {
                        midwayPauseTimer--;
                    }
                    else if(FrameCount != 1)
                    {
                        CurrentFrame = (CurrentFrame + 1) % FrameCount;
                        while (CurrentFrameLengthInfo <= 0)
                        {
                            CurrentFrame = (CurrentFrame + 1) % FrameCount;
                        }
                    }
                    totalTimePlaying++;

                    if (hitStunCounter > 0 && totalTimePlaying >= hitStunCounter)
                    {
                        CurrentFrame = StartFrame;
                        IsDone = true;
                        PlayCount++;
                        totalTimePlaying = 0;
                    }
                }
            }
            
        }
    }
}
