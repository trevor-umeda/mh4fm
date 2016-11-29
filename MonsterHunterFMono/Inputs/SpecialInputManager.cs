using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace MonsterHunterFMono
{
    public class SpecialInputManager
    {
        private InputManager inputManager;
        private ControlSetting controlSetting;
       
        public SpecialInputManager()
        {
            inputManager = new InputManager();   
        }

        public ControlSetting ControlSetting
        {
            get { return controlSetting; }
            set 
            {
                this.controlSetting = value;
                this.inputManager.ControlSetting = value;
            }
        }

        public String checkMoves(CharacterState characterState, Direction direction, String currentMove, KeyboardState newKeyboardState)
        {
            Dictionary<String, Keys> controls = controlSetting.Controls;
            String returnMove = null;
            // First enqueue the current state into our input queue
            //
            inputManager.enqueueState(newKeyboardState, controlSetting.Controls);

            if (inputManager.DetermineJumpPress(newKeyboardState))
            {
               // Console.WriteLine("JUMP CANCEL");
                return "jumpcancel";
            }
            if(!inputManager.DetermineButtonPress(newKeyboardState))
            {
                
                if (characterState == CharacterState.DASHING && newKeyboardState.IsKeyDown(controls["right"]))
                {
                    if (direction != Direction.Left)
                    {
                        return "dash";
                    }
                }
                if (characterState == CharacterState.DASHING && newKeyboardState.IsKeyDown(controls["left"]))
                {
                    if (direction == Direction.Left)
                    {
                       return "dash";
                    }    
                }
            }
            if(characterState != CharacterState.AIRBORNE && characterState != CharacterState.AIRDASH)
            {
                returnMove = inputManager.checkGroundMoves(direction, currentMove, newKeyboardState);                
            }
            else
            {
                returnMove = inputManager.checkAirMoves(direction, currentMove, newKeyboardState);
                if (characterState == CharacterState.AIRDASH && returnMove == null)
                {
                    return "airdash";
                }
            }
            return returnMove; 
        }

        public void registerGatling(String name, List<MoveInput> inputs)
        {
            inputManager.registerGatling(name, inputs);
        }
        public void registerGroundMove(String name, List<String> input)
        {
            // input.Reverse();
            inputManager.registerGroundMove(name, input);
        }

        public void registerAirMove(String name, List<String> input)
        {
            inputManager.registerAirMove(name, input);
        }
    }
}
