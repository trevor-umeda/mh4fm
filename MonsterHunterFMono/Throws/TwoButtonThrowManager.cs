using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterHunterFMono
{
    class TwoButtonThrowManager
    {
        private CharacterState player1State;
        private CharacterState player2State;

        public void updateCharacterState(int playerNum, CharacterState characterState)
        {

        }

        public bool isValidThrow()
        {
            return false;
        }

        public CharacterState Player1State
        {
            get { return player1State; }
            set { player1State = value; }
        }

        public CharacterState Player2State
        {
            get { return player2State; }
            set { player2State = value; }
        }
    }
}
