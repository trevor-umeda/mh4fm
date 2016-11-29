using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterHunterFMono
{
    public class InputMoveBuffer
    {
        readonly public int bufferTime = 5; 
        public int bufferTimer = 0;
        public String bufferedMove;

        public void setBufferedMove(String bufferedMove)
        {
            this.bufferedMove = bufferedMove;
            bufferTimer = bufferTime;
        }

        public void unbufferCurrentMove()
        {
            this.bufferedMove = null;
            bufferTimer = 0;
        }

        public String getBufferedMove()
        {
            return bufferedMove;
        }

        public void decrementBufferTimer()
        {
            if (bufferTimer > 0)
            {
                bufferTimer--;
                if (bufferTimer <= 0)
                {
                    bufferedMove = null;
                }
            }

        }
    }
}
