using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher
{
    class Config
    {
        public string gamepath;
        public string modname;
        public string commandline;
        public string botpath;
        public string botprofile;
        public string charname;
        public string botapi;

        public WindowConfig window;

        public int gamestarttimeout;
        public int accountloadtimeout;
    }

    class WindowConfig
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}
