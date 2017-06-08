using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaHelper
{
    //useful struct for working with 1d results
    public struct OneDResult
    {
        public double[][] values;
        public string units;
        public string title;

        public OneDResult(double[][] tValues, string tUnits, string tTitle)
        {
            values = (double[][])tValues.Clone();
            units = tUnits;
            title = tTitle;
        }
    }
}
