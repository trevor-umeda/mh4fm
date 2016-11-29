using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterHunterFMono
{
    public class Camera2d
    {
        protected float zoom; // Camera Zoom
        public Matrix transform; // Matrix Transform
        public Vector2 position; // Camera Position
        protected float rotation; // Camera Rotation
        public Vector2 Origin { get; set; }

        protected float leftSideLimit;
        protected float rightSideLimit;
        protected float bottomSideLimit;
        protected int width;

        protected int gameWidth;
        protected int gameHeight;
        protected int screenWidth;
        protected int screenHeight;
 

        public Camera2d(int gameWidth, int screenWidth, int gameHeight, int screenHeight)
        {
            zoom = 1.0f;
            rotation = 0.0f;
            position = new Vector2(gameWidth / 2, 360.0f);
            Origin = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;            
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            computeLimits();
            width = screenWidth;
            
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return zoom; }
            set { zoom = value;
                if (zoom < 0.1f)
                {
                 zoom = 0.1f;
                }
                computeLimits();
            } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {      
            position += amount;
            adjustForHeightLimits();
        }

        public void ZoomIn(float amount)
        {
            zoom += amount;
            computeLimits();
            adjustForHeightLimits();
        }

        private void computeLimits()
        {
          
            leftSideLimit = (float)screenWidth * .5f;
            rightSideLimit = gameWidth - (float)screenWidth * .5f;
            bottomSideLimit = 360 / zoom;
        }

        private void adjustForHeightLimits()
        {
            if (position.Y > bottomSideLimit)
            {
                position.Y = bottomSideLimit;
            }
        }

        private void adjustForLimits()
        {
            if (position.X < leftSideLimit)
            {
                position.X = leftSideLimit;
            }
            else if (position.X > rightSideLimit)
            {
                position.X = rightSideLimit;
            }
            if (position.Y > bottomSideLimit)
            {
                position.Y = bottomSideLimit;
            }
        }

        // Get set position
        public Vector2 Pos
        {
            get { return position; }
            set { position = value; }
        }

        public int X
        {
            get { return (int)position.X; }
            set 
            { 
                position.X = value;
                adjustForLimits();
            }
        }

        public int Y
        {
            get { return (int)position.Y; }
            set
            {
                position.Y = value;
                adjustForHeightLimits();
                adjustForLimits();
            }
        }

        public int LeftEdge
        {
            get { return (int)position.X - (width / 2); }
        }

        public int RightEdge
        {
            get { return (int)position.X + (width / 2); }
        }

        public Matrix getTransformation(GraphicsDevice graphicsDevice)
        {
            transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 0));
            return transform;
        }
        public Matrix GetViewMatrix(Vector2 parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Pos * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
    }
}
