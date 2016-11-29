using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterHunterFMono
{
    class Config
    {
        private static Config instance;

        private int screenWidth;
        private int screenHeight;

        private int gameWidth;
        private int gameHeight;

        private int player1XPosition;
        private int player2XPosition;
        private int playerYHeight;

        private int groundYPos;

        public int ScreenWidth
        {
            get { return screenWidth; }
        }

        public int ScreenHeight
        {
            get { return screenHeight; }
        }

        public int GameWidth
        {
            get { return gameWidth; }
        }

        public int GameHeight
        {
            get { return gameHeight; }
        }

        public int Player1XPosition { get { return player1XPosition; } }
        public int Player2XPosition { get { return player2XPosition; } }
        public int PlayerYHeight { get { return playerYHeight; } }
        public int GroundYHeight { get { return groundYPos; } }

        private Config() 
        {
            gameWidth = 2000;
            gameHeight = 1200;

            screenWidth = 1300;
            screenHeight = 768;
            player1XPosition = 100;
            player2XPosition = 600;
            playerYHeight = 500;

            groundYPos = 725;
        }

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Config();
                }
                return instance;
            }
        }
    }
}
