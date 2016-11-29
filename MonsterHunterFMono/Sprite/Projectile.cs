using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public class Projectile
    {
        public Texture2D DummyTexture { get; set; }
        public Boolean Finished { get; set; }
        public Direction Direction { get; set; }

        // These are overrides of the particle sprites. 
        //
        public int NumOfHits { get; set; }
        public int XSpeed { get; set; }
        public int YSpeed { get; set; }

        public Boolean PlayOnce { get; set; }

        Dictionary<string, ProjectileAnimation> animations;

        // Which FrameAnimation from the dictionary above is playing
        string currentAnimation = null;

        Vector2 v2Position = new Vector2(0, 0);
        Vector2 v2Center;
        int hitSlowdown = 0;

        float sizeRefactor = 1f;

        public float SizeRefactor
        {
            get { return sizeRefactor; }
            set { sizeRefactor = value; }
        }
        public int PlayerNumber {get; set;}
        public Rectangle Hitbox { get; set; }

        public int Timer { get; set; }

        public Projectile()
        {
            Finished = false;
            animations = new Dictionary<string, ProjectileAnimation>();
        }

        public string CurrentAnimation
        {
            get { return currentAnimation; }
            set 
            { 
                currentAnimation = value;
                NumOfHits = animations[currentAnimation].NumOfHits;
                XSpeed = animations[currentAnimation].XSpeed;
                YSpeed = animations[currentAnimation].YSpeed;
                Timer = animations[currentAnimation].TimerLength;
                PlayOnce = animations[currentAnimation].PlayOnce;
                animations[currentAnimation].CurrentFrame = 0;
                animations[currentAnimation].PlayCount = 0;
                Finished = false;
                Hitbox = new Rectangle();
                Hitbox info = CurrentProjectile.CurrentHitboxInfo;
                if (info != null)
                {
                    Hitbox = info.getHitboxRectangle(Hitbox, Direction.Right, v2Position, CurrentProjectile.FrameWidth);
                }
            }
        }

        public ProjectileAnimation CurrentProjectile
        {
            get
            {
                if (!string.IsNullOrEmpty(currentAnimation))
                    return animations[currentAnimation];
                else
                    return null;
            }
        }

        public Vector2 Position
        {
            get { return v2Position; }
            set
            {                
                v2Position = value;
            }
        }
        public int X
        {
            get { return (int)v2Position.X; }
            set
            {
                v2Position.X = value;
                Hitbox info = CurrentProjectile.CurrentHitboxInfo;
                if (info != null)
                {
                    Hitbox = info.getHitboxRectangle(Hitbox, Direction.Right, v2Position, CurrentProjectile.FrameWidth);
                }
            }
        }

        ///
        /// The Y position of the sprite's upper left corner pixel.
        ///
        public int Y
        {
            get { return (int)v2Position.Y; }
            set
            {
                v2Position.Y = value;
                Hitbox info = CurrentProjectile.CurrentHitboxInfo;
                if (info != null)
                {
                    Hitbox = info.getHitboxRectangle(Hitbox, Direction.Right, v2Position, CurrentProjectile.FrameWidth);
                }
            }
        }
   
        public void Update(GameTime gameTime)
        {
            CurrentProjectile.Update(gameTime);
            Timer--;
            if (Direction == Direction.Right)
            {
                if (hitSlowdown > 0)
                {
                    X += XSpeed / 5;
                }
                else
                {
                    X += XSpeed;  
                }
                  
            }
            else 
            {
                X -= XSpeed;
            }
            Y += YSpeed;
            Hitbox = new Rectangle();
            Hitbox info = CurrentProjectile.CurrentHitboxInfo;
            if (info != null)
            {
                Hitbox = info.getHitboxRectangle(Hitbox, Direction.Right, v2Position, CurrentProjectile.FrameWidth);
            }
            
            if (hitSlowdown > 0)
            {
                hitSlowdown--;
            }
            if (PlayOnce)
            {
                Finished = CurrentProjectile.IsDone;
            }
            else
            {
                if (Timer <= 0)
                {
                    Finished = true;
                }
                if (CurrentProjectile.PlayCount > 0)
                {
                    // If there is, see if the currently playing animation has
                    // completed a full animation loop
                    //
                    if (!String.IsNullOrEmpty(CurrentProjectile.NextAnimation))
                    {
                        // If it has, set up the next animation
                        CurrentAnimation = CurrentProjectile.NextAnimation;
                    }
                }
            }
            
        }

        public void hitEnemy()
        {
            hitSlowdown = 20;
            NumOfHits--;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Direction == Direction.Left)
            {
                spriteBatch.Draw(CurrentProjectile.Texture, (v2Position + v2Center),
                                 CurrentProjectile.FrameRectangle, Color.White,
                                 0, v2Center, sizeRefactor, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(CurrentProjectile.Texture, (v2Position + v2Center),
                                               CurrentProjectile.FrameRectangle, Color.White,
                                               0, v2Center, sizeRefactor, SpriteEffects.None, 0);
            }
            Color translucentRed = Color.Red * 0.2f;
            
           //spriteBatch.Draw(DummyTexture, Hitbox , translucentRed);  
        }

        internal void AddProjectile(String name, ProjectileAnimation projectileAnimation)
        {
            animations.Add(name, projectileAnimation);
        }
    }
}
