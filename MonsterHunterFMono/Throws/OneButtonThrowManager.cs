using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace MonsterHunterFMono
{
    public class OneButtonThrowManager : ThrowManager
    {

        private String FORWARD_THROW_INPUT = "6C";
        private String BACK_THROW_INPUT = "4C";

        private String FORWARD_THROW_WHIFF_MOVE = "cattack";
        private String BACK_THROW_WHIFF_MOVE = "cattack";

        private CharacterState player1State;
        private CharacterState player2State;

        private Vector2 player1Position;
        private Vector2 player2Position;

        // TODO see if this is different and not a constant
        //
        private int player1ThrowRange;
        private int player2ThrowRange;

        public void updateCharacterState(int playerNum, Player player)
        {
            if (playerNum == 1)
            {
                player1State = player.CharacterState;
                player1Position = player.Position;
                // Super hacky way to lazy initialize throw range without modifying contracts
                // TODO maybe make this less hacky?
                //
                if (player1ThrowRange == null || player1ThrowRange == 0)
                {
                    player1ThrowRange = player.ThrowRange;
                }
            }
            else
            {
                player2State = player.CharacterState;
                player2Position = player.Position;
                if (player2ThrowRange == null || player2ThrowRange == 0)
                {
                    player2ThrowRange = player.ThrowRange;
                }
            }
        }

        public String ForwardThrowInput
        {
            get
            {
                return FORWARD_THROW_INPUT;
            }
        }

        public String BackThrowInput
        {
            get
            {
                return BACK_THROW_INPUT;
            }
            
        }

        public String ForwardThrowWhiffMove
        {
            get
            {
                return FORWARD_THROW_WHIFF_MOVE;
            }            
        }

        public String BackThrowWhiffMove
        {
            get
            {
                return BACK_THROW_WHIFF_MOVE;
            }
        }

        public bool isValidThrow(int playerNum)
        {
            // You have to check the opponent (the other players) state
            //
            if (playerNum == 1)
            {
                if (Math.Abs(player2Position.X - player1Position.X) < player1ThrowRange)
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool checkPlayerThrowState()
        {
            return true;
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
