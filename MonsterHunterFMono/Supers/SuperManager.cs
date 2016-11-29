using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterHunterFMono
{
    public interface SuperManager
    {

        void drawSuperEffects(SpriteBatch spriteBatch, Texture2D background, Rectangle mainFrame);
        Boolean isDrawingSuperFreeze();
        Boolean isInSuperFreeze();
        Boolean isInSuper();
        void performSuper(int player, Vector2 position);
        void endSuper(int player);
        int playerPerformingSuper();
        void processSuperFreeze(); 
    }
}
