using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterHunterFMono
{
    class BasicSuperManager : SuperManager
    {
        int superFreezeDelayTime { get; set; }
        int superFreezeTimer { get; set; }

        int timeAllowedToZoom;

        Vector2 cameraPositionMovement;

        Vector2 startingCamPosition;
        Vector2 playerZoomInPosition;

        int playerNumber;
        bool isInSuperState;
        Camera2d camera;

        public BasicSuperManager(Camera2d camera)
        {
            superFreezeDelayTime = 60;
            superFreezeTimer = 0;
            timeAllowedToZoom = 10;
            this.camera = camera;
        }

        public void drawSuperEffects(SpriteBatch spriteBatch, Texture2D background, Rectangle mainFrame)
        {
            if (isInSuperFreeze())
            {
                Color backgroundTint = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                //spriteBatch.Draw(background, mainFrame, backgroundTint);

            }
            else if (isInSuper())
            {
                Color backgroundTint = Color.Lerp(Color.White, Color.Black, 0.5f);
                //spriteBatch.Draw(background, mainFrame, backgroundTint);
            }
        }

        public void decrementTimer()
        {
            superFreezeTimer--;
        }
        public bool isDrawingSuperFreeze()
        {
            return (isInSuper() || isInSuperFreeze());

        }
        public bool isInSuperFreeze()
        { 
            return superFreezeTimer > 0;
        }

        public bool isInSuper()
        {
            return isInSuperState;
        }

        public int playerPerformingSuper()
        {
            return playerNumber;
        }

        public void endSuper(int player)
        {
            if (player == playerNumber && isInSuperState == true)
            {
                isInSuperState = false;
            }
            
            camera.Zoom = 1;
        }
        public void processSuperFreeze()
        {
            // TODO make these based on the move and not hardcoded values
            //
            if( superFreezeDelayTime - superFreezeTimer <= timeAllowedToZoom)
            {
                //camera.Move(cameraPositionMovement);
                //camera.ZoomIn(0.01f);
            }
            if (superFreezeTimer < timeAllowedToZoom)
            {
               
                //camera.Move(-cameraPositionMovement);
                //camera.Y = 360;
                //camera.ZoomIn(-0.01f);   
            }

            decrementTimer();

        }
        public void performSuper(int player, Vector2 position)
        {
            playerNumber = player;
            isInSuperState = true;
            playerZoomInPosition = position;
            playerZoomInPosition.Y -= 150;
            startingCamPosition = camera.Pos;

            cameraPositionMovement = (playerZoomInPosition - startingCamPosition) / timeAllowedToZoom;
            //camera.X = (int)playerZoomInPosition;
            superFreezeTimer = superFreezeDelayTime;
            Console.WriteLine("PERFORMING SUPER FOR PLAYER: " + player);
        }

    }
}
