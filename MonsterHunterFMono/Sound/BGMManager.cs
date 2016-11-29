using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace MonsterHunterFMono
{
    class BGMManager
    {
        List<Song> songList;

        public BGMManager(ContentManager content)
        {
            songList = new List<Song>();
            //songList.Add(content.Load<Song>("bgm/20"));
            songList.Add(content.Load<Song>("bgm/Liara's Theme"));
        }
        
        public Song getRandomBGM()
        {
            Random rnd = new Random();
            return songList[rnd.Next(songList.Count)];
        }

    }
}
