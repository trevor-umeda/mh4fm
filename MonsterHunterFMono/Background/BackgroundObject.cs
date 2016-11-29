using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public struct BackgroundObject
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle mainFrame; 
        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, mainFrame, Color.White);
        }
    }
}
