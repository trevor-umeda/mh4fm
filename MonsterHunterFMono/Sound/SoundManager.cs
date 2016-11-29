using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
namespace MonsterHunterFMono
{
    class SoundManager
    {
        // Dictionary holding all of the FrameAnimation objects
        // associated with this sprite.
        Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();


        public SoundManager()
        {

        }

        public void AddSound(SoundEffect sound, String name)
        {
            sounds.Add(name, sound);
        }

        public void PlaySound(String name)
        {
           SoundEffect soundEffect = null;
            if(sounds.TryGetValue(name, out soundEffect)) 
            {
                soundEffect.Play();
            }       
        }
    }
}
