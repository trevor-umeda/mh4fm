using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MonsterHunterFMono
{
    public class HuntingHornPlayer : Player
    {

        // TODO Make it so they can have multiple projectiles...
        //
        public Projectile ProjectileA { get; set; }
        Projectile ProjectileB { get; set; }
        Projectile ProjectileC { get; set; }
        Projectile ProjectileSpecial { get; set; }

        public Projectile ProjectileStrum { get; set; }

        public HuntingHornPlayer(int playerNumber, int xPosition, int yHeight, ComboManager comboManager, ThrowManager throwManager, Gauge healthBar) 
            : base ( playerNumber, xPosition, yHeight, comboManager, throwManager, healthBar)
        {
            CurrentHealth = 1000;
            MaxHealth = 1000;
           
            Sprite.BoundingBoxHeight = 288;
            Sprite.BoundingBoxWidth = 90;

            // TODO make these have to be set for every character.
            //
            ThrowRange = 200;
            BackAirDashVel = 8;
            AirDashVel = 13;
            BackStepVel = 12;
            DashVel = 11;
            BackWalkVel = 3;
            WalkVel = 4;
//            projectile = new ProjectileAnimation(texture, X, Y, Width, Height, Frames, columns, frameLength, characterState, timeLength, direction);

        }

        public override void checkValidityAndChangeMove(string moveName)
        {
            if (moveName == "supera")
            {
                if (CurrentSpecial < 50)
                {
                    changeMove("cattack");
                    moveName = "cattack";
                }
                else
                {
                    PerformSuperFreeze();
                    CurrentSpecial -= 50;
                }
               
            }

            base.changeMove(moveName);
        }

        public override void performGroundSpecialMove(KeyboardState ks, String moveName)
        {
            if (moveName == "slide")
            {
                Sprite.BoundingBoxHeight = 288;
                IsPhysical = false;
                if (Sprite.CurrentMoveAnimation.CurrentFrame > 4 && Sprite.CurrentMoveAnimation.CurrentFrame < 9)
                {
                    if (Direction == Direction.Left)
                    {
                        Sprite.MoveBy(-15, 0);
                    }
                    else
                    {
                        Sprite.MoveBy(15, 0);
                    }
                }
                if (Sprite.CurrentMoveAnimation.CurrentFrame >= 9)
                {
                    if (Direction == Direction.Left)
                    {
                        Sprite.MoveBy(-3, 0);
                    }
                    else
                    {
                        Sprite.MoveBy(3, 0);
                    }
                }

            }
            else if (moveName == "cattack")
            {
                // This seems kinda clumsy to perform every c attack. Maybe we should keep a reference the the projectile instead of cloning it.
                //
                ProjectileSummonStrum();
                
            }
            else if (moveName == "projectileA")
            {
                ProjectileSummonA();
            }
            else if (moveName == "projectileB")
            {
                ProjectileSummonB();
            }
            else if (moveName == "projectileC")
            {
                ProjectileSummonC();
            }
            else if (moveName == "superaslash")
            {
                ProjectileSummonSpecial();
            }
            else
            {
                IsPhysical = true;
                base.performGroundSpecialMove(ks, moveName);
            }
        }
        public void ProjectileSummonSpecial()
        {
            if (Sprite.CurrentMoveAnimation.CurrentFrame == Sprite.CurrentMoveAnimation.ProjectileCreationFrame)
            {
                if (IsCastingProjectile == false)
                {
                    Console.WriteLine("Creating a fireball!");

                    Projectile clonedProjectile = ProjectileSpecial;
                    clonedProjectile.CurrentAnimation = "specialEffect1";
                    clonedProjectile.CurrentProjectile.IsDone = false;

                    clonedProjectile.Direction = Direction;
                    int mo = Direction == Direction.Right ? 1 : -1;
                    clonedProjectile.Y = Sprite.Y - 150;
                    if (Direction == Direction.Right)
                    {
                        clonedProjectile.X = Sprite.X + Sprite.Width - clonedProjectile.CurrentProjectile.FrameWidth / 2 - 300;
                    }
                    else
                    {
                        clonedProjectile.X = Sprite.X - clonedProjectile.CurrentProjectile.FrameWidth / 2 + 300;
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
        public void ProjectileSummonA()
        {
            if (Sprite.CurrentMoveAnimation.CurrentFrame == Sprite.CurrentMoveAnimation.ProjectileCreationFrame)
            {
                if (IsCastingProjectile == false)
                {
                    Console.WriteLine("Creating a fireball!");

                    Projectile clonedProjectile = ProjectileA;
                    clonedProjectile.CurrentAnimation = "note1";
                    clonedProjectile.Direction = Direction;
                    int mo = Direction == Direction.Right ? 1 : -1;
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

        public void ProjectileSummonStrum()
        { 
            if (Sprite.CurrentMoveAnimation.CurrentFrame == Sprite.CurrentMoveAnimation.ProjectileCreationFrame)
            {
                if (IsCastingProjectile == false)
                {
                    Console.WriteLine("Creating a fireball!");

                    Projectile clonedProjectile = ProjectileStrum;
                    clonedProjectile.CurrentAnimation = "cstrum";
                    clonedProjectile.CurrentProjectile.IsDone = false;

                    clonedProjectile.Direction = Direction;
                    int mo = Direction == Direction.Right ? 1 : -1;
                    clonedProjectile.Y = Sprite.Y ;
                    if (Direction == Direction.Right)
                    {
                        clonedProjectile.X = Sprite.X + Sprite.Width - clonedProjectile.CurrentProjectile.FrameWidth / 2 + 100;
                    }
                    else
                    {
                        clonedProjectile.X = 250 - clonedProjectile.CurrentProjectile.FrameWidth + Sprite.X;
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

        public void ProjectileSummonB()
        {
            if (Sprite.CurrentMoveAnimation.CurrentFrame == Sprite.CurrentMoveAnimation.ProjectileCreationFrame)
            {
                if (IsCastingProjectile == false)
                {
                    Console.WriteLine("Creating a fireball!");

                    Projectile clonedProjectile = ProjectileB;
                    clonedProjectile.CurrentAnimation = "note2";

                    clonedProjectile.Direction = Direction;
                    int mo = Direction == Direction.Right ? 1 : -1;
                    clonedProjectile.Y = Sprite.Y - 150;
                    if (Direction == Direction.Right)
                    {
                        clonedProjectile.X = Sprite.X + Sprite.Width - clonedProjectile.CurrentProjectile.FrameWidth / 2 - 200;
                    }
                    else
                    {
                        clonedProjectile.X = 200 - clonedProjectile.CurrentProjectile.FrameWidth + Sprite.X;
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

        public void ProjectileSummonC()
        {
            if (Sprite.CurrentMoveAnimation.CurrentFrame == Sprite.CurrentMoveAnimation.ProjectileCreationFrame)
            {
                if (IsCastingProjectile == false)
                {
                    Console.WriteLine("Creating a fireball!");

                    Projectile clonedProjectile = ProjectileC;
                    clonedProjectile.CurrentAnimation = "note3";
                    clonedProjectile.Direction = Direction;
                    int mo = Direction == Direction.Right ? 1 : -1;
                    clonedProjectile.Y = Sprite.Y + 200;
                    if (Direction == Direction.Right)
                    {
                        clonedProjectile.X = Sprite.X + Sprite.Width - clonedProjectile.CurrentProjectile.FrameWidth / 2 + 70;
                    }
                    else
                    {
                        clonedProjectile.X = 140 - clonedProjectile.CurrentProjectile.FrameWidth + Sprite.X;
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
        public override void AddProjectile(String name, ProjectileAnimation projectileAnimation)
        {
            base.AddProjectile(name, projectileAnimation);
            if (name == "cstrum")
            {
                ProjectileStrum = new Projectile();
                ProjectileStrum.AddProjectile(name, projectileAnimation);
                ProjectileStrum.DummyTexture = Sprite.DummyTexture;
            }
            else if(name == "note1")
            {
                if (ProjectileA == null)
                {
                    ProjectileA = new Projectile();
                   
                    ProjectileC = new Projectile();
                }
                ProjectileA.AddProjectile(name, projectileAnimation);
                ProjectileA.DummyTexture = Sprite.DummyTexture;
            }
            else if (name == "note2")
            {
                if (ProjectileB == null)
                {
                    ProjectileB = new Projectile();
                }
                ProjectileB.AddProjectile(name, projectileAnimation);
                ProjectileB.DummyTexture = Sprite.DummyTexture;
            }
            else if (name == "note3")
            {
                ProjectileC.AddProjectile(name, projectileAnimation);
                ProjectileC.DummyTexture = Sprite.DummyTexture;
            }
            else
            {
                if (ProjectileSpecial == null)
                {
                    ProjectileSpecial = new Projectile();
                }
                ProjectileSpecial.AddProjectile(name, projectileAnimation);
                //ProjectileSpecial.SizeRefactor = 2f;
                ProjectileSpecial.DummyTexture = Sprite.DummyTexture; ;
            }
        }
    }
}
