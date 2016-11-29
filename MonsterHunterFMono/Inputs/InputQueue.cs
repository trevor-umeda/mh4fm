using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace MonsterHunterFMono
{
    public class InputQueue<T> : IEnumerable<T>
    {
        //This will function like a queue. But we want to interate over it and it will be circular
        private int inputBufferSize = 12;
        private T[] inputQueue;
     
        private int currentPosition = 0;

        public InputQueue()
        {
            inputQueue = new T[inputBufferSize];
        }
        public int Size
        {
            get { return inputQueue.Count(); }
        }
        public void Enqueue(T state)
        {
            
            inputQueue[currentPosition] = state;
            currentPosition = (currentPosition + 1) % inputBufferSize;
        }

        public void Reset()
        {
            currentPosition = 0;
            inputQueue = new T[inputBufferSize];
        }

        // These make Stack<T> implement IEnumerable<T> allowing 
        // a stack to be used in a foreach statement. 
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < inputBufferSize; i++)
            {
                //int index = (currentPosition - i + inputBufferSize) % inputBufferSize;
                // Need t o
                //
                var index = ((currentPosition) + i ) % inputBufferSize;
                
                yield return inputQueue[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // These make Stack<T> implement IEnumerable<T> allowing 
        // a stack to be used in a foreach statement. 
        public IEnumerable<T> GetReverseEnumerator
        {
            get
            {
                 for (int i = 0; i < 10 ; i++)
                 {
                    //int index = (currentPosition - i + inputBufferSize) % inputBufferSize;
                    var index = ((currentPosition - 1) - i  + inputBufferSize) % inputBufferSize;
                    yield return inputQueue[index];
                 }
            } 
        }
    }
}
