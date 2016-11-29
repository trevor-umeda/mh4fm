using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterHunterFMono
{
    public class Layer
    {
        private readonly Camera2d camera;
        public Layer(Camera2d camera)
        {
            this.camera = camera;
            Parallax = Vector2.One;
            Sprites = new List<BackgroundObject>();
        }

        public Vector2 Parallax { get; set; }
        public List<BackgroundObject> Sprites { get; private set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(Parallax));
            foreach (BackgroundObject sprite in Sprites)
                sprite.Draw(spriteBatch);
            spriteBatch.End();
        }

        
    }
}
