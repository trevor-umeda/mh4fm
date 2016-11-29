using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public class SpriteAnimationManager
    {

        public Texture2D dummyTexture ;
        public Texture2D DummyTexture
        {
            get
            {
                return dummyTexture;
            }
        }

        // True if animations are being played
        bool bAnimating = true;

        Rectangle hitbox;

        Rectangle hurtbox;

        int boundingBoxHeight;
        int boundingBoxWidth;
        Rectangle boundingBox;

        // If set to anything other than Color.White, will colorize
        // the sprite with that color.
        Color colorTint = Color.White;

        // Screen Position of the Sprite
        Vector2 v2Position = new Vector2(0, 0);
        Vector2 v2LastPosition = new Vector2(0, 0);

        // Dictionary holding all of the FrameAnimation objects
        // associated with this sprite.
        Dictionary<string, Move> animations = new Dictionary<string, Move>();

        // Which FrameAnimation from the dictionary above is playing
        string currentAnimation = null;

        // The next animation from the dictionary to be played
        string nextAnimation = null;

        // If true, the sprite will automatically rotate to align itself
        // with the angle difference between it's new position and
        // it's previous position.  In this case, the 0 rotation point
        // is to the right (so the sprite should start out facing to
        // the right.
        bool bRotateByPosition = false;

        // Calcualted center of the sprite
        Vector2 v2Center;

        // Calculated width and height of the sprite
        int iWidth;
        int iHeight;

        int currentXVelocity;

        bool showHitboxes = true;

        ///
        /// Vector2 representing the position of the sprite's upper left
        /// corner pixel.
        ///
        public Vector2 Position
        {
            get { return v2Position; }
            set
            {
                v2LastPosition = v2Position;
                v2Position = value;
            }
        }

        public Vector2 PositionCenter
        {
            get
            {
                int xPos = X + CurrentMoveAnimation.FrameWidth/2;
                int yPos = Y + CurrentMoveAnimation.FrameHeight/2;
                return new Vector2(xPos, yPos);
            }
        }

        ///
        /// The X position of the sprite's upper left corner pixel.
        ///
        public int X
        {
            get { return (int)v2Position.X; }
            set
            {
                v2LastPosition.X = v2Position.X;
                v2Position.X = value;
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
                v2LastPosition.Y = v2Position.Y;
                v2Position.Y = value;
            }
        }

        public int CenterX
        {
            get { return X + Width / 2; }
        }
        public int CenterY
        {
            get { return Y + Height / 2; }
        }

        ///
        /// Width (in pixels) of the sprite animation frames
        ///
        public int Width
        {
            get { return CurrentMoveAnimation.FrameWidth; }
        }

        ///
        /// Height (in pixels) of the sprite animation frames
        ///
        public int Height
        {
            get { return CurrentMoveAnimation.FrameHeight; }
        }

        ///
        /// If true, the sprite will automatically rotate in the direction
        /// of motion whenever the sprite's Position changes.
        ///
        public bool AutoRotate
        {
            get { return bRotateByPosition; }
            set { bRotateByPosition = value; }
        }
        ///
        /// Screen coordinates of the bounding box surrounding this sprite
        ///
        public Rectangle BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }
        public int BoundingBoxWidth
        {
            get { return boundingBoxWidth; }
            set { boundingBoxWidth = value; }
        }
        public int BoundingBoxHeight
        {
            get { return boundingBoxHeight; }
            set { boundingBoxHeight = value; }
        }
        public Rectangle Hitbox
        {
            get { return hitbox; }
            set { hitbox = value; }
        }

        public Rectangle Hurtbox
        {
            get { return hurtbox; }
            set { hurtbox = value; }
        }

        ///
        /// Color value to tint the sprite with when drawing.  Color.White
        /// (the default) indicates no tinting.
        ///
        public Color Tint
        {
            get { return colorTint; }
            set { colorTint = value; }
        }

        ///
        /// True if the sprite is (or should be) playing animation frames.  If this value is set
        /// to false, the sprite will not be drawn (a sprite needs at least 1 single frame animation
        /// in order to be displayed.
        ///
        public bool IsAnimating
        {
            get { return bAnimating; }
            set { bAnimating = value; }
        }

        public int CurrentXVelocity
        {
            get { return currentXVelocity; }
            set { this.currentXVelocity = value; }
        }

        public List<string> AnimationsList
        {
            get { return new List<string>(animations.Keys); }
        }

        ///
        /// The FrameAnimation object of the currently playing animation
        ///
        public Move CurrentMoveAnimation
        {
            get
            {
                if (!string.IsNullOrEmpty(currentAnimation))
                    return animations[currentAnimation];
                else
                    return null;
            }
        }

        ///
        /// The string name of the currently playing animaton.  Setting the animation
        /// resets the CurrentFrame and PlayCount properties to zero.
        ///
        public string CurrentAnimation
        {
            get { return currentAnimation; }
            set
            {
                if (animations.ContainsKey(value))
                {
                    //Change animation only if
                    if (( currentAnimation != value && !animations[value].IsAttack) // The animation is different from the current and the new animation is not an attack
                        || animations[value].IsAttack  // The new animation IS an attack
                        || animations[value].CharacterState == CharacterState.HIT //The new animation is the new character is getting hit
                        )
                    {
                        int previousHeight = 0;
                        int previousWidth = 0;
                        // Before changing animations, we need to take care of some stuff
                        //
                        if (currentAnimation != null && animations[currentAnimation] != null)
                        {
                            previousHeight = animations[currentAnimation].FrameHeight;
                            previousWidth = animations[currentAnimation].FrameWidth;

                            // By default once a animation changes, make sure defaults are set
                            //
                            animations[currentAnimation].HasHitOpponent = false;
                            animations[currentAnimation].Blocked = false;
                        }

                        currentAnimation = value;
                        if (animations[currentAnimation].StartFrame > 0)
                        {
                            animations[currentAnimation].CurrentFrame = animations[currentAnimation].StartFrame;
                        }
                        else
                        {
                            animations[currentAnimation].CurrentFrame = 0;
                        }
                        

                        animations[currentAnimation].PlayCount = 0;
                        nextAnimation = animations[currentAnimation].NextAnimation;
                        animations[currentAnimation].IsDone = false;
                        int currentHeight = animations[currentAnimation].FrameHeight;
                        int currentWidth = animations[currentAnimation].FrameWidth;

                        // Because we render position wise using the top left corner, Sprites with different heights would float off the ground.
                        // What we want is to render starting from the feet
                        //
                        if (previousHeight != 0)
                        {
                            Y += previousHeight - currentHeight;
                        }

                        if (previousWidth != 0)
                        {
                            X += ( previousWidth / 2) - (currentWidth / 2) ;
                        }

                    }
                }
            }
        }

        public SpriteAnimationManager()
        {
          
        }

        public void AddAnimation(Texture2D texture, string Name, int X, int Y, int Width, int Height, int Frames, int columns, float FrameLength, CharacterState characterState)
        {
            if (characterState == CharacterState.HIT || characterState == CharacterState.BLOCK)
            {
                animations.Add(Name, new HitAnimation(texture, X, Y, Width, Height, Frames, columns, FrameLength, characterState));
            }
            else
            {
                animations.Add(Name, new Move(texture, X, Y, Width, Height, Frames, columns, FrameLength, characterState));
            }

            iWidth = Width;
            iHeight = Height;
            v2Center = new Vector2(iWidth / 2, iHeight / 2);
        }

        public void AddAnimation(string Name, Move move)
        {
            animations.Add(Name, move);
        }

    

        public void AddResetInfo(String moveName, int index)
        {
            animations[moveName].AddResetInfo(index, true);
        }

        public void AddHitbox(String moveName, int index, Hitbox hitbox)
        {
            animations[moveName].AddHitboxInfo(index, hitbox);
        }

        public void AddHurtbox(String moveName, int index, Hitbox hitbox)
        {
            animations[moveName].AddHurtboxInfo(index, hitbox);
        }

        public HitInfo SetAttackMoveProperties(String moveName, HitInfo hitInfo)
        {
            animations[moveName].HitInfo = hitInfo;
            return animations[moveName].HitInfo;
        }

        public HitInfo SetAttackMoveProperties(String moveName, int hitstun, int blockstun, Hitzone hitzone)
        {
            HitInfo hitInfo = new HitInfo(hitstun, blockstun, hitzone);
            animations[moveName].HitInfo = hitInfo;
            return animations[moveName].HitInfo;
        }

        public Move GetAnimationByName(string Name)
        {
            if (animations.ContainsKey(Name))
            {
                return animations[Name];
            }
            else
            {
                return null;
            }
        }

        public Boolean isLastFrameOfAnimation()
        {
            return CurrentMoveAnimation.isLastFrameOfAnimation();
        }

        public void MoveBy(int x, int y)
        {
            v2LastPosition = v2Position;
            v2Position.X += x;
            v2Position.Y += y;
            currentXVelocity += x;
        }

        public void setXByBoundingBox(int boundingBoxX)
        {       
            X = boundingBoxX + boundingBoxWidth / 2 - Width / 2;
        }

       
        public String retrieveBackupMove(String moveName)
        {
           return animations[moveName].BackupMove;
        }

        public void Update(GameTime gameTime, Direction direction)
        {

            // Don't do anything if the sprite is not animating
            if (bAnimating)
            {
                // If there is not a currently active animation
                if (CurrentMoveAnimation == null)
                {
                    // Make sure we have an animation associated with this sprite
                    if (animations.Count > 0)
                    {
                        // Set the active animation to the first animation
                        // associated with this sprite
                        string[] sKeys = new string[animations.Count];
                        animations.Keys.CopyTo(sKeys, 0);
                        CurrentAnimation = sKeys[0];
                    }
                    else
                    {
                        return;
                    }
                }
                // Run the Animation's update method
                CurrentMoveAnimation.Update(gameTime);
                Hitbox info = CurrentMoveAnimation.CurrentHitboxInfo;
                // Ready hitbox info
                //

                if (info != null)
                {
                    hitbox = info.getHitboxRectangle(hurtbox, direction, v2Position, CurrentMoveAnimation.FrameWidth);
                }
                else
                {
                    hitbox = new Rectangle();
                }

                Hitbox hurtboxInfo = CurrentMoveAnimation.CurrentHurtboxInfo;
                // Set up the hurtbox for the move
                //
                if (hurtboxInfo != null)
                {
                    hurtbox = hurtboxInfo.getHitboxRectangle(hurtbox, direction, v2Position, CurrentMoveAnimation.FrameWidth);
                }
                else
                {
                    hurtbox = new Rectangle();
                }

                // Calculate where our bounding box is. Calculating this every cycle hopefully doesn't add too much overhead
                //
                if (BoundingBoxHeight != null)
                {
       
                    boundingBox.Height = boundingBoxHeight;
                    boundingBox.Width = boundingBoxWidth;

                    boundingBox.X = CenterX - boundingBoxWidth/2;
                    boundingBox.Y = (int)v2Position.Y + Height -  boundingBoxHeight;
                }
                else
                {
                    boundingBox = new Rectangle();
                }
            
                // Check to see if there is a "followup" animation named for this animation
                //
                if (!String.IsNullOrEmpty(CurrentMoveAnimation.NextAnimation))
                {
                    // If there is, see if the currently playing animation has
                    // completed a full animation loop
                    //
                    if (CurrentMoveAnimation.IsDone)
                    {
                        Console.WriteLine("SWITCHING TO " + CurrentMoveAnimation.NextAnimation);
                        // If it has, set up the next animation
                        CurrentAnimation = CurrentMoveAnimation.NextAnimation;
                    }
                }
            }
        }

        public void shadowDraw(SpriteBatch spriteBatch, int XOffset, int YOffset, Direction direction)
        {
            if (CurrentMoveAnimation.CurrentFrame > 2)
            {
                if (direction == Direction.Right)
                {
                    spriteBatch.Draw(CurrentMoveAnimation.Texture, (v2Position + new Vector2(XOffset, YOffset) + v2Center),
                                                  CurrentMoveAnimation.PrevFrameRectangle, colorTint * 0.5f,
                                                  0, v2Center, 1f, SpriteEffects.None, 0);                   
                }
                else
                {
                    spriteBatch.Draw(CurrentMoveAnimation.Texture, (v2Position + new Vector2(XOffset, YOffset) + v2Center),
                                                   CurrentMoveAnimation.PrevFrameRectangle, colorTint * 0.5f,
                                                   0, v2Center, 1f, SpriteEffects.FlipHorizontally, 0);
                }
            }
            if (CurrentMoveAnimation.CurrentFrame > 3)
            {
                if (direction == Direction.Right)
                {
                    spriteBatch.Draw(CurrentMoveAnimation.Texture, (v2Position + new Vector2(XOffset, YOffset) + v2Center),
                                                  CurrentMoveAnimation.TestPrevFrameRectangle, colorTint * 0.5f,
                                                  0, v2Center, 1f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(CurrentMoveAnimation.Texture, (v2Position + new Vector2(XOffset, YOffset) + v2Center),
                                                   CurrentMoveAnimation.TestPrevFrameRectangle, colorTint * 0.5f,
                                                   0, v2Center, 1f, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset, Direction direction)
        {
            if (bAnimating)
            {
                if (direction == Direction.Right)
                {
                    spriteBatch.Draw(CurrentMoveAnimation.Texture, (v2Position + new Vector2(XOffset, YOffset) + v2Center),
                                                    CurrentMoveAnimation.FrameRectangle, colorTint,
                                                    0, v2Center, 1f, SpriteEffects.None, 0);
                    
                }
                else
                {
                    spriteBatch.Draw(CurrentMoveAnimation.Texture, (v2Position + new Vector2(XOffset, YOffset) + v2Center),
                                                   CurrentMoveAnimation.FrameRectangle, colorTint,
                                                   0, v2Center, 1f, SpriteEffects.FlipHorizontally, 0);
                }

                if (showHitboxes)
                {
                    Color translucentRed = Color.Red * 0.5f;
                   // spriteBatch.Draw(dummyTexture, hitbox, translucentRed);

                    Color translucentBlue = Color.Blue * 0.5f;
                    //spriteBatch.Draw(dummyTexture, hurtbox, translucentBlue);

                    Color DarkBlue = Color.Blue * 0.7f;
                    //spriteBatch.Draw(dummyTexture, boundingBox, translucentBlue);
                }
            }
        }
    }
}
