using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public class Hitbox
    {
        int xPos;
        int yPos;
        int width;
        int height;

        public int XPos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public int YPos
        {
            get { return yPos; }
            set { yPos = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public Rectangle getHitboxRectangle(Rectangle hitboxRect, Direction direction, Vector2 position, int frameWidth)
        {
            // IF the player is facing left
            //              
            if (direction == Direction.Left)
            {
                hitboxRect.Height = Height;
                hitboxRect.Width = Width;
                hitboxRect.X = XPos - Width / 2 + (int)position.X;
                hitboxRect.Y = YPos - Height / 2 + (int)position.Y;
            }
            else
            {
                hitboxRect.Height = Height;
                hitboxRect.Width = Width;

                hitboxRect.X = (int)position.X + frameWidth - Width / 2 - XPos;
                hitboxRect.Y = YPos - Height / 2 + (int)position.Y;
            }
            return hitboxRect;
        }

        public Hitbox(String X, String Y, String Width, String Height)
        {
            xPos = Convert.ToInt32(X);
            yPos = Convert.ToInt32(Y);
            width = Convert.ToInt32(Width);
            height = Convert.ToInt32(Height);
        }
    }
}
