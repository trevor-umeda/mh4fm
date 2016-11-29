using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
namespace MonsterHunterFMono
{
    public abstract class Player
    {
         // The SpriteAnimation object that holds the graphical and animation data for this object
        SpriteAnimationManager sprite;

        SpecialInputManager specialInputManager;

        ComboManager ComboManager { get; set; }

        ThrowManager ThrowManager { get; set; }

        public SuperManager SuperManager { get; set; }

        public ProjectileManager ProjectileManager { get; set; }

        SoundManager SoundManager { get; set; }

        InputMoveBuffer InputMoveBuffer { get; set; }

        public int ThrowRange { get; set; }

        ControlSetting controlSetting;

        public int PlayerNumber { get; set; }

        // The speed at which the sprite will close with it's target
        float speed = 1f;

        KeyboardState prevKeyboardState;

        Direction directionFacing;

        // These two integers represent a clipping range for determining bounding-box style
        // collisions.  They return the bounding box of the sprite trimmed by a horizonal and
        // verticle offset to get a collision cushion
        int collisionBufferX = 0;
        int collisionBufferY = 0;

        // Health
        //
        Gauge healthBar;
  
        // Special bar
        //
        Gauge specialBar;

        // Determine the status of the sprite.  An inactive sprite will not be updated but will be drawn.
        bool active = true;

        // Determines if the sprite should track towards a v2Target.  If set to false, the sprite
        // will not move on it's own towards v2Target, and will not process pathing information
        bool movingTowardsTarget = true;

        // If true, the sprite can collide with other objects.  Note that this is only provided as a flag
        // for testing with outside code.
        bool bCollidable = true;

        // If true, the sprite will be drawn to the screen
        bool bVisible = true;

        // Whether the character is crouching
        bool isCrouching = false;

        Vector2 currentVelocity = new Vector2(0, 0);

        bool isInInterruptableAnimation = false;


        //
        /// <summary>
        ///  Configuralbe stuffs
        /// </summary>
        readonly Vector2 gravityModifierV = new Vector2(0, 2500f);

        readonly Vector2 initialJumpSpeed = new Vector2(0, -1200);

        readonly float jumpHorizontalSpeed = 350f;

        public int BackAirDashVel { get; set; }
        public int AirDashVel { get; set; }
        public int BackStepVel { get; set; }
        public int DashVel { get; set; }
        public int BackWalkVel { get; set; }
        public int WalkVel { get; set; }
        // Some hacky stuff to give a player slight momentum
        //
        int momentumCounter = 0;

        int momentumXMovement = 0;

        int currentXVelocity = 0;

        int airJumpLimit = 1;
        int timesJumped = 0;

        int untechTime = 0;

        Vector2 startingPosition;
        public Player(int playerNumber, int xPosition, int yHeight, ComboManager comboManager, ThrowManager throwManager, Gauge healthBar)
        {
            sprite = new SpriteAnimationManager();
            PlayerNumber = playerNumber;
            Position = new Vector2(xPosition, Config.Instance.GroundYHeight - yHeight);
            startingPosition = new Vector2(xPosition, Config.Instance.GroundYHeight - yHeight);
            ComboManager = comboManager;
            ThrowManager = throwManager;
            specialInputManager = new SpecialInputManager();
            SoundManager = new SoundManager();
            ControlSetting = new ControlSetting();
            InputMoveBuffer = new InputMoveBuffer();
            IsPhysical = true;
            HealthBar = healthBar;
        }

        public ControlSetting ControlSetting
        {
            get { return controlSetting; }
            set
            {
                this.controlSetting = value;
                specialInputManager.ControlSetting = value;
            }
        }

        public bool IsAirborne
        {
            get {
                return Y + Sprite.CurrentMoveAnimation.FrameHeight < Config.Instance.GroundYHeight - 30;
            }
        }

        public bool isGettingHit
        {
            get {
                return (Sprite.CurrentMoveAnimation.CharacterState == CharacterState.HIT || Sprite.CurrentMoveAnimation.CharacterState == CharacterState.KNOCKDOWN ||
                Sprite.CurrentMoveAnimation.CharacterState == CharacterState.AIRBORNEHIT);
            }
        }

        // Are we in a move that we can cancel
        //
        public bool IsCancealableMove
        {
            get { return Sprite.CurrentMoveAnimation != null &&
                (!isGettingHit) && // If we aren't getting hit
                (Sprite.CurrentMoveAnimation.IsAttack && Sprite.CurrentMoveAnimation.IsDone || // or if the move is done
                Sprite.CurrentMoveAnimation.CharacterState == CharacterState.DASHING  || Sprite.CurrentMoveAnimation.CharacterState == CharacterState.AIRDASH || // or if the current move is dashing
                !Sprite.CurrentMoveAnimation.IsAttack);  // or if the  move is not an attack
              }
        }

