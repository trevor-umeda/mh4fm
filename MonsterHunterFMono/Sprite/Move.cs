
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterHunterFMono
{
    public class Move : SpriteAnimation, ICloneable
    {
        private Hitbox[] hitboxInfo;

        private Hitbox[] hurtboxInfo;

        // TODO move this out to a seperate object problly
        //
        private Boolean[] resetHitInfo;

        private int[] frameLengthInfo;

        private int[] xMovementInfo;

        private int frameLengthTimer;

        private int loopCount;

        private CharacterState characterState;

        bool hasHitOpponent = false;

        bool isBlocked = false;

        private int projectileCreationFrame = -1;
            
        private HitInfo hitInfo;

        public String NextMoveOnHit { get; set; }

        public String BackupMove { get; set; }

        public Hitbox[] HitBoxInfo
        {
            get
            {
                return hitboxInfo;
            }
            set
            {
                hitboxInfo = value;
            }
        }

        public Hitbox CurrentHitboxInfo
        {
            get { return hitboxInfo[CurrentFrame]; }
        }

        public Hitbox CurrentHurtboxInfo
        {
            get { return hurtboxInfo[CurrentFrame]; }
        }

        public int CurrentXMovementInfo
        {
            get 
            {
                if (xMovementInfo != null )
                {
                    return xMovementInfo[CurrentFrame];
                }
                else return 0;
            }
        }

        public int CurrentFrameLengthInfo
        {
            get { return frameLengthInfo[CurrentFrame]; }
        }

        public bool HasHitOpponent
        {
            get { return hasHitOpponent; }
            set { hasHitOpponent = value; }
        }

        public bool Blocked
        {
            get { return isBlocked; }
            set { isBlocked = value; }
        }

        public HitInfo HitInfo
        {
            get { return hitInfo; }
            set { hitInfo = value; } 
        }

        public int LoopCount
        {
            get { return loopCount; }
            set { loopCount = value; }
        }

        public int FrameLengthTimer
        {
            get { return frameLengthTimer; }
            set { frameLengthTimer = value; }
        }

        public CharacterState CharacterState
        {
            get { return characterState; }
            set { characterState = value; }
        }

        public int ProjectileCreationFrame
        {
            get { return projectileCreationFrame; }
            set { projectileCreationFrame = value; }
        }

        public void AddHitboxInfo(int index, Hitbox hitbox)
        {
            hitboxInfo[index] = hitbox;
        }

        public void AddHurtboxInfo(int index, Hitbox hitbox)
        {
            hurtboxInfo[index] = hitbox;
        }

        public void AddResetInfo(int index, bool reset)
        {
            resetHitInfo[index] = reset;
        }

        public void SetFrameLengthInfo(int[] info)
        {
            frameLengthInfo = info;
        }

        public void SetXMovementInfo(int[] info)
        {
            xMovementInfo = info;
        }

        public Move(Texture2D texture, int X, int Y, int Width, int Height, int Frames, int columns, float frameLength, CharacterState CharacterState)
        {
            Texture = texture;
            RectInitialFrame = new Rectangle(X, Y, Width, Height);
            FrameCount = Frames;
            hitboxInfo = new Hitbox[Frames];
            hurtboxInfo = new Hitbox[Frames];
            resetHitInfo = new Boolean[Frames];
            FrameLength = frameLength;
            characterState = CharacterState;
            Columns = columns;
            frameLengthInfo = new int[Frames];
            frameLengthTimer = 0;
            loopCount = 1;
        }
        public override Boolean isLastFrameOfAnimation()
        {
            return (CurrentFrame == FrameCount - 1 && PlayCount == loopCount - 1);
        }
        public override void Update(GameTime gameTime)
        {
 
            FrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (FrameTimer > FrameLength)
            {
                FrameTimer = 0.0f;
                frameLengthTimer++;
                if (frameLengthInfo[CurrentFrame] <= 0 ||frameLengthTimer >= frameLengthInfo[CurrentFrame])
                {
                    frameLengthTimer = 0;
                    CurrentFrame = (CurrentFrame + 1) % FrameCount;
                    if (resetHitInfo[CurrentFrame])
                    {
                        HasHitOpponent = false;
                        Blocked = false;
                    }
                    if (CurrentFrame == 0)
                    {
                        if (StartFrame != null)
                        {
                            CurrentFrame = StartFrame;
                        }
                        PlayCount = (int)MathHelper.Min(PlayCount + 1, int.MaxValue);
                        if (PlayCount >= loopCount)
                        {
                            IsDone = true;
                        }
                       
                    }
                }
               

            }
        }

        object ICloneable.Clone()
        {
            return new HitAnimation(Texture, this.RectInitialFrame.X, this.RectInitialFrame.Y,
                                      this.RectInitialFrame.Width, this.RectInitialFrame.Height,
                                      FrameCount, this.Columns, this.FrameLength, this.characterState);
        }
    }
    
}
