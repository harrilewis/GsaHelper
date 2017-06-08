using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaHelper
{
    //as close as possible to the GsaSection struct. With a method to convert to a gwacommand
    public class Gsa2dSection
    {
        public int Ref;
        public string Name;
        public int Color;
        public bool LocalAxis;
        public int Material;
        public string type;
        public double thickness;
        public double additionalMass; //additional mass per unit area [kg/m2]
        public double bendingThicknessFactor;
        public double inPlaneThicknessFactor;
        public double massThicknessFactor;

        public Gsa2dSection(int tRef)
        {
            Ref = tRef;
            Name = "default";
            Color = (int)Colours.GREY;
            LocalAxis = true;
            Material = (int) StandardMaterial.STEEL;
            type = TwoDType.SHELL;
            thickness = 0.1;
            additionalMass = 0.0;
            bendingThicknessFactor = 100.0;
            inPlaneThicknessFactor = 100.0;
            massThicknessFactor = 100.0;
        }

        public string CreateGWACommand()
        {
            string command = "SET,PROP_2D,";
            command += this.Ref + "," + this.Name + "," + this.Color + ",";
            if (this.type == TwoDType.LOADPANEL)
            {
                command += "0,0," + this.type + ",SUP_AUTO";
            }
            else {
                if (LocalAxis)
                {
                    command += "LOCAL,";
                }
                else {
                    command += "GLOBAL,";
                }
                command += this.Material + "," + this.type + "," + this.thickness + "," + this.additionalMass + "," + this.bendingThicknessFactor + "%," + this.inPlaneThicknessFactor + "%," + this.massThicknessFactor + "%";
            }
            return command;
        }
    }
}
