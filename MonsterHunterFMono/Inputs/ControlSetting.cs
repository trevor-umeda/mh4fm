using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MonsterHunterFMono
{
    public class ControlSetting
    {
        // Maybe the commands should be enums
        //
        Dictionary<string, Keys> controls;

        public ControlSetting()
        {
            controls = new Dictionary<string, Keys>();
        }

        public Dictionary<string, Keys> Controls
        {
            get { return controls; } 
        }

        public void setControl(String command, Keys input)
        {
            controls[command] = input;
        }
    }
}
