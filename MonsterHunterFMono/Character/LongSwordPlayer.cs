using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonsterHunterFMono
{
    public class LongSwordPlayer : Player
    {
        bool DisplayShadow { get;  set;}
        int SwordLevel { get; set; }
        int SwordGauge
        {
            get { return SwordGaugeBar.CurrentAmount; }
            set { SwordGaugeBar.CurrentAmount = value; }
        }

        int MaxSwordGauge = 100;
        int rekkaLevel;

        // TODO Make it so they can have multiple projectiles...
        //
        public Projectile Projectile { get; set; }

        Dictionary<String, int> SwordGaugeGains { get; set; }
        Dictionary<String, int> MoveCosts { get; set; }

        Gauge SwordGaugeBar { get; set; }
        int swordGaugeBarMargin;
        int[] test;
        public LongSwordPlayer(int playerNumber, int xPosition, int yHeight, ComboManager comboManager, ThrowManager throwManager, Gauge healthBar) 
            : base ( playerNumber, xPosition, yHeight, comboManager, throwManager, healthBar)
        {
            CurrentHealth = 1000;

            MaxHealth = 1000;
            DisplayShadow = false;
            
            // D mechanic related moves
            //
            SwordLevel = 1;
           

            SwordGaugeGains = new Dictionary<String, int>();
            MoveCosts = new Dictionary<String, int>();

            SwordGaugeGains.Add("aattack", 5);
            SwordGaugeGains.Add("battack", 10);
            SwordGaugeGains.Add("cattack", 15);
            SwordGaugeGains.Add("2aattack", 5);
            SwordGaugeGains.Add("2battack", 10);
            SwordGaugeGains.Add("2cattack", 15);
            SwordGaugeGains.Add("jaattack", 5);
            SwordGaugeGains.Add("jbattack", 10);
            SwordGaugeGains.Add("jcattack", 15);
           // MoveCosts.Add("battack", 10);
            MoveCosts.Add("backfireball", 0);

            MoveCosts.Add("rekka", 0);
            MoveCosts.Add("rekkaB", 0);
            MoveCosts.Add("rekkaC", 0);

            Sprite.BoundingBoxHeight = 288;
            Sprite.BoundingBoxWidth = 90;

            // Essentially how many rekkas we've done
            //
            rekkaLevel = 1;

            // TODO make these have to be set for every character.
            //
            ThrowRange = 200;
            BackAirDashVel = 8;
            AirDashVel = 13;
            BackStepVel = 13;
            DashVel = 8;
            BackWalkVel = 3;
            WalkVel = 4;
//            projectile = new ProjectileAnimation(texture, X, Y, Width, Height, Frames, columns, frameLength, characterState, timeLength, direction);

        }

        public override void cleanUp()
        {
            base.cleanUp();
            if (!Sprite.CurrentMoveAnimation.IsAttack)
            {
                rekkaLevel = 1;
                SuperManager.endSuper(PlayerNumber);
                DisplayShadow = false;
                IsPhysical = true;
            }
        }

        public override void checkValidityAndChangeMove(string moveName)
        {
            // If we have a designated cost for our move. Make sure we can perform it. 
            //
            int moveCostValue;
            
            // If its a move that takes sword gauge, do some move specific checking and action
            //
            if (MoveCosts.TryGetValue(moveName, out moveCostValue))
            {
                if (SwordGauge - moveCostValue >= 0)
                {  
                    if (moveName.Contains("rekka"))
                    {
                        convertRekka(moveName);
                    }
                    else if (moveName == "backfireball")
                    {
                        if (!ProjectileManager.containsPlayerProjectile(PlayerNumber))
                        {
                            changeMove(moveName);
                        }
                        else
                        {
                            changeMove(determineBackupMove(moveName));
                        }
                    }
                    else
                    {
                        changeMove(moveName);
                    }
                   
                    SwordGauge = SwordGauge - moveCostValue;
                }
            
                else
                {
                    changeMove(determineBackupMove(moveName));
                    Console.WriteLine("Couldn't perform move cus not enough gauge");
                }
            }
            else if (CurrentSpecial < 50 && moveName == "supera")
            {
                changeMove("rekka");
            }
            else
            {
                changeMove(moveName);
            }
        }

        public override void changeMove(string moveName)
        {
            if (moveName == "supera")
            {
                PerformSuperFreeze();
                CurrentSpecial -= 50;
            }
            base.changeMove(moveName);
        }

        private void convertRekka(String moveName)
        {
            if (rekkaLevel  == 1 && moveName == "rekka" ||
                rekkaLevel == 2 && moveName == "rekkaB" || 
                rekkaLevel == 3 && moveName == "rekkaC")
            {
                changeMove(moveName);
                rekkaLevel++;
            }
            else if (rekkaLevel == 1 &&
                (moveName == "rekkaB" || moveName == "rekkaC"))
            {
                changeMove(determineBackupMove(moveName));
            }
        }

        public override void performGroundSpecialMove(KeyboardState ks, String moveName)
        {
            if (moveName == "fireball")
            {
                Fireball();
            }
            if (moveName == "supera")
            {
                DisplayShadow = true;
                if (Direction == Direction.Left)
                {
                    Sprite.MoveBy(-Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                }
                else
                {
                    Sprite.MoveBy(Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                }
            }
            if (moveName == "superaending")
            {
                if (Sprite.isLastFrameOfAnimation())
                {
                    // Tell the super manager that the special is over
                    //
                    SuperManager.endSuper(PlayerNumber);
                    Console.WriteLine("SPECIAL IS NOW OVER YAY");
                }
            }
            else if (moveName == "superafollowup")
            {
                if (Direction == Direction.Left)
                {
                    Sprite.MoveBy(-Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                }
                else
                {
                    Sprite.MoveBy(Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                }
            }
            else if (moveName == "superaslash")
            {
                if (Sprite.CurrentMoveAnimation.CurrentFrame >= 0 && Sprite.CurrentMoveAnimation.CurrentFrame < 4)
                {
                    IsPhysical = false;

                    if (Direction == Direction.Left)
                    {
                        Sprite.MoveBy(-Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                    }
                    else
                    {
                        Sprite.MoveBy(Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                    }
                    GivePlayerMomentum(7, 3, true);
                }
                else
                {
                    IsPhysical = true;
                }
                if (Sprite.isLastFrameOfAnimation() && Sprite.CurrentMoveAnimation.HasHitOpponent && !Sprite.CurrentMoveAnimation.Blocked)
                {
                    Sprite.CurrentAnimation = "superaending";
                }
            }
            else if (moveName == "aattack")
            {

            }
            else if (moveName == "cattack")
            {
                // This seems kinda clumsy to perform every c attack. Maybe we should keep a reference the the projectile instead of cloning it.
                //
                List<Projectile> projectiles = ProjectileManager.getPlayerProjectiles(PlayerNumber);
                for (int i = projectiles.Count - 1; i >= 0; i--)
                {
                    if (Sprite.Hitbox.Intersects(projectiles[i].Hitbox))
                    {
                        projectiles[i].CurrentAnimation = "projectile";
                        
                    }
                }
            }
            else if (moveName == "rekka")
            {
                if (Direction == Direction.Left)
                {
                    Sprite.MoveBy(-5, 0);
                }
                else
                {
                    Sprite.MoveBy(5, 0);
                }
            }
            else if (moveName == "rekkaB")
            {
                if (Sprite.CurrentMoveAnimation.CurrentFrame > 3 && Sprite.CurrentMoveAnimation.CurrentFrame < 11)
                {
                    if (Direction == Direction.Left)
                    {
                        Sprite.MoveBy(-20, 0);
                    }
                    else
                    {
                        Sprite.MoveBy(20, 0);
                    }
                }
            }
            else if (moveName == "rekkaC")
            {
                if (Sprite.CurrentMoveAnimation.CurrentFrame > 4 && Sprite.CurrentMoveAnimation.CurrentFrame < 8)
                {
                    IsPhysical = false;
                    if (Direction == Direction.Left)
                    {
                        Sprite.MoveBy(-Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                    }
                    else
                    {
                        Sprite.MoveBy(Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                    }
                    GivePlayerMomentum(7, 3, true);
                }
                else
                {
                    IsPhysical = true;
                }
            }

            
            else if (moveName == "superaending")
            {

            }
            else if (moveName == "specialcommand")
            {
                if (Sprite.CurrentMoveAnimation.CurrentFrame > 4 && Sprite.CurrentMoveAnimation.CurrentFrame < 8)
                {
                    IsPhysical = false;
                    if (Direction == Direction.Left)
                    {
                        Sprite.MoveBy(Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                    }
                    else
                    {
                        Sprite.MoveBy(-Sprite.CurrentMoveAnimation.CurrentXMovementInfo, 0);
                    }
                    GivePlayerMomentum(7, 3, true);
                }
                else
                {
                    IsPhysical = true;
                }
            }
            else if (moveName == "backfireball")
            {
                BackFireball();
            }
            else
            {
                base.performGroundSpecialMove(ks, moveName);
            }
        }

        public void Fireball()
        {
            DisplayShadow = true;
            Dash();          
        }

        public void BackFireball()
        {
            
            if (Sprite.CurrentMoveAnimation.CurrentFrame == Sprite.CurrentMoveAnimation.ProjectileCreationFrame)
            {
                if(IsCastingProjectile == false)
                {
                    Console.WriteLine("Creating a fireball!");

                    Projectile clonedProjectile = Projectile;
                    clonedProjectile.CurrentAnimation = "staticprojectile"; 
                    clonedProjectile.Direction = Direction;
                    
                    clonedProjectile.Y = Sprite.Y + 50;
                    if (Direction == Direction.Right)
                    {
                        clonedProjectile.X = Sprite.X + Sprite.Width - clonedProjectile.CurrentProjectile.FrameWidth / 2 - 70;
                    }
                    else
                    {
                        clonedProjectile.X = 70 - clonedProjectile.CurrentProjectile.FrameWidth + Sprite.X;
                    }
                  
                    clonedProjectile.PlayerNumber = PlayerNumber;
                    ProjectileManager.createProjectile(clonedProjectile);

                    Console.WriteLine("DOING BACK FIREBALL");
                    IsCastingProjectile = true;
                }              
            }
            else
            {
                IsCastingProjectile = false;
            }
        }

        public override void Backstep()
        {
            base.Backstep();
        }

        public override void Dash()
        {
            base.Dash();

            GivePlayerMomentum(7, 3, true);      
        }
        public override void BackWalk()
        {
            base.BackWalk();
            
        }

        public override void ForwardWalk()
        {
            base.ForwardWalk();
            
        }

        public override void hitEnemy(Boolean hitEnemy)
        {
            int swordGaugeGainValue;
            if (SwordGaugeGains.TryGetValue(Sprite.CurrentAnimation, out swordGaugeGainValue))
            {
                SwordGauge += swordGaugeGainValue;
                if (SwordGauge > 100)
                {
                    SwordGauge = 100;
                }
            }
            base.hitEnemy(hitEnemy);
        }

        public override void AddProjectile(String name, ProjectileAnimation projectileAnimation)
        {
            base.AddProjectile(name, projectileAnimation);
            if (Projectile == null)
            {
                Projectile = new Projectile();
            }
            Projectile.AddProjectile(name, projectileAnimation);
            Projectile.DummyTexture = Sprite.DummyTexture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, 0, 0, Direction);
            if (DisplayShadow)
            {
                Sprite.shadowDraw(spriteBatch, -9, 0, Direction);
            }           
        }

        public override void DrawGauges(SpriteBatch spriteBatch)
        {
            base.DrawGauges(spriteBatch);
            //Draw the special filled
            //
            //spriteBatch.Draw(SwordGaugeBar, new Rectangle(swordGaugeBarMargin,
            //            630, (int)(SwordGaugeBar.Width * ((double)SwordGauge / MaxSwordGauge)), 25), new Rectangle(0, 45, SwordGaugeBar.Width, 44), Color.Green);
            //SwordGaugeBar.Draw(spriteBatch);
            //Draw the box around the Special bar
            //spriteBatch.Draw(SwordGaugeBar, new Rectangle(swordGaugeBarMargin,
            //        630, SwordGaugeBar.Width, 25), new Rectangle(0, 0, SwordGaugeBar.Width, 44), Color.White);
        }

        public override void setUpGauges(ContentManager content, int healthBarMargin)
        {
            base.setUpGauges(content, healthBarMargin);
            //Texture2D SwordGaugeBarTexture = content.Load<Texture2D>("swordlbar2");
            //Texture2D SwordGaugeBarOuterTexture = content.Load<Texture2D>("Liara_Special_empty");
            SwordGaugeBar = new Gauge(null, 630, healthBarMargin, PlayerNumber, 15, new Rectangle(0, 0, 948, 26));
            SwordGaugeBar.MaxAmount = 100;
            SwordGaugeBar.CurrentAmount = 0;
        
        }

        public override void resetRound()
        {
            base.resetRound();
            SwordGauge = 0;
        }

    }
}
