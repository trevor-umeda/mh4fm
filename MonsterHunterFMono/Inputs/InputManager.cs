using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections;
namespace MonsterHunterFMono
{
    public class InputManager
    {

        InputQueue<Keys[]> inputs;
        KeyboardState lastKeyboardState;
        private ControlSetting controlSetting;

        private Keys[] keysPressed;

        readonly String[] DIRECTIONS = {"up","down","left","right" };
        readonly String[] ATTACKS = { "a", "b", "c", "d"};

        List<MoveInput> groundMoveList;
        List<MoveInput> airMoveList;
        List<MoveInput> dashList;

        GatlingTable gatlingTable;

        public KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }
        public InputManager()
        {
            inputs = new InputQueue<Keys[]>();
            lastKeyboardState = new KeyboardState();
            groundMoveList = new List<MoveInput>();
            airMoveList = new List<MoveInput>();
            dashList = new List<MoveInput>();
            gatlingTable = new GatlingTable();
            // TODO: this is a terrible place to put these. find a better spot
            //
            dashList.Add(new MoveInput("backstep", new List<string> { "4", "5", "4" }));
            dashList.Add(new MoveInput("dash", new List<string> { "6", "5", "6" }));           
        }

        public String checkMoves(Direction direction, KeyboardState newKeyboardState, List<MoveInput> moveList, Boolean dashCancealable)
        {
          
            // on a button press determine if a special move was inputted.
            //
            if (DetermineButtonPress(newKeyboardState))
            {
                // Reset our command index for every special move
                //
                foreach (MoveInput moveInput in moveList)
                {
                    moveInput.resetCurrentInputCommandIndex();
                }
                // Iterate through the input queue
                //
                foreach (Keys[] keyboardState in inputs)
                {
                    // For every command
                    //
                    foreach (MoveInput moveInput in moveList)
                    {
                        // check to see if a pressed key in the current input state matches a necessary move input
                        //
                        if (MoveInput.checkStringInputToKeyInput(moveInput.InputCommand[moveInput.CurrentInputCommandIndex], keyboardState, direction, controlSetting.Controls))
                        {
                            // Move up our command index for that move
                            //
                            moveInput.moveCurrentInputCommandIndex();
                            // If our command index for a move is greater than how many inputs it needs, we must have input it
                            //
                            if (moveInput.CurrentInputCommandIndex >= moveInput.InputCommand.Count)
                            {

                                lastKeyboardState = newKeyboardState;
                                Console.WriteLine("Activating " + moveInput.Name);
                                // Clear up our input queue
                                //
                                inputs.Reset();
                                return moveInput.Name;
                            }
                        }
                    }
                }
            }
            // Otherwise this is a movement special input like a dash
            //
            else if(dashCancealable)
            {
                // Atm we only really care about reading a dash
                //
                foreach (MoveInput dashInput in dashList)
                {
                    dashInput.resetCurrentInputCommandIndex();
                }

                foreach (Keys[] keyboardState in inputs.GetReverseEnumerator)
                {
                    foreach (MoveInput dash in dashList)
                    {
                        if (MoveInput.checkStringInputToKeyInputForMovement(dash.InputCommand[dash.CurrentInputCommandIndex], keyboardState, direction, controlSetting.Controls))
                        {
                            dash.moveCurrentInputCommandIndex();
                            if (dash.CurrentInputCommandIndex >= dash.InputCommand.Count)
                            {
                                System.Diagnostics.Debug.WriteLine(dash.Name);
                                //inputs.Reset();
                                return dash.Name;
                            }
                        }
                    }
                }
            }
            lastKeyboardState = newKeyboardState;
            return null;
        }

        public String checkGroundMoves(Direction direction, String currentMove, KeyboardState newKeyboardState)
        {
            // Check to see if we have a gatling table for the move. otherwise just have everything
            //
            List<MoveInput> possibleMoveList = gatlingTable.getPossibleGatlings(currentMove);
            Boolean dashCancealable = false;
            if (possibleMoveList == null)
            {
                possibleMoveList = groundMoveList;
                dashCancealable = true;
            }
            else
            {
                
            }
            return checkMoves(direction, newKeyboardState, possibleMoveList, dashCancealable);
        }

        public String checkAirMoves(Direction direction, String currentMove, KeyboardState newKeyboardState)
        {
            // Check to see if we have a gatling table for the move. otherwise just have everything
            //
            List<MoveInput> possibleMoveList = gatlingTable.getPossibleGatlings(currentMove);
            if (possibleMoveList == null)
            {
                possibleMoveList = airMoveList;
            }
            return checkMoves(direction, newKeyboardState, possibleMoveList, true);
        }

        public bool DetermineJumpPress(KeyboardState presentState)
        {
            if (MoveInput.KeyboardPressed(presentState, lastKeyboardState, controlSetting.Controls["up"]))
            {
                Console.WriteLine("Jump Button");
                return true;
            }
            else return false;
        }

        public bool DetermineButtonPress(KeyboardState presentState)
        {
            //A attack button pressed
            if (MoveInput.KeyboardPressed(presentState, lastKeyboardState, controlSetting.Controls["a"]))
            {
                System.Diagnostics.Debug.WriteLine("A button pressed");
                return true;
            }
            // B attack button pressed

            if (MoveInput.KeyboardPressed(presentState, lastKeyboardState, controlSetting.Controls["b"]))
            {
                System.Diagnostics.Debug.WriteLine("B button pressed");
                return true;
            }

            if (MoveInput.KeyboardPressed(presentState, lastKeyboardState, controlSetting.Controls["c"]))
            {
                System.Diagnostics.Debug.WriteLine("C button pressed");
                return true;
            }

            if (MoveInput.KeyboardPressed(presentState, lastKeyboardState, controlSetting.Controls["d"]))
            {
                System.Diagnostics.Debug.WriteLine("d button pressed");
                return true;
            }

            return false;
        }

        public void registerGatling(String name, List<MoveInput> inputs)
        {
            gatlingTable.addGatling(name, inputs);
        }

        public void registerGroundMove(String name, List<String> input)
        {
           // input.Reverse();
            groundMoveList.Add(new MoveInput(name, input));
        }

        public void registerAirMove(String name, List<String> input)
        {
            airMoveList.Add(new MoveInput(name, input));
        }

        public void enqueueState(KeyboardState state, Dictionary<string, Keys> controls)
        {
            keysPressed = new Keys[state.GetPressedKeys().Length];
            int counter = 0;
          
            foreach (String attack in ATTACKS)
            {
                if (MoveInput.KeyboardPressed(state, lastKeyboardState, controls[attack]))
                {
                    
                    keysPressed[counter] = controls[attack];
                    counter++;
                }
            }
            foreach(String direction in DIRECTIONS)
            {
                if (state.IsKeyDown(controls[direction]))
                { 
                    keysPressed[counter] = controls[direction];
                    counter++;
                }
            }
            inputs.Enqueue(keysPressed);
        }
        
        public ControlSetting ControlSetting
        {
            get { return controlSetting; }
            set { this.controlSetting = value; }
        }

        public GatlingTable GatlingTable
        {
            get { return gatlingTable; }
            set { this.gatlingTable = value; }
        }
    }
}
