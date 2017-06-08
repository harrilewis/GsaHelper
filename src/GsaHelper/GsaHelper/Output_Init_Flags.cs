using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaHelper
{
    //an enum for the flags for the output data retrieval stuff
    public enum Output_Init_Flags
    {
        OP_INIT_2D_BOTTOM = 0x01,    //output 2D stresses at bottom layer
        OP_INIT_2D_MIDDLE = 0x02,    //output 2D stresses at middle layer
        OP_INIT_2D_TOP = 0x04,       //output 2D stresses at top layer
        OP_INIT_2D_BENDING = 0x08,   //output 2D stresses at bending layer
        OP_INIT_2D_AVGE = 0x10,     //average 2D element stresses at nodes
        OP_INIT_1D_AUTO_PTS = 0x20, //calculate 1D results at interesting points
        OP_INIT_INFINITY = 0x40,    //return infinity and NaN values as such, else as zero
        OP_INIT_1D_WALL_RES_SECONDARY = 0x80    //output secondary stick of wall equivalent beam results, else primary
    }
}
