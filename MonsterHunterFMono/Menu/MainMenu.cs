using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    class MainMenu
    {
        private String[] menuList;
        private int selection;
        KeyboardState prevState;
        private int numOfOptions;

        private String selectedMenu;
        private SpriteFont spriteFont;
        private Texture2D pressStart;

        int blinkTimer = 0;

        public String SelectedMenu
        {
            get { return selectedMenu; }
            set { selectedMenu = value; }
        }

        public Boolean menuItemSelected()
        {
            return selectedMenu != null;
        }

        public MainMenu(SpriteFont font, Texture2D pressStart)
        {   
            menuList = new String[1];
            menuList[0] = "Start";

            this.pressStart = pressStart;
            numOfOptions = menuList.Length;
            selection = 0;
            blinkTimer = 0;
            selectedMenu = null;
            spriteFont = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            blinkTimer += 1;
            blinkTimer = blinkTimer % 100;
            for (int i = 0; i < numOfOptions; i ++ )
            {
                int height = ((i * 100) + 600);
                if (i == selection)
                {
                    if (blinkTimer < 60)
                    {
                        spriteBatch.Draw(pressStart, new Rectangle(500, height - 50, 300, 97), Color.White);
                    }
                    
                 //   spriteBatch.DrawString(spriteFont, menuList[i], new Vector2(500, height), Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                   // spriteBatch.DrawString(spriteFont, menuList[i], new Vector2(501, height+1), Color.Black, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, menuList[i], new Vector2(500, height), Color.White, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
                }
                
            }
            
        }

        public void moveMenuSelection(KeyboardState key, Dictionary<string, Keys> controls, Dictionary<string, Keys> controls2)
        {
    
            moveMenuSelection(key, controls);
          
            moveMenuSelection(key, controls2);
                  
            selectMenuItem(key, controls);
            selectMenuItem(key, controls2);
            prevState = key;
        }
        public void resetMenuSelection()
        {
            selectedMenu = null;
        }
        private void selectMenuItem(KeyboardState key, Dictionary<string, Keys> controls)
        {
            if (key.IsKeyDown(controls["a"]) && prevState.IsKeyUp(controls["a"]))
            {
                selectedMenu = menuList[selection];
            }
            if (key.IsKeyDown(controls["start"]) && prevState.IsKeyUp(controls["start"]))
            {
                selectedMenu = menuList[selection];
            }
            if (key.IsKeyDown(controls["b"]) && prevState.IsKeyUp(controls["b"]))
            {
                selectedMenu = null;
            }
        }
        private void moveMenuSelection(KeyboardState key, Dictionary<string, Keys> controls)
        {

            if (key.IsKeyDown(controls["up"]) && prevState.IsKeyUp(controls["up"]))
            {

                if (selection == 0)
                {
                    selection = numOfOptions - 1;
                }
                else
                {
                    selection = (selection - 1) % numOfOptions;
                }
            }
            if (key.IsKeyDown(controls["down"]) && prevState.IsKeyUp(controls["down"]))
            {            
                selection = (selection + 1) % numOfOptions;             
            }
        }
    }
}
