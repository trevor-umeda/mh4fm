using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonsterHunterFMono
{
    class PauseMenu
    {
        private String[] menuList;
        private int selection;
        KeyboardState prevState;
        private int numOfOptions;

        private String selectedMenu;
        private SpriteFont spriteFont;
        public String SelectedMenu
        {
            get { return selectedMenu; }
            set { selectedMenu = value; }
        }

        public Boolean menuItemSelected()
        {
            return selectedMenu != null;
        }

        public PauseMenu(SpriteFont font)
        {   
            menuList = new String[3];
            menuList[0] = "Resume";
            menuList[1] = "Controller Settings";
            menuList[2] = "Exit";

            numOfOptions = menuList.Length;
            selection = 0;
            selectedMenu = null;
            spriteFont = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < numOfOptions; i ++ )
            {
                int height = ((i * 100) + 100);
                if (i == selection)
                {
                    spriteBatch.DrawString(spriteFont, menuList[i], new Vector2(500, height), Color.Yellow, 0, new Vector2(0, 0), 3, SpriteEffects.None, 0);
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
