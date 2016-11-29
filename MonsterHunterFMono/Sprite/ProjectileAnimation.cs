using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    public class ProjectileAnimation : Move
    { 
   
        public int NumOfHits { get; set; }
        // Currently can only shoot projectiles linearly... Is that a problem?
        //
        public int XSpeed { get; set; }
        public int YSpeed { get; set; }

        public int TimerLength { get; set; }

        public Boolean PlayOnce { get; set; }
         public ProjectileAnimation(Texture2D texture, int X, int Y, int Width, int Height, int Frames, int columns, float frameLength, CharacterState characterState, int timeLength, Direction direction)
            : base(texture, X, Y, Width, Height, Frames, columns, frameLength, characterState)
        {
            TimerLength = timeLength;
            PlayOnce = false;
        }

    }
}
