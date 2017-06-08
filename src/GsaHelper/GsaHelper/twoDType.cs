using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaHelper
{
    //struct to define 2d element types
    public struct TwoDType
    {
        public static string SHELL = "SHELL";
        public static string PLANESTRESS = "STRESS";
        public static string FABRIC = "FABRIC";
        public static string FLATPLATE = "PLATE";
        public static string CURVEDSHELL = "CURVED";
        public static string LOADPANEL = "LOAD";
        public static string WALL = "WALL";
    }
}
