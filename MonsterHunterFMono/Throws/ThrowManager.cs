using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
namespace MonsterHunterFMono
{
    public interface ThrowManager
    {
        CharacterState Player1State { get; set; }
        CharacterState Player2State { get; set; }

        String ForwardThrowInput { get; }
        String BackThrowInput { get; }

        String ForwardThrowWhiffMove { get; }
        String BackThrowWhiffMove { get; }

        void updateCharacterState(int playerNum, Player player);
        bool isValidThrow(int playerNum);       
    }
}
