using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace MonsterHunterFMono
{
    public class ProjectileManager
    {
        List<Projectile> player1Projectiles;
        List<Projectile> player2Projectiles;
        List<ParticleAnimation> particles;
        ParticleAnimation punchEffect;
        ParticleAnimation slashEffect;
        ParticleAnimation impactSlashEffect;
        ParticleAnimation spikySlashEffect;
        ParticleAnimation spikyImpactEffect;
        ParticleAnimation blockEffect;

        ParticleAnimation jumpEffect;

        ParticleAnimation dashEffect;

        ParticleAnimation portrait;
        public ProjectileManager(ContentManager content)
        {
            player1Projectiles = new List<Projectile>();
            player2Projectiles = new List<Projectile>();
            particles = new List<ParticleAnimation>();
            // Load some universal things
            //
   
            Dictionary<String, Object> moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/impact.txt");
            
            Texture2D punchTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            punchEffect = new ParticleAnimation(
                punchTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/spikySlash.txt");

            Texture2D spikySlashTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            spikySlashEffect = new ParticleAnimation(
                spikySlashTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/spikyImpact.txt");

            Texture2D spikyImpactTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            spikyImpactEffect = new ParticleAnimation(
                spikyImpactTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/impactSlash.txt");

            Texture2D impactSlashTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            impactSlashEffect = new ParticleAnimation(
                impactSlashTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/slash.txt");

            Texture2D slashTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            slashEffect = new ParticleAnimation(
                slashTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/block.txt");

            Texture2D blockTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            blockEffect = new ParticleAnimation(
                blockTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/portraits/LongSword.txt");

            Texture2D portraitTexture = content.Load<Texture2D>("LongSword/" + (String)moveInfo["sprite"]);
            portrait = new ParticleAnimation(
                portraitTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/jumpDust.txt");

            Texture2D jumpTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            jumpEffect = new ParticleAnimation(
                jumpTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine

            moveInfo = PlayerFactory.parseMoveInfo("Config/Base/effect/dashDust.txt");

            Texture2D dashTexture = content.Load<Texture2D>((String)moveInfo["sprite"]);
            dashEffect = new ParticleAnimation(
                dashTexture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"])
               ); // As a default this is prob fine
        }

        public List<Projectile> getPlayerProjectiles(int playerNumber)
        {
            if (playerNumber == 1)
            {
                return player1Projectiles;
            }
            else
            {
                return player2Projectiles;
            }
        }

        public void clearAllProjectiles()
        {
            player1Projectiles = new List<Projectile>();
            player2Projectiles = new List<Projectile>();
            particles = new List<ParticleAnimation>();
         
        }

        public void createJumpParticle(float xPos, float yPos, Direction direction)
        {
            if (direction == Direction.Right)
            {
                particles.Add(jumpEffect.NewInstance((int)xPos - jumpEffect.FrameWidth, (int)yPos - jumpEffect.FrameHeight, false, direction));
            }
            else
            {
                particles.Add(jumpEffect.NewInstance((int)xPos, (int)yPos - jumpEffect.FrameHeight, false, direction));
            }
        }

        public void createDashParticle(float xPos, float yPos, Direction direction)
        {
            particles.Add(dashEffect.NewInstance((int)xPos, (int)yPos - dashEffect.FrameHeight, false, direction));
        }

        public void createHitparticle(Rectangle hitSection, HitType hitType)
        {
            Vector2 center = new Vector2(hitSection.X + (hitSection.Width/2), hitSection.Y + (hitSection.Height/2));

            if (hitType == HitType.IMPACT)
            {
                int xPos = ((int)center.X - punchEffect.FrameHeight / 2);
                int yPos = (int)center.Y - punchEffect.FrameWidth / 2;
                particles.Add(punchEffect.NewInstance(xPos, yPos, true));
            }
            else if (hitType == HitType.BLOCK)
            {
                int xPos = ((int)center.X - blockEffect.FrameHeight / 2);
                int yPos = (int)center.Y - blockEffect.FrameWidth / 2;
                particles.Add(blockEffect.NewInstance(xPos, yPos, true));
            }
            else if (hitType == HitType.HORN_HIT)
            {
                int xPos = ((int)center.X - spikySlashEffect.FrameHeight / 2);
                int yPos = (int)center.Y - spikySlashEffect.FrameWidth / 2;
                particles.Add(spikySlashEffect.NewInstance(xPos, yPos, true));
            }
            else if (hitType == HitType.HORN_IMPACT)
            {
                int xPos = ((int)center.X - spikyImpactEffect.FrameWidth);
                int yPos = (int)center.Y - spikyImpactEffect.FrameHeight / 2;
                particles.Add(spikyImpactEffect.NewInstance(xPos, yPos, false));
            }
            else if (hitType == HitType.SWORD_STRONG)
            {
                int xPos = ((int)center.X - impactSlashEffect.FrameHeight / 2);
                int yPos = (int)center.Y - impactSlashEffect.FrameWidth / 2;
                particles.Add(impactSlashEffect.NewInstance(xPos, yPos, true));
            }
            else
            {
                int xPos = ((int)center.X - slashEffect.FrameHeight / 2);
                int yPos = (int)center.Y - slashEffect.FrameWidth / 2;
                particles.Add(slashEffect.NewInstance(xPos, yPos, true));
            }

        }

        public void createProjectile(Projectile projectileAnimation)
        {
            if (projectileAnimation.PlayerNumber == 1)
            {
                player1Projectiles.Add(projectileAnimation);
            }
            else
            {
                player2Projectiles.Add(projectileAnimation);
            }
        }

        public void checkHitOnPlayers(Player player1, Player player2, ComboManager comboManager, RoundManager roundManager, KeyboardState Keyboard, GameState gameState)
        {
            for (int i = player1Projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = player1Projectiles[i];
                if (projectile.Hitbox.Intersects(player2.Sprite.Hurtbox) )
                {
                   
                    if (projectile.NumOfHits > 0)
                    {
                        Rectangle collisionZone = Rectangle.Intersect(projectile.Hitbox, player2.Sprite.Hurtbox);

                        comboManager.player1LandedHit(player2.CharacterState);
                        Boolean hitEnemy = player2.hitByEnemy(Keyboard, projectile.CurrentProjectile.HitInfo, collisionZone);
                        player1.hitEnemy(hitEnemy);
                        projectile.hitEnemy(); 
                    }
                    else
                    {
                        if (!projectile.PlayOnce)
                        {
                            player1Projectiles.RemoveAt(i);
                        }
                        
                    }
                    System.Diagnostics.Debug.WriteLine("We have projectile  1 collision at " + projectile.CurrentProjectile.CurrentFrame);
                    if (player2.CurrentHealth <= 0)
                    {
                        roundManager.roundEnd(1);
                        gameState = GameState.ROUNDEND;
                    }
                }
            }
                
            for( int j = player2Projectiles.Count - 1; j >=0; j--)
            {
                Projectile projectile = player2Projectiles[j];
                if (projectile.Hitbox.Intersects(player1.Sprite.Hurtbox))
                {
                    if (projectile.NumOfHits > 0)
                    {
                        Rectangle collisionZone = Rectangle.Intersect(projectile.Hitbox, player1.Sprite.Hurtbox);
                        comboManager.player2LandedHit(player1.CharacterState);
                        Boolean hitEnemy = player1.hitByEnemy(Keyboard, projectile.CurrentProjectile.HitInfo, collisionZone);
                        player2.hitEnemy(hitEnemy);
                        projectile.hitEnemy();
                    }
                    else
                    {
                        if (!projectile.PlayOnce)
                        {
                            player2Projectiles.RemoveAt(j);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("We have projectile collision at " + projectile.CurrentProjectile.CurrentFrame);
                    if (player1.CurrentHealth <= 0)
                    {
                        roundManager.roundEnd(2);
                        gameState = GameState.ROUNDEND;
                    }
                }         
            }
            
        }

        public Boolean containsPlayerProjectile(int playerNumber)
        {
            if (playerNumber == 1)
            {
                return player1Projectiles.Count > 0;
            }
            else
            {
                return player2Projectiles.Count > 0;
            }
        }

        public void updateProjectileList(GameTime gameTime)
        {
            for (int i = player1Projectiles.Count - 1; i >= 0; i--)
            {
                player1Projectiles[i].Update(gameTime);

                if (player1Projectiles[i].Finished)
                {
                    player1Projectiles.RemoveAt(i);
                }             
            }

            for (int i = player2Projectiles.Count - 1; i >= 0; i--)
            {
                player2Projectiles[i].Update(gameTime);

                if (player2Projectiles[i].Finished)
                {
                    player2Projectiles.RemoveAt(i);
                }
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);

                if (particles[i].Finished)
                {
                    particles.RemoveAt(i);
                }
            }
        }
        public void drawPortrait(SpriteBatch spriteBatch)
        {
            portrait.Draw(spriteBatch);
        }
        public void drawAllProjectiles(SpriteBatch spriteBatch)
        {
            for (int i = player1Projectiles.Count - 1; i >= 0; i--)
            {
                player1Projectiles[i].Draw(spriteBatch);
            }
            for (int i = player2Projectiles.Count - 1; i >= 0; i--)
            {
                player2Projectiles[i].Draw(spriteBatch);
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Draw(spriteBatch);
            }
        }
    }
}
