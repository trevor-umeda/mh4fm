using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterHunterFMono
{
    class CharacterSelectList
    {

        private CharacterSelectNode[,] characterSelection;
         
        private Vector2 player1Selection;
        private Vector2 player2Selection;

        private String player1CharacterId;
        private String player2CharacterId;

        Texture2D blankBox;

        private int width;
        private int height;

        KeyboardState prevState;

        public String Player1CharacterId { get { return player1CharacterId; } }
        public String Player2CharacterId { get { return player2CharacterId; } }

        public CharacterSelectList(ContentManager content)
        {
            blankBox = content.Load<Texture2D>("HealthBar2");
            characterSelection = new CharacterSelectNode[2, 1];
            characterSelection[0, 0] = new CharacterSelectNode("LongSword", new Vector2(100, 100), content.Load<Texture2D>("portraits/lsportrait"));
            characterSelection[1, 0] = new CharacterSelectNode("LongSword", new Vector2(700, 100), content.Load<Texture2D>("portraits/lsportrait"));
            width = 2;
            height = 1;

            player1CharacterId = null;
            player2CharacterId = null;
        }

        public void moveCharacterSelection(KeyboardState key, Dictionary<string, Keys> controls, Dictionary<string, Keys> controls2)
        {
            if (player1CharacterId == null)
            {
                moveCharacterSelection(1, key, controls);
            }
            if (player2CharacterId == null)
            {
                moveCharacterSelection(2, key, controls2);
            }
            selectCharacter(1, key, controls);
            selectCharacter(2, key, controls2);
            prevState = key;
        }

        private void selectCharacter(int playerNum, KeyboardState key, Dictionary<string, Keys> controls)
        {
            if (key.IsKeyDown(controls["a"]) && prevState.IsKeyUp(controls["a"]))
            {
                if (playerNum == 1)
                {
                    player1CharacterId = characterSelection[(int)player1Selection.X, (int)player1Selection.Y].CharacterId;
                }
                else
                {
                    player2CharacterId = characterSelection[(int)player2Selection.X, (int)player2Selection.Y].CharacterId;
                }
            }
            if (key.IsKeyDown(controls["b"]) && prevState.IsKeyUp(controls["b"]))
            {
                if (playerNum == 1)
                {
                    player1CharacterId = null;
                }
                else
                {
                    player2CharacterId = null;
                }
            }
        }

        private void moveCharacterSelection(int playerNum, KeyboardState key, Dictionary<string, Keys> controls)
        {
            if (key.IsKeyDown(controls["right"]) && prevState.IsKeyUp(controls["right"]))
            {
                if (playerNum == 1)
                {
                    player1Selection.X = (player1Selection.X + 1) % width;
                }
                else
                {
                    player2Selection.X = (player2Selection.X + 1) % width;
                }
            }
            if (key.IsKeyDown(controls["left"]) && prevState.IsKeyUp(controls["left"]))
            {
                if (playerNum == 1)
                {
                    if (player1Selection.X == 0)
                    {
                        player1Selection.X = width - 1;
                    }
                    else
                    {
                        player1Selection.X = (player1Selection.X - 1) % width;
                    }
                }
                else
                {
                    if (player2Selection.X == 0)
                    {
                        player2Selection.X = width - 1;
                    }
                    else
                    {
                        player2Selection.X = (player2Selection.X - 1) % width;
                    }
                }
            }
            if (key.IsKeyDown(controls["up"]) && prevState.IsKeyUp(controls["up"]))
            {
                if (playerNum == 1)
                {
                    if (player1Selection.Y == 0)
                    {
                        player1Selection.Y = height - 1;
                    }
                    else
                    {
                        player1Selection.Y = (player1Selection.Y - 1) % height;
                    }
                }
                else
                {
                    if (player2Selection.Y == 0)
                    {
                        player2Selection.Y = height - 1;
                    }
                    else
                    {
                        player2Selection.Y = (player2Selection.Y - 1) % height;
                    }
                }
               
            }
            if (key.IsKeyDown(controls["down"]) && prevState.IsKeyUp(controls["down"]))
            {
                if (playerNum == 1)
                {
                    player1Selection.Y = (player1Selection.Y + 1) % height;
                }
                else
                {
                    player2Selection.Y = (player2Selection.Y + 1) % height;
                }
            }

          
           
        }

        public Boolean selectionLocked()
        {
            return (player1CharacterId != null && player2CharacterId != null);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(CharacterSelectNode node in characterSelection)
            {
                if (node != null)
                {
                    node.Draw(spriteBatch, blankBox);
                }
                spriteBatch.Draw(blankBox, characterSelection[(int)player1Selection.X, (int)player1Selection.Y].DrawRectangle, new Rectangle(0, 0, 467, 44), Color.White);
                Color backgroundTint = Color.Lerp(Color.White, Color.Red, 0.5f);
                spriteBatch.Draw(blankBox, characterSelection[(int)player2Selection.X, (int)player2Selection.Y].DrawRectangle, new Rectangle(0, 0, 467, 44), backgroundTint);
            }

        }
    }
}
