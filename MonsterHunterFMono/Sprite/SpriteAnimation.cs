using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonsterHunterFMono
{
    public abstract class SpriteAnimation : ICloneable
    {
         // The sprite itself will hold onto its own sprite
        private Texture2D t2dTexture;

       
        private Rectangle rectInitialFrame;

        private int rows = 1;
        private int columns;

        // Number of frames in the Animation
        private int frameCount = 1;

        // The frame currently being displayed. 
        // This value ranges from 0 to iFrameCount-1
        private int currentFrame = 0;

        // Amount of time (in seconds) to display each frame
        private float frameLength = 0.2f;


        // Amount of time that has passed since we last animated
        private float frameTimer = 0.0f;

        // The number of times this animation has been played
        private int playCount = 0;

        // The animation that should be played after this animation
        private string nextAnimation = null;

        private bool isDone = false;

        private bool isAttack = false;
        
        public Texture2D Texture
        {
            get { return t2dTexture; }
            set { t2dTexture = value; }
        }

        public Rectangle RectInitialFrame
        {
            get { return rectInitialFrame; }
            set { rectInitialFrame = value; }
        }

        /// 
        /// The number of frames the animation contains
        /// 
        public int FrameCount
        {
            get { return frameCount; }
            set { frameCount = value; }
        }

        public float FrameTimer
        {
            get { return frameTimer; }
            set { frameTimer = value; }
        }

        /// 
        /// The time (in seconds) to display each frame
        /// 
        public float FrameLength
        {
            get { return frameLength; }
            set { frameLength = value; }
        }

        /// 
        /// The frame number currently being displayed
        /// 
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frameCount - 1); }
        }

        public int FrameWidth
        {
            get { return rectInitialFrame.Width; }
        }

        public int FrameHeight
        {
            get { return rectInitialFrame.Height; }
        }

        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }

        public bool IsAttack
        {
            get { return isAttack; }
            set { isAttack = value; }
        }

        // Temp stuff, should probably get rid of this
        //
        public int StartFrame
        {
            get;
            set;
        }

        /// 
        /// The rectangle associated with the current
        /// animation frame.
        /// 
        public Rectangle FrameRectangle
        {
            get
            {
                return new Rectangle(
                    rectInitialFrame.X + (rectInitialFrame.Width * (currentFrame % columns)),
                    rectInitialFrame.Y + (rectInitialFrame.Height * (currentFrame / columns)), rectInitialFrame.Width, rectInitialFrame.Height);
            }
        }

        public Rectangle PrevFrameRectangle
        {
            get
            {
                return new Rectangle(
                    rectInitialFrame.X + (rectInitialFrame.Width * (currentFrame - 2)),
                    rectInitialFrame.Y, rectInitialFrame.Width, rectInitialFrame.Height);
            }
        }
        public Rectangle TestPrevFrameRectangle
        {
            get
            {
                return new Rectangle(
                    rectInitialFrame.X + (rectInitialFrame.Width * (currentFrame - 3)),
                    rectInitialFrame.Y, rectInitialFrame.Width, rectInitialFrame.Height);
            }
        }
        public int PlayCount
        {
            get { return playCount; }
            set { playCount = value; }
        }

        public string NextAnimation
        {
            get { return nextAnimation; }
            set { nextAnimation = value; }
        }
       
        public virtual Boolean isLastFrameOfAnimation()
        {
            return currentFrame == frameCount - 1;
        }

        public virtual void Update(GameTime gameTime)
        {
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (frameTimer > frameLength)
            {
                frameTimer = 0.0f;
                currentFrame = (currentFrame + 1) % frameCount;

                if (currentFrame == 0)
                {
                    playCount = (int)MathHelper.Min(playCount + 1, int.MaxValue);
                    IsDone = true;
                }
            
            }
        }

        object ICloneable.Clone()
        {
            return null;
        }
    }
}