        public CharacterState CharacterState
        {
            get { return Sprite.CurrentMoveAnimation.CharacterState; }
        }

        public bool HasHitOpponent
        {
            get { return Sprite.CurrentMoveAnimation.HasHitOpponent; }
            set { Sprite.CurrentMoveAnimation.HasHitOpponent = value; }
        }

        public bool AttackWasBlocked
        {
            get { return Sprite.CurrentMoveAnimation.Blocked; }
            set { Sprite.CurrentMoveAnimation.Blocked = value; }
        }

        public Rectangle BoundingBox
        {
            get { return sprite.BoundingBox; }
        }

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(
                    sprite.BoundingBox.X + collisionBufferX,
                    sprite.BoundingBox.Y + collisionBufferY,
                    sprite.Width - (2 * collisionBufferX),
                    sprite.Height - (2 * collisionBufferY));
            }
        }

        // Methods to set up character
        //
        public void RegisterGroundMove(String name, List<String> input)
        {
            SpecialInputManager.registerGroundMove(name, input);
        }

        public void RegisterAirMove(String name, List<String> input)
        {
            SpecialInputManager.registerAirMove(name, input);
        }

        public virtual void SetUpUniversalAttackMoves()
        {
            // TODO see if 6b is universal. if not it complicates things...
            // All these commands are universal so input em here! Is 6B a universal? hmm....
            //
            RegisterGroundMove("forwardthrow", new List<string> { ThrowManager.ForwardThrowInput });
            RegisterGroundMove("backthrow", new List<string> { ThrowManager.BackThrowInput });
           // RegisterGroundMove("forwardaattack", new List<string> { "6A" });
            RegisterGroundMove("specialcommand", new List<string> { "BC" });
         //   RegisterGroundMove("forwardbattack", new List<string> { "6B" });
           // RegisterGroundMove("forwardcattack", new List<string> { "6C" });
            RegisterGroundMove("2aattack", new List<string> { "2A" });
            RegisterGroundMove("2battack", new List<string> { "2B" });
            RegisterGroundMove("2cattack", new List<string> { "2C" });
   
            RegisterGroundMove("cattack", new List<string> { "C" });
            RegisterGroundMove("battack", new List<string> { "B" });
            RegisterGroundMove("aattack", new List<string> { "A" });

            RegisterAirMove("jaattack", new List<string> { "A" });
            RegisterAirMove("jbattack", new List<string> { "B" });
            RegisterAirMove("jcattack", new List<string> { "C" });
        }

        public void SetAttackMoveProperties(String moveName, HitInfo hitInfo)
        {
            sprite.SetAttackMoveProperties(moveName, hitInfo);
        }

        public void SetAttackMoveProperties(String moveName, int hitstun, int blockstun, Hitzone hitzone, int damage)
        {
            HitInfo hitInfo = sprite.SetAttackMoveProperties(moveName, hitstun, blockstun, hitzone);
            hitInfo.Damage = damage;
        }

        public void AddSound(SoundEffect sound, String name)
        {
            SoundManager.AddSound(sound, name);
        }

        // Methods to modify player movement and such
        //
        public void GivePlayerMomentum(int timeToGiveMomentum, int amountOfMomentum, bool momentumInDirectionFacing)
        {
            MomentumCounter = timeToGiveMomentum;
            int momentumValue = amountOfMomentum;
            if ((momentumInDirectionFacing && Direction == Direction.Left) || (!momentumInDirectionFacing && Direction == Direction.Right))
            {
                MomentumXMovement = -momentumValue;
            }
            else
            {
                MomentumXMovement = momentumValue;
            }
        }


        public void processBasicMovement(GameTime gameTime, KeyboardState ks)
        {
            // I dislike having this in the player class, but atm it fits really well.
            //
            if (Sprite.CurrentMoveAnimation == null ||
                Sprite.CurrentMoveAnimation.CharacterState != CharacterState.AIRBORNE)
            {
                if (IsCancealableMove)
                {

                    if (ks.IsKeyDown(controlSetting.Controls["down"]))
                    {
                        //Console.WriteLine("CROUCHING");
                        Crouch();
                    }
                    else
                    {
                        UnCrouch();

                        if (ks.IsKeyDown(controlSetting.Controls["right"]))
                        {
                            if (Direction == Direction.Right)
                            {
                                ForwardWalk();
                            }
                            else
                            {
                                BackWalk();
                            }
                        }
                        if (ks.IsKeyDown(controlSetting.Controls["left"]))
                        {
                            if (Direction == Direction.Right)
                            {
                                BackWalk();
                            }
                            else
                            {
                                ForwardWalk();
                            }
                        }
                        //if (ks.IsKeyDown(controlSetting.Controls["up"]))
                        //{
                        //    if (ks.IsKeyDown(controlSetting.Controls["right"]))
                        //    {
                        //        Jump(Direction.Right);
                        //    }
                        //    else if (ks.IsKeyDown(controlSetting.Controls["left"]))
                        //    {
                        //        Jump(Direction.Left);
                        //    }
                        //    else
                        //    {
                        //        Jump();
                        //    }
                        //}
                    }
                }
                if (!ks.IsKeyDown(controlSetting.Controls["right"]) && !ks.IsKeyDown(controlSetting.Controls["left"]) && !ks.IsKeyDown(controlSetting.Controls["down"]) && !ks.IsKeyDown(controlSetting.Controls["up"]) && Sprite.CurrentMoveAnimation != null)
                {
                    if (IsCancealableMove)
                    {
                        Neutral();
                    }

                }
            }
            else if (Sprite.CurrentMoveAnimation == null ||
                Sprite.CurrentMoveAnimation.CharacterState == CharacterState.AIRBORNE)
            {
                //if (IsCancealableMove && timesJumped < airJumpLimit && prevKeyboardState.IsKeyUp(controlSetting.Controls["up"]) && ks.IsKeyDown(controlSetting.Controls["up"]))
                //{
                    
                //    timesJumped++;
                //    Console.WriteLine(timesJumped);
                //    if (ks.IsKeyDown(controlSetting.Controls["right"]))
                //    {
                //        AirJump(Direction.Right);
                //    }
                //    else if (ks.IsKeyDown(controlSetting.Controls["left"]))
                //    {
                //        AirJump(Direction.Left);
                //    }
                //    else
                //    {
                //        AirJump();
                //    }
                //}
            }
        }

        public void Update(GameTime gameTime, KeyboardState ks, Boolean inHitstop)
        {

            Sprite.CurrentXVelocity = 0;

            // Might not be necessary
            //
            if (Sprite.CurrentMoveAnimation != null)
            {
                determineCurrentMove(gameTime, ks, inHitstop);
            }
            if (!inHitstop)
            {
              handlePerformCurrentMove(gameTime, ks);
                // This is basically for jumps as only jump movement is calculated via velocity
                //
               float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
               Position += CurrentVelocity * time;

               sprite.Update(gameTime, Direction);

               HealthBar.Update(gameTime);
               SpecialBar.Update(gameTime);
               handleExtraMovement();
               cleanUp();
           }
            prevKeyboardState = Keyboard.GetState();
        }

        public void UpdateWithoutInput(GameTime gameTime)
        {
            Sprite.CurrentXVelocity = 0;

            // Might not be necessary
            //
            //if (Sprite.CurrentMoveAnimation != null)
            //{
            //    determineCurrentMove(gameTime, ks, inHitstop);
            //}
           
            //handlePerformCurrentMove(gameTime, ks);
            // This is basically for jumps as only jump movement is calculated via velocity
            //
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += CurrentVelocity * time;

            sprite.Update(gameTime, Direction);

            handleExtraMovement();
            cleanUp();
        }

        public void determineCurrentMove(GameTime gameTime, KeyboardState ks, Boolean inHitstop)
        {

          String moveName = SpecialInputManager.checkMoves(Sprite.CurrentMoveAnimation.CharacterState, Direction, Sprite.CurrentAnimation, ks);
        
          // See if we are in a state to change moves
          //
          if(IsCancealableMove || // Are we doing an action that can be canceled
              HasHitOpponent || // We can also cancel a move if we hit an opponent
              InputMoveBuffer.getBufferedMove() != null)
          {
              if (moveName == null && InputMoveBuffer.getBufferedMove() == null)
              {
                  // Handle basic stuff like moving, jumping and crouching
                  //
                  processBasicMovement(gameTime, ks);
              }
              else if (moveName == "jumpcancel")
              {
                  if (Sprite.CurrentMoveAnimation.CharacterState != CharacterState.AIRBORNE)
                  {
                      if ((IsCancealableMove || HasHitOpponent))
                      {
                          if (ks.IsKeyDown(controlSetting.Controls["right"]))
                          {
                              Jump(Direction.Right);
                          }
                          else if (ks.IsKeyDown(controlSetting.Controls["left"]))
                          {
                              Jump(Direction.Left);
                          }
                          else
                          {
                              Jump();
                          }
                          if (Direction == Direction.Right)
                          {
                              ProjectileManager.createJumpParticle(Position.X, Position.Y + Sprite.CurrentMoveAnimation.FrameHeight, Direction.Left);
                          }
                          else
                          {
                              ProjectileManager.createJumpParticle(Position.X + Sprite.CurrentMoveAnimation.FrameWidth, Position.Y + Sprite.CurrentMoveAnimation.FrameHeight, Direction.Right);
                          }
                      }
                  }
                  else if (Sprite.CurrentMoveAnimation == null ||
                            Sprite.CurrentMoveAnimation.CharacterState == CharacterState.AIRBORNE)
                  {
                      // Jump canceling moves
                      //
                      if ((IsCancealableMove || HasHitOpponent) && timesJumped < airJumpLimit && prevKeyboardState.IsKeyUp(controlSetting.Controls["up"]) && ks.IsKeyDown(controlSetting.Controls["up"]))
                      {

                          timesJumped++;
                          Console.WriteLine(timesJumped);
                          if (ks.IsKeyDown(controlSetting.Controls["right"]))
                          {
                              AirJump(Direction.Right);
                          }
                          else if (ks.IsKeyDown(controlSetting.Controls["left"]))
                          {
                              AirJump(Direction.Left);
                          }
                          else
                          {
                              AirJump();
                          }
                      }
                  }
              }
              else
              {
                  UnCrouch();

                  // If we're trying to do a throw, see if we actually can. if not then change it.
                  //
                  if (moveName == "forwardthrow" || moveName == "backthrow")
                  {
                      // Have the throw manager see if the throw is valid
                      //
                      if (!ThrowManager.isValidThrow(PlayerNumber))
                      {
                          // If its not then have the move perform a throw whiff move
                          // This can work for either 1 button or 2 button
                          //
                          if (moveName == "forwardthrow")
                          {
                              moveName = ThrowManager.ForwardThrowWhiffMove;
                              Console.WriteLine("OOPS WAS NOT A THROW DOING FORWARD C");
                          }
                          else
                          {
                              moveName = ThrowManager.BackThrowWhiffMove;
                          }
                      }
                  }

                  // If we're in hitstop, queue up the move.
                  //
                  if (inHitstop)
                  {
                      if (moveName != null)
                      {
                          Console.WriteLine("Queueing up : " + moveName);
                          InputMoveBuffer.setBufferedMove(moveName);
                      }
                  }
                  // If we're not in hitstop then we're free to perform things
                  //
                  else
                  {
                      // If its appropriate and we have a move queued up then it takes priority
                      //
                      if ((IsCancealableMove || HasHitOpponent) && InputMoveBuffer.getBufferedMove() != null)
                      {
                          Console.WriteLine("Unbuffering Move");
                          checkValidityAndChangeMove(InputMoveBuffer.getBufferedMove());
                          InputMoveBuffer.unbufferCurrentMove();
                      }
                      // Otherwise perform the current move
                      //
                      else if (moveName != null)
                      {
                          if (moveName != "backstep" && moveName != "dash")
                          {
                              checkValidityAndChangeMove(moveName);
                          }
                          else if (timesJumped < 1)
                          {
                              // Doing an airdash or back step
                              //
                              SoundManager.PlaySound(moveName);
                              if(moveName == "dash" && Sprite.CurrentAnimation != "dash")
                              {
                                  ProjectileManager.createDashParticle(Position.X, Position.Y + Sprite.CurrentMoveAnimation.FrameHeight, Direction);
                              }
                              Sprite.CurrentAnimation = moveName;

                              // If we're in the air when we do it, note we jumped up
                              //
                              if (IsAirborne)
                              {
                                  if (Sprite.CurrentAnimation == "dash")
                                  {
                                      Sprite.CurrentAnimation = "airdashstart";
                                      Console.WriteLine(" Start Dashing in the air");
                                  }

                                  timesJumped++;
                              }
                          }
                      }
                  }
              }
          }
          // If we've input a move but cant cancel it, then put it in our buffer
          //
          else if(moveName != null && Sprite.CurrentMoveAnimation.IsAttack && moveName != "backstep")
          {
              Console.WriteLine("Queueing up : " + moveName);
              InputMoveBuffer.setBufferedMove(moveName);
          }
        }

        public void handlePerformCurrentMove(GameTime gameTime, KeyboardState ks)
        {
          // Parse the ground special inputs here
          //   
          if (!IsAirborne)
          {
              performGroundSpecialMove(ks, Sprite.CurrentAnimation);
          }
          if (IsAirborne)
          {
              // Do airdash stuff
              //
              if (Sprite.CurrentAnimation == "backstep")
              {
                  AirBackdash();
              }
              else if (Sprite.CurrentAnimation == "airdash" || Sprite.CurrentAnimation == "airdashstart")
              {
                  Console.WriteLine("DASHING IN THE AIR");
                  AirDash();
              }
              else
              {
                  // If not doing an airdash, do typical jump movement
                  //
                  AirborneMovement(gameTime);

                  // Check to see if we can tech here
                  //
                  if (isGettingHit && untechTime <= 0)
                  {
                      if(ks.IsKeyDown(controlSetting.Controls["a"]))
                      {
                          Console.WriteLine("DID A TECH");
                          Sprite.CurrentAnimation = "standing";
                      }
                  }
              }
          }
        }

        public void handleExtraMovement()
        {
          // Move the player a bit for a small amount of time
          //
          if (momentumCounter > 0)
          {
              momentumCounter--;
              sprite.MoveBy(momentumXMovement, 0);
          }
          if (momentumCounter == 0)
          {
              momentumXMovement = 0;

          }
  
           // Logic to handle when they are landing
           // If bottom of sprite is touching the "floor" then you are landed
           //
          if (Y + Sprite.CurrentMoveAnimation.FrameHeight >= Config.Instance.GroundYHeight && currentVelocity.Y > 0)
          {
              // Stop velocity
              //
              CurrentVelocity = new Vector2(0, 0);

              // Once they land, make sure we reset how many times we've jumped
              //
              timesJumped = 0;

              // If they were getting hit though, they'll hit the ground instead of just landing
              //
              if (CharacterState.KNOCKDOWN == Sprite.CurrentMoveAnimation.CharacterState ||
                  CharacterState.AIRBORNEHIT == sprite.CurrentMoveAnimation.CharacterState)
              {
                  Sprite.CurrentAnimation = "hitground";
                  if (CurrentHealth <= 0)
                  {
                      Sprite.CurrentAnimation = "KO";
                  }
              }
              else
              {
                  Sprite.CurrentAnimation = "standing";

              }
              // Set their position to be on the ground depending on what animation they are in
              //
              Position = new Vector2(Position.X, Config.Instance.GroundYHeight - Sprite.CurrentMoveAnimation.FrameHeight);
          }
        }

        public void PerformSuperFreeze()
        {
            SuperManager.performSuper(PlayerNumber, Sprite.PositionCenter);
        }

        public virtual void Backstep()
        {
            if (Direction == Direction.Left)
            {
                Sprite.MoveBy(BackStepVel, 0);
            }
            else
            {
                Sprite.MoveBy(-BackStepVel, 0);
            }
        }
        public virtual void Dash()
        {

            if (Direction == Direction.Left)
            {
                Sprite.MoveBy(-DashVel, 0);
            }
            else
            {
                Sprite.MoveBy(DashVel, 0);
            }
            
        }

        public virtual void cleanUp()
        {
            InputMoveBuffer.decrementBufferTimer();
        }

        public abstract void checkValidityAndChangeMove(String moveName);

        public String determineBackupMove(String moveName)
        {
            return Sprite.retrieveBackupMove(moveName);
        }

        public virtual void changeMove(String moveName)
        {
            // Sub class must overwrite this... and perform this base to actually switch moves;
            //
            
            if (moveName != null)
            {
                Console.WriteLine("Changing moves to " + moveName);
                HasHitOpponent = false;
                Sprite.CurrentAnimation = moveName;
            }
            
        }

        public virtual void performGroundSpecialMove(KeyboardState ks, String moveName)
        {
            if (Sprite.CurrentAnimation == "backstep")
            {
                Backstep();
            }
            if (Sprite.CurrentAnimation == "dash" && (IsCancealableMove|| Sprite.CurrentMoveAnimation.CharacterState == CharacterState.HOP))
            {
                
                Dash();
                // If dash and they jump, do a dash jump
                //
                if (ks.IsKeyDown(controlSetting.Controls["up"]))
                {
                    DashJump();
                }
            }
        }

        public virtual void AirBackdash()
        {
            CurrentVelocity = new Vector2(0, 0);
            if (Sprite.isLastFrameOfAnimation())
            {
                if (Direction == Direction.Left)
                {
                    CurrentVelocity = new Vector2(JumpHorizontalSpeed, 200);
                }
                else
                {
                    CurrentVelocity = new Vector2(-JumpHorizontalSpeed, 200); ;
                }
            }

            if (Direction == Direction.Left)
            {
                Sprite.MoveBy(BackStepVel, 0);
            }
            else
            {
                Sprite.MoveBy(-BackStepVel, 0);
            }
        }

        public virtual void AirDash()
        {
            CurrentVelocity = new Vector2(0, 0);
     
            if (Sprite.isLastFrameOfAnimation())
            {

                if (Direction == Direction.Left)
                {
                    CurrentVelocity = new Vector2(-JumpHorizontalSpeed, 200);
                }
                else
                {
                    CurrentVelocity = new Vector2(JumpHorizontalSpeed, 200); ;
                }
            }

            if (Direction == Direction.Left)
            {
                Sprite.MoveBy(-AirDashVel, 0);
                CurrentVelocity = new Vector2(-JumpHorizontalSpeed, 0);
            }
            else
            {
                Sprite.MoveBy(AirDashVel, 0);
                CurrentVelocity = new Vector2(JumpHorizontalSpeed, 0); ;
            }
        }

        public virtual void BackWalk()
        {
            Sprite.CurrentAnimation = "backwalk";
            if (Direction == Direction.Left)
            {
                Sprite.MoveBy(BackWalkVel, 0);
            }
            else
            {
                Sprite.MoveBy(-BackWalkVel, 0);
            }
        }

        public virtual void ForwardWalk()
        {
            Sprite.CurrentAnimation = "walk";
            if (Direction == Direction.Left)
            {
                Sprite.MoveBy(-WalkVel, 0);
            }
            else
            {
                Sprite.MoveBy(WalkVel, 0);
            }
        }

        public virtual void Neutral()
        {
            Sprite.CurrentAnimation = "standing";
        }

        public virtual void Jump(Direction directionJumped)
        {
            if (directionJumped == Direction.Right)
            {
                CurrentVelocity = new Vector2(JumpHorizontalSpeed, 0);
            }
            else if (directionJumped == Direction.Left)
            {
                CurrentVelocity = new Vector2(-JumpHorizontalSpeed, 0);
            }
            Jump();
        }

        public virtual void Jump()
        {
            CurrentVelocity += InitialJumpVelocity;
            Sprite.CurrentAnimation = "jumpup";
        }

        public virtual void AirJump()
        {
            //Lazy atm
            float temp = CurrentVelocity.X;
            CurrentVelocity = new Vector2(temp,InitialJumpVelocity.Y);
            Sprite.CurrentAnimation = "jumpup";
        }

        public virtual void AirJump(Direction directionJumped)
        {
            if (directionJumped == Direction.Right)
            {
                CurrentVelocity = new Vector2(JumpHorizontalSpeed, 0);
            }
            else if (directionJumped == Direction.Left)
            {
                CurrentVelocity = new Vector2(-JumpHorizontalSpeed, 0);
            }
            AirJump();
        }

        public virtual void DashJump()
        {
            if (Direction == Direction.Right)
            {
                CurrentVelocity = new Vector2(jumpHorizontalSpeed * 2, 0);
            }
            else if (Direction == Direction.Left)
            {
                CurrentVelocity = new Vector2(-jumpHorizontalSpeed * 2, 0);
            }
            Jump();
        }

        public virtual void Jumping()
        {
            if (!Sprite.CurrentMoveAnimation.IsAttack)
            {
                if (currentVelocity.Y > 0)
                {
                    Sprite.CurrentAnimation = "jumpdown";
                }
                else if (currentVelocity.Y > -230)
                {
                    Sprite.CurrentAnimation = "jumptop";
                }
            }
           
        }

        public virtual void AirborneMovement(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CurrentVelocity += gravityModifierV * time;
            if (Sprite.CurrentMoveAnimation.CharacterState == CharacterState.HIT || Sprite.CurrentMoveAnimation.CharacterState == CharacterState.KNOCKDOWN
                || Sprite.CurrentMoveAnimation.CharacterState == CharacterState.AIRBORNEHIT)
            {
                // Is there special animation logic for getting hit in the air?
                //
                Console.WriteLine("GETTING HIT IN THE AIR");
                untechTime--;
            }
            else
            {
                Jumping();
            }
        }

        public void Crouch()
        {

            if (!IsCrouching)
            {
                Sprite.CurrentAnimation = "crouching";
            }
            else
            {
                Sprite.CurrentAnimation = "crouchingidle";
            }
            IsCrouching = true;
        }

        public void UnCrouch()
        {
            if (IsCrouching)
            {
                Sprite.CurrentAnimation = "crouchingup";
            }
            IsCrouching = false;

        }

        public virtual Boolean hitByEnemy(KeyboardState keyState, HitInfo hitInfo, Rectangle collisionZone)
        {
            //Check if blocked or not
            //int hitStun, int blockStun, Hitzone hitzone, float? xVel, float? yVel
            if(!hitInfo.Unblockable && (Sprite.CurrentMoveAnimation.CharacterState != CharacterState.HIT || Sprite.CurrentAnimation.Contains("block"))
                && !Sprite.CurrentMoveAnimation.IsAttack && isAttackBlocked(keyState, hitInfo.Hitzone) )
            {
                if (Sprite.CurrentMoveAnimation.CharacterState == CharacterState.AIRBORNE)
                {
                    Sprite.CurrentAnimation = "airblock";
                }
                else if (Sprite.CurrentMoveAnimation.CharacterState == CharacterState.CROUCHING)
                {
                    Sprite.CurrentAnimation = "crouchblock";
                }
                else
                {
                    Sprite.CurrentAnimation = "block";
                }
                
                HitAnimation block = (HitAnimation)Sprite.CurrentMoveAnimation;
                Console.WriteLine("DID A BLOCK!");
                block.reset();
                block.HitStunCounter = hitInfo.Blockstun;
                Console.WriteLine(block.HitStunCounter);
                ProjectileManager.createHitparticle(collisionZone, HitType.BLOCK);
                return false;
            }
            else
            {
                ProjectileManager.createHitparticle(collisionZone, hitInfo.HitType);
                // So many different ways to get hit
                //
                if (Sprite.CurrentMoveAnimation.CharacterState == CharacterState.CROUCHING )
                {
                    Sprite.CurrentAnimation = "crouchhit";
                }
                else if (hitInfo.Hitzone == Hitzone.LOW)
                {
                    Sprite.CurrentAnimation = "lowhit";
                }
                else
                {
                    Sprite.CurrentAnimation = "hit";
                }

               

                Sprite.CurrentMoveAnimation.CurrentFrame = Sprite.CurrentMoveAnimation.StartFrame;
                CurrentHealth -= ComboManager.calculateProratedDamage(hitInfo);
                
                if (hitInfo.FreezeOpponent)
                {
                    Sprite.CurrentAnimation = "freeze";

                }
                else
                {
                    // Not sure if this a bad idea memory wise
                    //
                    HitAnimation hit = (HitAnimation)Sprite.CurrentMoveAnimation;
                    hit.HitStunCounter = ComboManager.calculateProratedHitStun(hitInfo);
                    hit.reset();
                    if (CurrentHealth <= 0)
                    {
                        hitInfo = new HitInfo(100, 10, Hitzone.MID);
                        hitInfo.AirXVelocity = 300;
                        hitInfo.AirYVelocity = -100;
                    }
                    if (IsAirborne || hitInfo.ForceAirborne || CurrentHealth <= 0)
                    {
                        untechTime = hitInfo.AirUntechTime;
                        Sprite.CurrentAnimation = "falldown";
                        Position += new Vector2( 0,-10);
                        if (hitInfo.AirXVelocity != null && hitInfo.AirYVelocity != null)
                        {
                            if (directionFacing == Direction.Right)
                            {
                                CurrentVelocity = new Vector2((float)-hitInfo.AirXVelocity, (float)hitInfo.AirYVelocity);
                            }
                            else
                            {
                                CurrentVelocity = new Vector2((float)hitInfo.AirXVelocity, (float)hitInfo.AirYVelocity);
                            }
                        }
                    }
                    else
                    {
                        GivePlayerMomentum(5, 4, false);
                        if (hitInfo.IsHardKnockDown)
                        {
                            // Make em HARD DOWN
                            //
                            Sprite.CurrentAnimation = "knockdown";

                        }
                    }
                }

                ComboManager.registerHit(hitInfo);
                return true;
            }

        }

        private Boolean isAttackBlocked(KeyboardState keyState, Hitzone hitzone)
        {
            // Check to see if the right direction is held
            //
            if ((directionFacing == Direction.Right && keyState.IsKeyDown(controlSetting.Controls["left"])) ||
                (directionFacing == Direction.Left && keyState.IsKeyDown(controlSetting.Controls["right"])))
            {
                // Check to see if the right zone is held.
                // Right now just check to see if they have down held or not
                //
                if ((hitzone == Hitzone.LOW && keyState.IsKeyDown(controlSetting.Controls["down"])) ||
                    (hitzone == Hitzone.HIGH && keyState.IsKeyUp(controlSetting.Controls["down"])) ||
                    hitzone == Hitzone.MID)
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public virtual void hitEnemy(Boolean attackSuccessful)
        {
            AttackWasBlocked = !attackSuccessful;
            HasHitOpponent = true;
            // TODO make sure to configure this
            //
            giveSpecialMeter(10);
            SoundManager.PlaySound(Sprite.CurrentAnimation);
            if (Sprite.CurrentAnimation == "forwardthrow")
            {
                Sprite.CurrentAnimation = "forwardthrowattack";
            }
            else if (Sprite.CurrentAnimation == "backthrow")
            {

            }

            // Maybe we'll use this later. Hardcode this for now
            //
            if (Sprite.CurrentMoveAnimation.NextMoveOnHit != null)
            {
                sprite.CurrentAnimation = Sprite.CurrentMoveAnimation.NextMoveOnHit;
            }
        }

        public virtual void hitEnemyWithProjectile()
        {
            giveSpecialMeter(10);
            SoundManager.PlaySound(Sprite.CurrentAnimation);
        }

        public void giveSpecialMeter(int amount)
        {
            CurrentSpecial += amount;
            if (CurrentSpecial > MaxSpecial)
            {
                CurrentSpecial = MaxSpecial;
            }
        }

        // Projectile stuff?!
        //

        public virtual void AddProjectile(String name, ProjectileAnimation projectileAnimation)
        {

        }

        public virtual void ShootProjectile(Projectile projectileAnimation)
        {
            ProjectileManager.createProjectile(projectileAnimation);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (bVisible)
            {
                sprite.Draw(spriteBatch, 0, 0, Direction);
            }
        }

        public virtual void DrawGauges(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(HealthBar.HealthBarTexture, new Rectangle(HealthBar.HealthBarMargin,
            //            20, 425, 150), new Rectangle(0, 0, 425, 150), Color.White);

            //Draw the box around the health bar
            HealthBar.Draw(spriteBatch);
            SpecialBar.Draw(spriteBatch);
            //spriteBatch.Draw(HealthBar.HealthBarTexture, new Rectangle(HealthBar.HealthBarMargin,
            //            675, (int)(HealthBar.Width * ((double)CurrentSpecial / MaxSpecial)), 44), new Rectangle(0, 45, HealthBar.Width, 44), Color.Blue);

            //Draw the box around the health bar
            //spriteBatch.Draw(HealthBar.HealthBarTexture, new Rectangle(HealthBar.HealthBarMargin,
            //        675, HealthBar.Width, 44), new Rectangle(0, 0, HealthBar.Width, 44), Color.White);

        }

        public virtual void setUpGauges(ContentManager content, int healthBar)
        {
   
        }

        public virtual void resetRound()
        {
            Sprite.CurrentAnimation = "standing";
            CurrentHealth = MaxHealth;
            CurrentSpecial = 0;
            Position = startingPosition;
        }

        // Boring accessors without special implementations. Keeping them down here just cus of how many there are...
        //
        public SpriteAnimationManager Sprite
        {
            get { return sprite; }
        }

        public SpecialInputManager SpecialInputManager
        {
            get { return specialInputManager; }
        }
        public int CurrentHealth
        {
            get { return HealthBar.CurrentAmount; }
            set { HealthBar.CurrentAmount = value; }
        }

        public int MaxHealth
        {
            get { return HealthBar.MaxAmount; }
            set { HealthBar.MaxAmount = value; }
        }

        public Gauge HealthBar
        {
            get { return healthBar; }
            set { healthBar = value; }
        }

        public int CurrentSpecial
        {
            get { return SpecialBar.CurrentAmount; }
            set { SpecialBar.CurrentAmount = value; }
        }

        public int MaxSpecial
        {
            get { return SpecialBar.MaxAmount; }
            set { SpecialBar.MaxAmount = value; }
        }

        public Gauge SpecialBar
        {
            get { return specialBar; }
            set { specialBar = value; }
        }

        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public Direction Direction
        {
            get { return directionFacing; }
            set { directionFacing = value; }
        }

        public int X
        {
            get { return sprite.X; }
            set { sprite.X = value; }
        }

        ///
        /// The Y position of the sprite's upper left corner pixel.
        ///
        public int Y
        {
            get { return sprite.Y; }
            set { sprite.Y = value; }
        }

        public int CenterX
        {
            get { return sprite.CenterX; }
        }
        public int CenterY
        {
            get { return sprite.CenterY; }
        }

        public int HorizontalCollisionBuffer
        {
            get { return collisionBufferX; }
            set { collisionBufferX = value; }
        }

        public int VerticalCollisionBuffer
        {
            get { return collisionBufferY; }
            set { collisionBufferY = value; }
        }

        public bool IsVisible
        {
            get { return bVisible; }
            set { bVisible = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public bool IsActive
        {
            get { return active; }
            set { active = value; }
        }

        public bool IsMoving
        {
            get { return movingTowardsTarget; }
            set { movingTowardsTarget = value; }
        }

        public bool IsCollidable
        {
            get { return bCollidable; }
            set { bCollidable = value; }
        }

        public bool IsCrouching
        {
            get { return isCrouching; }
            set { isCrouching = value; }
        }

        public Vector2 CurrentVelocity
        {
            get { return currentVelocity; }
            set { currentVelocity = value; }
        }

        public Vector2 InitialJumpVelocity
        {
            get { return initialJumpSpeed; }
        }

        public float JumpHorizontalSpeed
        {
            get { return jumpHorizontalSpeed; }
        }

        public bool IsInInterruptableAnimation
        {
            get { return isInInterruptableAnimation; }
            set { isInInterruptableAnimation = value; }
        }

        public int MomentumCounter
        {
            get { return momentumCounter; }
            set { this.momentumCounter = value; }
        }

        public Boolean IsCastingProjectile { get; set; }
       
        public int MomentumXMovement
        {
            get { return momentumXMovement; }
            set { this.momentumXMovement = value; }
        }


        public bool IsPhysical {get;set;}
     
    }

}
