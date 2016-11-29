using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.Threading;

namespace MonsterHunterFMono
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Player player1;
        public Player player2;
        Texture2D dummyTexture;
        Texture2D background;

        Texture2D titleScreen;
        Texture2D loadingScreen;
        Texture2D demoEndScreen;

        Camera2d cam;
        List<Layer> layers;

        Texture2D menuBg;
        Rectangle testHitbox;
        Rectangle mainFrame;
        HitInfo testHitInfo;

        String player1CharacterId;
        String player2CharacterId;

        ControlSetting player1Controls = new ControlSetting();
        ControlSetting player2Controls = new ControlSetting();

        private GameState gameState;
        private Thread backgroundThread;
        private bool isLoading = false;

        int comboNumber = 0;
        ComboManager comboManager;
        ThrowManager throwManager;
        SuperManager superManager;
        ProjectileManager projectileManager;

        RoundManager roundManager;
        ContentManager content;

        SpriteFont spriteFont;
        SpriteFont ingameFont;
        Texture2D victoryDots;

        int frameRate = 0;
        int frameCounter = 0;

        private SoundEffect effect;
        private SoundEffect hit2;
        private SoundEffect slash;
        private SoundEffect slash2;
        private SoundEffect strum;

        CharacterSelectList characterSelection;
        PauseMenu pauseMenu;
        MainMenu mainMenu;
        Texture2D pressStart;

        Texture2D player1NamePlate;
        Texture2D player2NamePlate;

        // Amount of time (in seconds) to display each frame
        private float frameLength = 0.016f;


        // Amount of time that has passed since we last animated
        private float frameTimer = 0.0f;

        TimeSpan elapsedTime = TimeSpan.Zero;
        SoundEffectInstance soundEffectInstance;

        private int hitstop = 0;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferHeight = Config.Instance.ScreenHeight;
            graphics.PreferredBackBufferWidth = Config.Instance.ScreenWidth;
            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            cam = new Camera2d(Config.Instance.GameWidth, Config.Instance.ScreenWidth, Config.Instance.GameHeight, Config.Instance.ScreenHeight);
            layers = new List<Layer>
            {
                 new Layer(cam) { Parallax = new Vector2(0.2f, 1.0f) },
                 new Layer(cam) { Parallax = new Vector2(0.3f, 1.0f) },
                 new Layer(cam) { Parallax = new Vector2(0.4f, 1.0f) },
                 new Layer(cam) { Parallax = new Vector2(0.8f, 1.0f) },
                 new Layer(cam) { Parallax = new Vector2(1.0f, 1.0f) },
                 new Layer(cam) { Parallax = new Vector2(1.0f, 1.0f) }
            };

            Texture2D test = Content.Load<Texture2D>("new_groundplane");
            layers[0].Sprites.Add(new BackgroundObject { texture = Content.Load<Texture2D>("background"), mainFrame = new Rectangle(-100, 0, 2200, Config.Instance.GameHeight) });
            layers[1].Sprites.Add(new BackgroundObject { texture = Content.Load<Texture2D>(@"main_castle"), mainFrame = new Rectangle(-100, 275, 2200, Config.Instance.GameHeight - 300) });
            layers[2].Sprites.Add(new BackgroundObject { texture = Content.Load<Texture2D>(@"midground"), mainFrame = new Rectangle(-100, 100, 2200, Config.Instance.GameHeight) });
            //layers[3].Sprites.Add(new BackgroundObject { texture = Content.Load<Texture2D>(@"foreground"), mainFrame = new Rectangle(-100, 0, 2200, Config.Instance.GameHeight) });
            layers[4].Sprites.Add(new BackgroundObject { texture = Content.Load<Texture2D>(@"new_groundplane"), mainFrame = new Rectangle(500, 0, 2200, Config.Instance.GameHeight) });
            //cam.Pos = new Vector2(Config.Instance.GameWidth/2, 360.0f);
            mainFrame = new Rectangle(-150, -450, 2200, Config.Instance.GameHeight);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("slant");
            ingameFont = Content.Load<SpriteFont>("arvil");
            comboManager = new ComboManager(ingameFont);

            BGMManager bgmManager = new BGMManager(Content);

            projectileManager = new ProjectileManager(Content);
            throwManager = new OneButtonThrowManager();
            superManager = new BasicSuperManager(cam);
            //background = Content.Load<Texture2D>("background");
            //background2 = Content.Load<Texture2D>("main_castle");

            menuBg = Content.Load<Texture2D>("bg2");

            victoryDots = Content.Load<Texture2D>("winDots");

            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);

            dummyTexture.SetData(new Color[] { Color.White });


            testHitbox = new Rectangle(100, 100, 100, 100);

            testHitInfo = new HitInfo(3, 20, Hitzone.HIGH);
            testHitInfo.IsHardKnockDown = true;
            testHitInfo.AirUntechTime = 8000;
            testHitInfo.AirXVelocity = 80;
            testHitInfo.AirYVelocity = 2;

            effect = Content.Load<SoundEffect>("hit_3");
            hit2 = Content.Load<SoundEffect>("hit_4");
            slash = Content.Load<SoundEffect>("stab_1");
            slash2 = Content.Load<SoundEffect>("stab_2");
            strum = Content.Load<SoundEffect>("strum_1");

            //MediaPlayer.Play(bgmManager.getRandomBGM());
            //MediaPlayer.Volume = 0.4f;

            player1Controls.setControl("down", Keys.Down);
            player1Controls.setControl("right", Keys.Right);
            player1Controls.setControl("left", Keys.Left);
            player1Controls.setControl("up", Keys.Up);
            player1Controls.setControl("a", Keys.A);
            player1Controls.setControl("b", Keys.S);
            player1Controls.setControl("c", Keys.D);
            player1Controls.setControl("d", Keys.Z);
            player1Controls.setControl("start", Keys.Enter);

            player2Controls.setControl("down", Keys.K);
            player2Controls.setControl("right", Keys.L);
            player2Controls.setControl("left", Keys.J);
            player2Controls.setControl("up", Keys.I);
            player2Controls.setControl("a", Keys.F);
            player2Controls.setControl("b", Keys.G);
            player2Controls.setControl("c", Keys.H);
            player2Controls.setControl("d", Keys.V);
            player2Controls.setControl("start", Keys.End);

            characterSelection = new CharacterSelectList(Content);
            gameState = GameState.MAINMENU;
            player1CharacterId = "LongSword";

            player2CharacterId = "HuntingHorn";

            pauseMenu = new PauseMenu(spriteFont);

            pressStart = Content.Load<Texture2D>("press_start");
            mainMenu = new MainMenu(spriteFont, pressStart);

            titleScreen = Content.Load<Texture2D>("Rising_Force_Title");
            loadingScreen = Content.Load<Texture2D>("VS_Screen");
            demoEndScreen = Content.Load<Texture2D>("thank_you");

            player1NamePlate = Content.Load<Texture2D>("Liara_game_text");
            player2NamePlate = Content.Load<Texture2D>("Aydin_game_text");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.MAINMENU)
            {
                mainMenu.moveMenuSelection(Keyboard.GetState(), player1Controls.Controls, player2Controls.Controls);
                if (mainMenu.menuItemSelected())
                {
                    if (mainMenu.SelectedMenu == "Start")
                    {
                        gameState = GameState.LOADING;
                        mainMenu.resetMenuSelection();
                    }
                }
            }
            if (gameState == GameState.PLAYERSELECT)
            {
                characterSelection.moveCharacterSelection(Keyboard.GetState(), player1Controls.Controls, player2Controls.Controls);
                if (characterSelection.selectionLocked())
                {
                    player1CharacterId = characterSelection.Player1CharacterId;
                    player2CharacterId = characterSelection.Player1CharacterId;
                    gameState = GameState.LOADING;
                }
            }
            if (gameState == GameState.LOADING && !isLoading) //isLoading bool is to prevent the LoadGame method from being called 60 times a seconds
            {
                backgroundThread = new Thread(LoadGame);
                isLoading = true;

                //start backgroundthread
                backgroundThread.Start();
            }
            else if (gameState == GameState.ROUNDEND)
            {
                projectileManager.clearAllProjectiles();
                player1.Update(gameTime, Keyboard.GetState(), false);
                player2.Update(gameTime, Keyboard.GetState(), false);
                roundManager.handleRoundEnd(projectileManager);
                if (roundManager.ResetRound == true)
                {
                    if (roundManager.Player1RoundWins == 2)
                    {
                        gameState = GameState.DEMOEND;
                        roundManager.keepFadedOut();
                        Thread.Sleep(1000);
                        Console.WriteLine("Player 1 wins at " + roundManager.Player1RoundWins + " ending round");
                    }
                    else if (roundManager.Player2RoundWins == 2)
                    {
                        gameState = GameState.DEMOEND;
                        roundManager.keepFadedOut();
                        Thread.Sleep(1000);
                        Console.WriteLine("Player 1 wins at " + roundManager.Player1RoundWins + " ending round");
                    }
                    else
                    {
                        gameState = GameState.PLAYING;
                    }

                }
                adjustCamera();
            }
            else if (gameState == GameState.DEMOEND)
            {
                projectileManager.clearAllProjectiles();
                player1.Update(gameTime, Keyboard.GetState(), false);
                player2.Update(gameTime, Keyboard.GetState(), false);
                roundManager.handleMatchEnd(projectileManager);
                if (roundManager.FadeAmount < .1f)
                {
                    Thread.Sleep(5000);
                    gameState = GameState.MAINMENU;
                }

            }
            else if (gameState == GameState.PAUSED)
            {
                pauseMenu.moveMenuSelection(Keyboard.GetState(), player1Controls.Controls, player2Controls.Controls);
                if (pauseMenu.menuItemSelected())
                {

                    if (pauseMenu.SelectedMenu == "Exit")
                    {
                        Exit();
                    }
                    else if (pauseMenu.SelectedMenu == "Controller Settings")
                    {
                        Keys[] test = Keyboard.GetState().GetPressedKeys();
                        if (test.Length > 0)
                        {
                            for (var i = 0; i < test.Length; i++)
                            {
                                Console.WriteLine(test[i]);
                            }
                        }
                        GamePadState padState1 = GamePad.GetState(PlayerIndex.One);
                        if (padState1.IsConnected)
                        {
                            if (padState1.Buttons.A == ButtonState.Pressed)
                            {
                                Console.WriteLine("Button A pressed on gamepad");
                            }
                        }

                    }
                    else if (pauseMenu.SelectedMenu == "Resume")
                    {
                        gameState = GameState.PLAYING;
                        pauseMenu.resetMenuSelection();
                    }
                }

            }
            else if (gameState == GameState.PLAYING)
            {
                frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //  if (frameTimer > frameLength)
                //{
                frameTimer = 0.0f;

                if (superManager.isInSuperFreeze())
                {
                    if (superManager.playerPerformingSuper() == 1)
                    {
                        player1.Update(gameTime, Keyboard.GetState(), false);
                    }
                    else
                    {
                        player2.Update(gameTime, Keyboard.GetState(), false);
                    }

                    superManager.processSuperFreeze();
                }
                else
                {
                    projectileManager.updateProjectileList(gameTime);
                    if (hitstop > 0)
                    {
                        player1.Update(gameTime, Keyboard.GetState(), true);

                        player2.Update(gameTime, Keyboard.GetState(), true);
                    }
                    else
                    {
                        player1.Update(gameTime, Keyboard.GetState(), false);

                        player2.Update(gameTime, Keyboard.GetState(), false);
                    }

                    if (hitstop == 0 && !superManager.isInSuperFreeze())
                    {

                        adjustPlayerPositioning();

                        keepPlayersInBound();

                        throwManager.updateCharacterState(1, player1);
                        throwManager.updateCharacterState(2, player2);

                        // Detect player collisions
                        //
                        if (player1.Sprite.Hitbox.Intersects(player2.Sprite.Hurtbox) && !player1.HasHitOpponent && player1.Sprite.CurrentMoveAnimation.HitInfo != null)
                        {
                            // TODO make this not hardcoded
                            //
                            Rectangle test = Rectangle.Intersect(player1.Sprite.Hitbox, player2.Sprite.Hurtbox);

                            hitstop = 7;
                            comboManager.player1LandedHit(player2.CharacterState);
                            Boolean hitEnemy = player2.hitByEnemy(Keyboard.GetState(), player1.Sprite.CurrentMoveAnimation.HitInfo, test);
                            player1.hitEnemy(hitEnemy);

                            System.Diagnostics.Debug.WriteLine("We have collision at " + player1.Sprite.CurrentMoveAnimation.CurrentFrame);
                            if (player2.CurrentHealth <= 0)
                            {
                                roundManager.roundEnd(1);
                                Console.WriteLine("Player 1 wins at " + roundManager.Player1RoundWins + " ending round");

                                gameState = GameState.ROUNDEND;
                            }
                        }
                        else if (player2.Sprite.Hitbox.Intersects(player1.Sprite.Hurtbox) && !player2.HasHitOpponent && player2.Sprite.CurrentMoveAnimation.HitInfo != null && player2.Sprite.Hitbox.Width > 0)
                        {
                            hitstop = 7;
                            Rectangle test = Rectangle.Intersect(player1.Sprite.Hurtbox, player2.Sprite.Hitbox);

                            comboManager.player2LandedHit(player1.CharacterState);
                            Boolean hitEnemy = player1.hitByEnemy(Keyboard.GetState(), player2.Sprite.CurrentMoveAnimation.HitInfo, test);
                            player2.hitEnemy(hitEnemy);
                            if (player1.CurrentHealth <= 0)
                            {
                                roundManager.roundEnd(2);

                                gameState = GameState.ROUNDEND;

                            }
                        }
                        else if (Keyboard.GetState().IsKeyDown(Keys.P))
                        {
                            Console.WriteLine("Test STuff");
                            //         cam.Zoom = 1.2f;
                            // player1.hitByEnemy(Keyboard.GetState(), testHitInfo);
                            //  player1.CurrentHealth -= 10;
                        }
                        else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        {
                            //Exit();
                            gameState = GameState.PAUSED;
                        }
                        else if (Keyboard.GetState().IsKeyDown(Keys.O))
                        {
                            cam.X -= 3;
                        }
                        projectileManager.checkHitOnPlayers(player1, player2, comboManager, roundManager, Keyboard.GetState(), gameState);
                        if (player1.CurrentHealth <= 0)
                        {


                            gameState = GameState.ROUNDEND;

                        }
                        elapsedTime += gameTime.ElapsedGameTime;

                        if (elapsedTime > TimeSpan.FromSeconds(1))
                        {
                            elapsedTime -= TimeSpan.FromSeconds(1);
                            frameRate = frameCounter;
                            frameCounter = 0;
                        }

                        // leftBorder.Width += 10;

                        adjustCamera();
                        comboManager.decrementComboTimer();

                        roundManager.decrementTimer(gameTime);
                        if (roundManager.isTimeOut())
                        {
                            roundManager.timeOut();
                        }

                        base.Update(gameTime);
                    }
                    else
                    {
                        hitstop--;
                        if (hitstop < 0)
                        {
                            hitstop = 0;
                        }
                    }
                }
            }


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            // Draw the safe area borders.
            Color translucentRed = Color.Red * 0.5f;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState == GameState.MAINMENU)
            {
                spriteBatch.Begin();
                //  Color black2 = Color.Black * .5f;
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, Config.Instance.ScreenWidth, Config.Instance.ScreenHeight), Color.White);
                mainMenu.Draw(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.PLAYERSELECT)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            null,
                            null,
                            cam.getTransformation(GraphicsDevice /*Send the variable that has your graphic device here*/));
                spriteBatch.Draw(menuBg, new Rectangle(0, 0, Config.Instance.ScreenWidth, Config.Instance.ScreenHeight), Color.White);
                characterSelection.Draw(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.LOADING)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(loadingScreen, new Rectangle(0, 0, Config.Instance.ScreenWidth, Config.Instance.ScreenHeight), Color.White);

                spriteBatch.End();
            }
            if (gameState == GameState.PLAYING || gameState == GameState.ROUNDEND || gameState == GameState.PAUSED)
            {
                //spriteBatch.Begin();
                foreach (Layer layer in layers)
                    layer.Draw(spriteBatch);
                spriteBatch.Begin(SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            null,
                            null,
                            cam.getTransformation(GraphicsDevice /*Send the variable that has your graphic device here*/));



                if (superManager.isDrawingSuperFreeze())
                {
                    superManager.drawSuperEffects(spriteBatch, background, mainFrame);
                    if (superManager.isInSuperFreeze())
                    {
                        // projectileManager.drawPortrait(spriteBatch);
                        Color black2 = Color.Black * .5f;
                        spriteBatch.Draw(dummyTexture, mainFrame, black2);
                    }

                }
                else
                {

                    //spriteBatch.Draw(background, mainFrame, Color.White);
                    //  spriteBatch.Draw(background2, mainFrame, Color.White);
                }


                //    spriteBatch.Draw(dummyTexture, test, translucentRed);
                //   spriteBatch.Draw(dummyTexture, testHitbox, translucentRed);



                player2.Draw(spriteBatch);

                player1.Draw(spriteBatch);

                projectileManager.drawAllProjectiles(spriteBatch);

                string health = string.Format("Health: {0}", player1.CurrentHealth);

                //spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
                //spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);
                //spriteBatch.DrawString(spriteFont, health, new Vector2(50, 50), Color.Black);

                spriteBatch.End();

                spriteBatch.Begin();

                spriteBatch.DrawString(ingameFont, roundManager.displayTime(), new Vector2(620, 20), Color.Black, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                spriteBatch.DrawString(ingameFont, roundManager.displayTime(), new Vector2(621, 21), Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                comboManager.displayComboMessage(spriteBatch);


                player1.DrawGauges(spriteBatch);
                player2.DrawGauges(spriteBatch);
                spriteBatch.Draw(player1NamePlate, new Rectangle(60, 75, 107, 70), Color.White);
                spriteBatch.Draw(player2NamePlate, new Rectangle(1100, 75, 107, 70), Color.White);

                if (roundManager.Player1RoundWins == 2)
                {
                    spriteBatch.Draw(victoryDots, (new Vector2(500, 60)),
                                                    new Rectangle(0, 100, 100, 100), Color.White,
                                                    0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                    spriteBatch.Draw(victoryDots, (new Vector2(540, 60)),
                                                    new Rectangle(0, 100, 100, 100), Color.White,
                                                    0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                }
                else if (roundManager.Player1RoundWins == 1)
                {
                    spriteBatch.Draw(victoryDots, (new Vector2(500, 60)),
                                                    new Rectangle(0, 100, 100, 100), Color.White,
                                                    0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                }
                else
                {
                    //spriteBatch.Draw(victoryDots, (new Vector2(500, 60)),
                    //                                new Rectangle(0, 0, 100, 100), Color.White,
                    //                                0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                    //spriteBatch.Draw(victoryDots, (new Vector2(540, 40)),
                    //                                new Rectangle(0, 100, 100, 100), Color.White,
                    //                                0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                }

                if (roundManager.Player2RoundWins == 2)
                {
                    spriteBatch.Draw(victoryDots, (new Vector2(705, 60)),
                                                    new Rectangle(0, 100, 100, 100), Color.White,
                                                    0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                    spriteBatch.Draw(victoryDots, (new Vector2(745, 60)),
                                                    new Rectangle(0, 100, 100, 100), Color.White,
                                                    0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                }
                else if (roundManager.Player2RoundWins == 1)
                {
                    spriteBatch.Draw(victoryDots, (new Vector2(705, 60)),
                                                    new Rectangle(0, 100, 100, 100), Color.White,
                                                    0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
                }
                else
                {
                }
                Color black = Color.Black * roundManager.FadeAmount;
                spriteBatch.Draw(dummyTexture, mainFrame, black);
                spriteBatch.End();

                // Have some pause drawing stuff
                //
                if (gameState == GameState.PAUSED)
                {
                    spriteBatch.Begin();
                    Color black2 = Color.Black * .5f;
                    spriteBatch.Draw(dummyTexture, mainFrame, black2);
                    pauseMenu.Draw(spriteBatch);
                    spriteBatch.End();
                }

            }
            if (gameState == GameState.DEMOEND)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(demoEndScreen, new Rectangle(0, 0, Config.Instance.ScreenWidth, Config.Instance.ScreenHeight), Color.White);
                Color black = Color.Black * roundManager.FadeAmount;
                spriteBatch.Draw(dummyTexture, mainFrame, black);
                //spriteBatch.Draw(dummyTexture, mainFrame, black2);
                //spriteBatch.DrawString(spriteFont, "Thanks For Playing!", new Vector2(500, 300), Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        protected void LoadGame()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            Console.WriteLine("Setting up Player1");
            PlayerFactory playerFactory = new PlayerFactory();
            playerFactory.DummyTexture = dummyTexture;

            player1 = playerFactory.createCharacter(player1CharacterId, this.Content, 1, comboManager, throwManager, superManager, projectileManager);
            Console.WriteLine("Player1 set up");
            // Set player 1 default controls
            //
            player1.ControlSetting = player1Controls;

            player2 = playerFactory.createCharacter(player2CharacterId, this.Content, 2, comboManager, throwManager, superManager, projectileManager);
            Console.WriteLine("Player2 set up");
            // Setting player 2 default controls
            //
            player2.ControlSetting = player2Controls;

            //player2.AddSound(strum, "cattack");

            //player1.AddSound(effect, "aattack");
            //player1.AddSound(hit2, "battack");
            //player1.AddSound(slash2, "cattack");
            //player1.AddSound(slash, "2cattack");
            //player1.AddSound(slash2, "rekkaB");
            //player1.AddSound(slash, "rekka");
            //player1.AddSound(slash, "rekkaC");
            //player1.AddSound(Content.Load<SoundEffect>("airbackdash_h"), "backstep");
            //player1.Sprite.AddResetInfo("aattack", 4);
            //  player1.Sprite.AddResetInfo("aattack", 6);

            roundManager = new RoundManager(player1, player2);

            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("TOOK " + elapsed_time + " ms to do this");
            if ((int)elapsed_time < 5000)
            {
                Console.WriteLine("Sleeping for a bit");
                Thread.Sleep(5000 - (int)elapsed_time);
            }
            gameState = GameState.PLAYING;
            isLoading = false;


        }

        protected void adjustCamera()
        {
            int newCamPosition = (player1.CenterX + player2.CenterX) / 2;

            cam.X = newCamPosition;
            int yPosition = (player1.CenterY + player2.CenterY) / 2;

            cam.Y = yPosition;
        }

        protected void adjustPlayerPositioning()
        {
            Vector2 player1Center = player1.Sprite.PositionCenter;
            Vector2 player2Center = player2.Sprite.PositionCenter;
            // Detect Player Collision. Ghetto atm and full of bugs but its a start as a base
            //

            int currentPlayer1XVel = Math.Abs(player1.Sprite.CurrentXVelocity);
            int currentPlayer2XVel = Math.Abs(player2.Sprite.CurrentXVelocity);

            // If the players are close enough, and they are heading in the opposite directions, then we can calculate collision movement
            //
            if ((Math.Abs(player1Center.X - player2Center.X) < 80) && player1.IsPhysical && player2.IsPhysical)
            {
                //&& (player1.Sprite.CurrentXVelocity * player2.Sprite.CurrentXVelocity < 0)
                int velocityDiff = currentPlayer1XVel - currentPlayer2XVel;
                // Case where the velocities are equal towards each other.
                //
                if (velocityDiff == 0)
                {
                    player1.Sprite.MoveBy(-player1.Sprite.CurrentXVelocity, 0);
                    player2.Sprite.MoveBy(-player2.Sprite.CurrentXVelocity, 0);
                }
                if (player1.Direction == Direction.Right)
                {
                    if ((currentPlayer1XVel > currentPlayer2XVel) && player1.Sprite.CurrentXVelocity > 0)
                    {
                        player2.Sprite.MoveBy(player1.Sprite.CurrentXVelocity, 0);
                        player1.Sprite.MoveBy(-player2.Sprite.CurrentXVelocity, 0);
                    }
                    else if (player2.Sprite.CurrentXVelocity < 0)
                    {
                        player1.Sprite.MoveBy(player2.Sprite.CurrentXVelocity, 0);
                        player2.Sprite.MoveBy(-player1.Sprite.CurrentXVelocity, 0);
                    }
                }
                else
                {
                    if ((currentPlayer1XVel > currentPlayer2XVel) && player1.Sprite.CurrentXVelocity < 0)
                    {

                        player2.Sprite.MoveBy(player1.Sprite.CurrentXVelocity, 0);
                        player1.Sprite.MoveBy(-player2.Sprite.CurrentXVelocity, 0);
                    }
                    else if (player2.Sprite.CurrentXVelocity > 0)
                    {

                        player1.Sprite.MoveBy(player2.Sprite.CurrentXVelocity, 0);
                        player2.Sprite.MoveBy(-player1.Sprite.CurrentXVelocity, 0);
                    }
                }
            }
            // Check to see which direction the player is facing
            //
            if (player1Center.X < player2Center.X)
            {
                if (player1.IsPhysical)
                {
                    player1.Direction = Direction.Right;
                }
                if (player2.IsPhysical)
                {
                    player2.Direction = Direction.Left;
                }

            }
            else
            {
                if (player1.IsPhysical)
                {
                    player1.Direction = Direction.Left;
                }

                if (player2.IsPhysical)
                {
                    player2.Direction = Direction.Right;
                }

            }
        }

        protected void keepPlayersInBound()
        {
            // Make sure the player doesn't go out of bound
            //
            if (player1.Sprite.BoundingBox.X < 0)
            {
                player1.Sprite.setXByBoundingBox(0);
            }
            if (player1.Sprite.BoundingBox.X + player1.Sprite.BoundingBox.Width > Config.Instance.GameWidth)
            {
                player1.Sprite.setXByBoundingBox(Config.Instance.GameWidth - player1.Sprite.BoundingBox.Width);
            }

            if (player1.Sprite.BoundingBox.X < cam.LeftEdge)
            {
                player1.Sprite.setXByBoundingBox(cam.LeftEdge);
            }

            if (player1.Sprite.BoundingBox.X + player1.Sprite.BoundingBox.Width > cam.RightEdge)
            {
                player1.Sprite.setXByBoundingBox(cam.RightEdge - player1.Sprite.BoundingBox.Width);

            }

            // Same out of bound checks for player 2
            //
            if (player2.Sprite.BoundingBox.X < 0)
            {
                player2.Sprite.setXByBoundingBox(0);
            }
            if (player2.Sprite.BoundingBox.X + player2.Sprite.BoundingBox.Width > Config.Instance.GameWidth)
            {
                player2.Sprite.setXByBoundingBox(Config.Instance.GameWidth - player2.Sprite.BoundingBox.Width);
            }
            if (player2.Sprite.BoundingBox.X < cam.LeftEdge)
            {
                player2.Sprite.setXByBoundingBox(cam.LeftEdge);
            }
            if (player2.Sprite.BoundingBox.X + player2.Sprite.BoundingBox.Width > cam.RightEdge)
            {
                player2.Sprite.setXByBoundingBox(cam.RightEdge - player2.Sprite.BoundingBox.Width);
            }
        }


    }
}
