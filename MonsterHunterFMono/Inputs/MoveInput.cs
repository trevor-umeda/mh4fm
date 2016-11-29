using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace MonsterHunterFMono
{
    // Has the input of a special move
    //
    public class MoveInput
    {
        List<String> inputCommand;

        // Should match the name of the sprite animation.
        //
        String name;

        int currentInputCommandIndex;

        public List<String> InputCommand
        {
            get { return inputCommand; }
            set { inputCommand = value; }
        }

        public int CurrentInputCommandIndex
        {
            get { return currentInputCommandIndex; }
            set { currentInputCommandIndex = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public MoveInput(String name, List<String> inputCommand)
        {
            this.name = name;
            this.inputCommand = inputCommand;
            currentInputCommandIndex = 0;
        }

        public void resetCurrentInputCommandIndex()
        {
            CurrentInputCommandIndex = 0;
        }

        public void moveCurrentInputCommandIndex()
        {
            CurrentInputCommandIndex++;
        }
        public static Boolean checkStringInputToKeyInput(String input, Keys[] keyboardState, Direction direction, Dictionary<string, Keys> controls)
        {
            // Check all types of command inputs to see if there is a match Maybe not very efficient here Try to do as much shortcutting
            // To determine priority and also make checks fast
            //
            if(input == "BC"&& KeyboardDown(keyboardState, controls["c"]) && KeyboardDown(keyboardState, controls["b"]))
            {
                return true;
            }
            if (input == "2C" && KeyboardDown(keyboardState, controls["c"]) &&
               (KeyboardDown(keyboardState, controls["down"])))
            {
                return true;
            }
            if (input == "4C" && KeyboardDown(keyboardState, controls["c"]) && !KeyboardDown(keyboardState, controls["down"]) &&
                ((KeyboardDown(keyboardState, controls["left"]) && direction == Direction.Right) || KeyboardDown(keyboardState, controls["right"]) && direction == Direction.Left))
            {
                return true;
            }
            if (input == "6C" && KeyboardDown(keyboardState, controls["c"]) && !KeyboardDown(keyboardState, controls["down"]) &&
                ((KeyboardDown(keyboardState, controls["right"]) && direction == Direction.Right) || KeyboardDown(keyboardState, controls["left"]) && direction == Direction.Left))
            {
                return true;
            }
            if (input == "6B" && KeyboardDown(keyboardState, controls["b"]) &&
                ((KeyboardDown(keyboardState, controls["right"]) && direction == Direction.Right) || KeyboardDown(keyboardState, controls["left"]) && direction == Direction.Left))
            {
                return true;
            }
            if (input == "6A" && KeyboardDown(keyboardState, controls["a"]) &&
                ((KeyboardDown(keyboardState, controls["right"]) && direction == Direction.Right) || KeyboardDown(keyboardState, controls["left"]) && direction == Direction.Left))
            {
                return true;
            }
           
            if (input == "2B" && KeyboardDown(keyboardState, controls["b"]) &&
                (KeyboardDown(keyboardState, controls["down"])))
            {
                return true;
            }
            if (input == "2A" && KeyboardDown(keyboardState, controls["a"]) &&
                (KeyboardDown(keyboardState, controls["down"])))
            {
                return true;
            }
            if (input == "C" && KeyboardDown(keyboardState, controls["c"]) &&
                (!KeyboardDown(keyboardState, controls["down"])))
            {
                return true;
            }
            if (input == "B" && KeyboardDown(keyboardState, controls["b"]) &&
                (!KeyboardDown(keyboardState, controls["down"])))
            {
                return true;
            }
            if (input == "A" && KeyboardDown(keyboardState, controls["a"]) &&
                (!KeyboardDown(keyboardState, controls["down"])))
            {               
                return true;
            }
            if (KeyboardDown(keyboardState, controls["right"]) && KeyboardDown(keyboardState, controls["down"]) && ((direction == Direction.Right && input == "3") || (direction == Direction.Left && input == "1")))
            {
                return true;
            }

            if (KeyboardDown(keyboardState, controls["left"]) && KeyboardDown(keyboardState, controls["down"]) && ((direction == Direction.Right && input == "1") || (direction == Direction.Left && input == "3")))
            {
                return true;
            }
            if ( KeyboardDown(keyboardState, controls["right"]) && ((direction == Direction.Right && input == "6") || (direction == Direction.Left && input =="4")))
            {
                return true;
            }
            if (KeyboardDown(keyboardState, controls["left"]) && ((direction == Direction.Right && input == "4") || (direction == Direction.Left && input == "6")))
            {
                return true;
            }
            if (input == "2" && KeyboardDown(keyboardState, controls["down"]) && !KeyboardDown(keyboardState, controls["left"]) && !KeyboardDown(keyboardState, controls["right"]))
            {
                return true;
            }
         
            if (input == "5" && !KeyboardDown(keyboardState, controls["down"]) && !KeyboardDown(keyboardState, controls["up"]) && !KeyboardDown(keyboardState, controls["right"]) && !KeyboardDown(keyboardState, controls["left"]))
            {
                return true;
            }
            
            return false;
        }

        public static Boolean checkStringInputToKeyInputForMovement(String input, Keys[] keyboardState, Direction direction, Dictionary<string, Keys> controls)
        {

            if (KeyboardDown(keyboardState, controls["right"]) && ((direction == Direction.Right && input == "6") || (direction == Direction.Left && input == "4")))
            {
                return true;
            }
            if (KeyboardDown(keyboardState, controls["right"]) && KeyboardDown(keyboardState, controls["down"]) && ((direction == Direction.Right && input == "3") || (direction == Direction.Left && input == "1")))
            {
                return true;
            }
            if (KeyboardDown(keyboardState, controls["left"]) && KeyboardDown(keyboardState, controls["down"]) && ((direction == Direction.Right && input == "1") || (direction == Direction.Left && input == "3")))
            {
                return true;
            }
            if (input == "2" && KeyboardDown(keyboardState, controls["down"]))
            {
                return true;
            }
            if (KeyboardDown(keyboardState, controls["left"]) && ((direction == Direction.Right && input == "4") || (direction == Direction.Left && input == "6")))
            {
                return true;
            }
            if (input == "5" && !KeyboardDown(keyboardState, controls["down"]) && !KeyboardDown(keyboardState, controls["up"]) && !KeyboardDown(keyboardState, controls["right"]) && !KeyboardDown(keyboardState, controls["left"]))
            {
                return true;
            }

            return false;
        }

        public static bool KeyboardPressed(KeyboardState keyboardState, KeyboardState lastKeyboardState, Keys key)
        {
            return (keyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key));
        }

        public static bool KeyboardReleased(KeyboardState keyboardState, KeyboardState lastKeyboardState, Keys key)
        {
            return (keyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key));
        }

        public static bool KeyboardDown(Keys[] keyboardState, Keys key)
        {
            if (keyboardState == null)
            {
                return false;
            }
            int pos = Array.IndexOf(keyboardState, key);
            if(pos >=0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
