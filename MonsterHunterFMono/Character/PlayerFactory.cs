using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace MonsterHunterFMono
{
    public class PlayerFactory
    {

        public Texture2D DummyTexture { get; set; }

        public Player createCharacter(String CharacterId, ContentManager content, int playerNumber,
            ComboManager comboManager, ThrowManager throwManager, SuperManager superManager, ProjectileManager projectileManager)
        {
            Player player;

            int xPosition;
            int healthBarMargin;
            Texture2D healthBar = content.Load<Texture2D>("healthBar1");
            Texture2D healthOuterBar = content.Load<Texture2D>("HealthBar_empty");
            Texture2D specialBar = content.Load<Texture2D>("specialbar2");
            Texture2D specialOuterBar = content.Load<Texture2D>("Special_4split_empty");
            Direction direction;
            if (playerNumber == 1)
            {
                xPosition = Config.Instance.Player1XPosition;
                healthBarMargin = ((Config.Instance.ScreenWidth / 2) - 600) / 2;
                direction = Direction.Right;
            }
            else
            {
                xPosition = Config.Instance.Player2XPosition;
                healthBarMargin = (((Config.Instance.ScreenWidth / 2) - 600) / 2) + (Config.Instance.ScreenWidth / 2);
                direction = Direction.Left;
            }
            Gauge HealthBar = new Gauge(healthBar, 20, healthBarMargin, playerNumber, 10, new Rectangle(0,0,1276,150));
            HealthBar.OuterBarTexture = healthOuterBar;
            if (CharacterId.Equals("LongSword"))
            {
                player = new LongSwordPlayer(playerNumber, xPosition, Config.Instance.PlayerYHeight, comboManager, throwManager, HealthBar);
            }
            else if (CharacterId.Equals("HuntingHorn"))
            {
                player = new HuntingHornPlayer(playerNumber, xPosition, Config.Instance.PlayerYHeight+50, comboManager, throwManager, HealthBar);
            }
            else
            {
                player = new LongSwordPlayer(playerNumber, xPosition, Config.Instance.PlayerYHeight, comboManager, throwManager, HealthBar);
            }
            player.SuperManager = superManager;
            player.ProjectileManager = projectileManager;
            player.Sprite.dummyTexture = DummyTexture;

            player.SpecialBar = new Gauge(specialBar, 675, healthBarMargin + 90, playerNumber, 15, new Rectangle(0, 0, 934, 68));
            player.SpecialBar.CurrentAmount = 100;
            player.SpecialBar.MaxAmount = 100;
            player.SpecialBar.OuterBarTexture = specialOuterBar;

            player.Direction = direction;
            player.setUpGauges(content, healthBarMargin+90);        
            
            loadCharacterDataConfigs(CharacterId, player, content);
            loadParticles(CharacterId, player, content);
            player.SetUpUniversalAttackMoves();
           
            return player;
        }

        private void loadParticles(string character, Player player, ContentManager content)
        {          
            string[] files = Directory.GetFiles("Config/" + character + "/Particles", "*.txt");
            Dictionary<String, Texture2D> spriteTextures = new Dictionary<String, Texture2D>();
            foreach (string fileName in files)
            {
                Dictionary<String, Object> moveInfo = parseMoveInfo(fileName);
                particleParse(content, character, player, moveInfo, spriteTextures);
            }
        }

        protected void loadCharacterDataConfigs(String character, Player player, ContentManager content)
        {
            string[] files = Directory.GetFiles("Config/" + character+ "/Moves", "*.txt");

            Dictionary<String, Texture2D> spriteTextures = new Dictionary<String, Texture2D>();
            Dictionary<String, MoveInput> moveInputList = new Dictionary<String, MoveInput>(); 
            foreach (string fileName in files)
            {
                Dictionary<String, Object> moveInfo = parseMoveInfo(fileName);
                moveParse(content, character, player, moveInfo, spriteTextures, moveInputList);
            }
            try
            {
                System.IO.Stream stream = TitleContainer.OpenStream("Config/"+ character + "/Gatling.txt");
                System.IO.StreamReader sreader = new System.IO.StreamReader(stream);
                // use StreamReader.ReadLine or other methods to read the file data
                int lineNumber = 0;
                List<String> moves = new List<String>(); ;
                while (sreader.Peek() >= 0)
                {
                    String movesInfo = sreader.ReadLine();
                    // If its the first line, use that to register the moves and determine priority
                    //
                    if (lineNumber == 0)
                    {
                        //Console.WriteLine(movesInfo);
                        moves = new List<String>(movesInfo.Split('|'));
                        for (int i = 0; i < moves.Count; i++)
                        {
                            String moveName = moves[i].Trim();
                            if(moveInputList.ContainsKey(moves[i].Trim()))
                            {
                                 List<String> moveInput = moveInputList[moves[i].Trim()].InputCommand;
                                // TODO ATM this only looks at special moves, and the universal moves are handled outside. which is a bit bad
                                //
                                if (moveInput.Count > 1)
                                {
                                    player.RegisterGroundMove(moveName, moveInputList[moves[i].Trim()].InputCommand);
                                }
                            }               
                        }
                    }
                    // Otherwise register the gatlings
                    //
                    else
                    {
                       // Console.WriteLine(movesInfo);
                        String[] sHb = movesInfo.Split('|');
                        List<MoveInput> moveInputs = new List<MoveInput>();
                        for (int i = 0; i < moves.Count; i++)
                        {
                            if (sHb[i].Trim().Equals("x") && moveInputList.ContainsKey(moves[i].Trim()))
                            {

                                moveInputs.Add(moveInputList[moves[i].Trim()]);
                            }
                            //Console.WriteLine("Use " + moves[i].Trim() + " is " + sHb[i].Trim());
                        }

                        player.SpecialInputManager.registerGatling(sHb[0].Trim(), moveInputs);
                    }
                    lineNumber++;
                }         
                stream.Close();
            }

            catch (System.IO.FileNotFoundException)
            {
                // this will be thrown by OpenStream if gamedata.txt
                // doesn't exist in the title storage location
            }
            player.Sprite.CurrentAnimation = "standing";
        }
        public static Dictionary<String, Object> parseMoveInfo(String fileName)
        {
            //Console.WriteLine(fileName);
            bool parsingList = false;
            String tempListKey = "";
            Dictionary<String, Object> moveInfo = new Dictionary<String, Object>();
            List<String> tempListValue = new List<String>(); ;
            System.IO.Stream stream = TitleContainer.OpenStream(fileName);
            System.IO.StreamReader sreader = new System.IO.StreamReader(stream);
            while (sreader.Peek() >= 0)
            {
                String spriteLine = sreader.ReadLine();

                if (!parsingList)
                {
                    String[] sHb = spriteLine.Split('=');

                    if (sHb[1].Contains("["))
                    {
                        parsingList = true;
                        tempListKey = sHb[0].Trim();
                        tempListValue = new List<String>();
                        tempListValue.Add(sHb[1].Split('[')[1].Trim());
                    }
                    else
                    {
                        moveInfo.Add(sHb[0].Trim(), sHb[1].Trim());
                    }
                }
                else
                {
                    if (spriteLine.Contains("]"))
                    {
                        parsingList = false;
                        tempListValue.Add(spriteLine.Split(']')[0].Trim());
                        moveInfo.Add(tempListKey, tempListValue);
                    }
                    else
                    {
                        tempListValue.Add(spriteLine.Trim());
                    }
                }
            }
            return moveInfo;
        }

        protected void particleParse(ContentManager content, String character, Player player, Dictionary<String, Object> moveInfo, Dictionary<String, Texture2D> spriteTextures)
        {
            String name = (String)moveInfo["name"];
            Console.WriteLine("Parsing particle Name: " + name);
            Texture2D texture = null;
            if(!spriteTextures.TryGetValue((String)moveInfo["sprite"], out texture)) 
            {
                texture = content.Load<Texture2D>(character + "/" + (String)moveInfo["sprite"]);
               // spriteTextures.Add((String)moveInfo["sprite"], texture);
            }

            ProjectileAnimation newMove;
            newMove = new ProjectileAnimation(
                texture,
                int.Parse((String)moveInfo["XImageStart"]),
                int.Parse((String)moveInfo["YImageStart"]),
                int.Parse((String)moveInfo["Width"]),
                int.Parse((String)moveInfo["Height"]),
                int.Parse((String)moveInfo["FrameCount"]),
                int.Parse((String)moveInfo["Columns"]),
                float.Parse((String)moveInfo["FrameLength"]),
                CharacterState.NONE, // Kinda ghetto to set character state as null
                int.Parse((String)moveInfo["TimerLength"]),
                Direction.Right); // As a default this is prob fine

            if (moveInfo.ContainsKey("PlayOnce"))
            {
               Boolean playOnce = Convert.ToBoolean(moveInfo["PlayOnce"]);
               newMove.PlayOnce = playOnce;
            }

            if (moveInfo.ContainsKey("FrameLengthInfo"))
            {
                String framelengthString = (String)moveInfo["FrameLengthInfo"];
                List<String> frameLengthList = new List<String>(framelengthString.Split(','));
                List<int> frameLengthIntList = frameLengthList.ConvertAll(s => Int32.Parse(s));
                int[] frameLengthInfo = frameLengthIntList.ToArray();

                newMove.SetFrameLengthInfo(frameLengthInfo);
            }
            if (moveInfo.ContainsKey("NextAnimation"))
            {
                newMove.NextAnimation = (String)moveInfo["NextAnimation"];
            }
            if (moveInfo.ContainsKey("Hitbox"))
            {
                List<String> hitInfo = (List<String>)moveInfo["Hitbox"];
                foreach (String hitBox in hitInfo)
                {
                    String[] hitBoxData = hitBox.Split(';');
                    // Console.WriteLine(int.Parse(hurtBoxData[0]));
                    newMove.AddHitboxInfo(int.Parse(hitBoxData[0]),
                        new Hitbox(hitBoxData[1], hitBoxData[2], hitBoxData[3], hitBoxData[4]));
                }
            }
             
            Hitzone hitZone;
            //int test = (int)moveInfo["Blockstun"];

            if (moveInfo.ContainsKey("XSpeed"))
            {
                newMove.XSpeed = int.Parse((String)moveInfo["XSpeed"]);
            }

            if (moveInfo.ContainsKey("YSpeed"))
            {
                newMove.YSpeed = int.Parse((String)moveInfo["YSpeed"]);
            }

            if(moveInfo.ContainsKey("Hitzone")&& moveInfo.ContainsKey("Hitstun") && moveInfo.ContainsKey("Blockstun")){
                Enum.TryParse((String)moveInfo["Hitzone"], true, out hitZone);
                HitInfo hitMoveInfo = new HitInfo(int.Parse((String)moveInfo["Hitstun"]), int.Parse((String)moveInfo["Blockstun"]), hitZone);
                hitMoveInfo.Damage = int.Parse((String)moveInfo["Damage"]);
                hitMoveInfo.IsHardKnockDown = Boolean.Parse((String)moveInfo["IsHardKnockDown"]);
                hitMoveInfo.AirUntechTime = int.Parse((String)moveInfo["AirUntechTime"]);
                hitMoveInfo.AirYVelocity = int.Parse((String)moveInfo["AirYVelocity"]);
                hitMoveInfo.AirXVelocity = int.Parse((String)moveInfo["AirXVelocity"]);
                newMove.NumOfHits = int.Parse((String)moveInfo["NumOfHits"]);
                if (moveInfo.ContainsKey("ForceAirborne"))
                {
                    hitMoveInfo.ForceAirborne = Boolean.Parse((String)moveInfo["ForceAirborne"]);
                }
                if (moveInfo.ContainsKey("Unblockable"))
                {
                    hitMoveInfo.Unblockable = Boolean.Parse((String)moveInfo["Unblockable"]);
                }
                newMove.HitInfo = hitMoveInfo;
            }
            player.AddProjectile(name, newMove);             
        }

        protected void moveParse(ContentManager content, String character, Player player, Dictionary<String, Object> moveInfo, Dictionary<String, Texture2D> spriteTextures, Dictionary<String, MoveInput> moveInputList)
        {

            String name = (String)moveInfo["name"];

            Texture2D texture = null;
            if(!spriteTextures.TryGetValue((String)moveInfo["sprite"], out texture)) 
            {
                texture = content.Load<Texture2D>(character + "/" +(String)moveInfo["sprite"]);
                spriteTextures.Add((String)moveInfo["sprite"], texture);
            }

            CharacterState moveState;
            Enum.TryParse((String)moveInfo["CharacterState"], true, out moveState);

            Move newMove;

            if (moveState == CharacterState.HIT || moveState == CharacterState.BLOCK)
            {
                newMove = new HitAnimation(
                    texture,
                    int.Parse((String)moveInfo["XImageStart"]),
                    int.Parse((String)moveInfo["YImageStart"]),
                    int.Parse((String)moveInfo["Width"]),
                    int.Parse((String)moveInfo["Height"]),
                    int.Parse((String)moveInfo["FrameCount"]),
                    int.Parse((String)moveInfo["Columns"]),
                    float.Parse((String)moveInfo["FrameLength"]),
                    moveState
                 );
            }
            else
            {
                newMove = new Move(
                    texture, 
                    int.Parse((String)moveInfo["XImageStart"]), 
                    int.Parse((String)moveInfo["YImageStart"]), 
                    int.Parse((String)moveInfo["Width"]),
                    int.Parse((String)moveInfo["Height"]),
                    int.Parse((String)moveInfo["FrameCount"]),
                    int.Parse((String)moveInfo["Columns"]),
                    float.Parse((String)moveInfo["FrameLength"]),
                    moveState
                );
            }

            if (moveInfo.ContainsKey("IsAttack"))
            {
                newMove.IsAttack = Boolean.Parse((String)moveInfo["IsAttack"]);
            }

            if (moveInfo.ContainsKey("BackupMove"))
            {
                newMove.BackupMove = (String)moveInfo["BackupMove"];
            }

            if (moveInfo.ContainsKey("LoopCount"))
            {
                newMove.LoopCount= int.Parse((String)moveInfo["LoopCount"]);
            }

            if (moveInfo.ContainsKey("FrameLengthInfo"))
            {
                String framelengthString = (String)moveInfo["FrameLengthInfo"];
                List<String> frameLengthList = new List<String>(framelengthString.Split(','));
                List<int> frameLengthIntList = frameLengthList.ConvertAll(s => Int32.Parse(s));
                int[] frameLengthInfo = frameLengthIntList.ToArray();

                newMove.SetFrameLengthInfo(frameLengthInfo);
            }

            if (moveInfo.ContainsKey("StartFrame"))
            {
                newMove.StartFrame = int.Parse((String)moveInfo["StartFrame"]);
            }


            if (moveInfo.ContainsKey("XMovement"))
            {
                String xMovementString = (String)moveInfo["XMovement"];
                List<String> xMovementList = new List<String>(xMovementString.Split(','));
                List<int> xMovementIntList = xMovementList.ConvertAll(s => Int32.Parse(s));
                int[] xMovementInfo = xMovementIntList.ToArray();

                newMove.SetXMovementInfo(xMovementInfo);
            }

            if (moveInfo.ContainsKey("Hitbox"))
            {
                List<String> hitInfo = (List<String>)moveInfo["Hitbox"];
                foreach (String hitBox in hitInfo)
                {
                    String[] hitBoxData = hitBox.Split(';');
                    // Console.WriteLine(int.Parse(hurtBoxData[0]));
                    if (hitBoxData.Count() > 1)
                    {
                        newMove.AddHitboxInfo(int.Parse(hitBoxData[0]),
                                              new Hitbox(hitBoxData[1], hitBoxData[2], hitBoxData[3], hitBoxData[4]));
                    }
                  
                }
            }

            
            if (moveInfo.ContainsKey("Hurtbox"))
            {
                List<String> hurtInfo = (List<String>)moveInfo["Hurtbox"];
                foreach (String hurtBox in hurtInfo)
                {
                    if (hurtBox.Length > 0)
                    {
                        String[] hurtBoxData = hurtBox.Split(';');
                        // Console.WriteLine(int.Parse(hurtBoxData[0]));
                        newMove.AddHurtboxInfo(int.Parse(hurtBoxData[0]),
                            new Hitbox(hurtBoxData[1], hurtBoxData[2], hurtBoxData[3], hurtBoxData[4]));
                    }
                    
                }
            }
             
            Hitzone hitZone;
            //int test = (int)moveInfo["Blockstun"];

            if(moveInfo.ContainsKey("Hitzone")&& moveInfo.ContainsKey("Hitstun") && moveInfo.ContainsKey("Blockstun")){
                Enum.TryParse((String)moveInfo["Hitzone"], true, out hitZone);
                HitInfo hitMoveInfo = new HitInfo(int.Parse((String)moveInfo["Hitstun"]), int.Parse((String)moveInfo["Blockstun"]), hitZone);
                hitMoveInfo.Damage = int.Parse((String)moveInfo["Damage"]);
                hitMoveInfo.IsHardKnockDown = Boolean.Parse((String)moveInfo["IsHardKnockDown"]);
                hitMoveInfo.AirUntechTime = int.Parse((String)moveInfo["AirUntechTime"]);
                hitMoveInfo.AirYVelocity = int.Parse((String)moveInfo["AirYVelocity"]);
                hitMoveInfo.AirXVelocity = int.Parse((String)moveInfo["AirXVelocity"]);
                if(moveInfo.ContainsKey("ResetHitInfo"))
                {
                    String resetHitInfoString = (String)moveInfo["ResetHitInfo"];
                    List<String> resetHitInfoList = new List<String>(resetHitInfoString.Split(','));
                    List<int> resetInfoIntList = resetHitInfoList.ConvertAll(s => Int32.Parse(s));
                    foreach (int resetSpot in resetInfoIntList)
                    {
                        newMove.AddResetInfo(resetSpot, true);
                    }

                }
                if (moveInfo.ContainsKey("ForceAirborne"))
                {
                    hitMoveInfo.ForceAirborne = Boolean.Parse((String)moveInfo["ForceAirborne"]);
                }
                if (moveInfo.ContainsKey("FreezeMove"))
                {
                    hitMoveInfo.FreezeOpponent = Boolean.Parse((String)moveInfo["FreezeMove"]);
                }
                if (moveInfo.ContainsKey("Unblockable"))
                {
                    hitMoveInfo.Unblockable = Boolean.Parse((String)moveInfo["Unblockable"]);
                }

                if (moveInfo.ContainsKey("HitType"))
                {
                    HitType hitType;
                    Enum.TryParse((String)moveInfo["HitType"], true, out hitType);
                    hitMoveInfo.HitType = hitType;
                }
                newMove.HitInfo = hitMoveInfo;
            }
            if (moveInfo.ContainsKey("NextAnimation"))
            {
                newMove.NextAnimation = (String)moveInfo["NextAnimation"];
            }
            if (moveInfo.ContainsKey("ProjectileCreationFrame"))
            {
                newMove.ProjectileCreationFrame = int.Parse((String)moveInfo["ProjectileCreationFrame"]);
            }

            if (moveInfo.ContainsKey("Input"))
            {
                String moveInput = (String)moveInfo["Input"];
                String[] inputs = moveInput.Split(';');
                List<String> inputList = new List<String>(inputs);
                moveInputList.Add(name, new MoveInput(name, inputList));
            }
            player.Sprite.AddAnimation(name, newMove);      
        }
    }
}
