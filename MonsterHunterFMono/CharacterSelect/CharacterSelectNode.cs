using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    class CharacterSelectNode
    {
        private String characterId;

        private Texture2D portrait;

        Rectangle drawRect;

        public CharacterSelectNode(String character, Vector2 pos, Texture2D texture)
        {
            characterId = character;
       
            portrait = texture;
            drawRect = new Rectangle((int)pos.X, (int)pos.Y, 137, 137);
        }

        public String CharacterId { get { return characterId; } }

        public Rectangle DrawRectangle { get { return drawRect; } }

        public void Draw(SpriteBatch sprite, Texture2D surroundingBox)
        {
            
            sprite.Draw(portrait, drawRect, Color.White);
            
       }

    }
}
