using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaHelper
{
    //useful struct for working with node results
    public struct NodeResult
    {
        public double[] values;
        public string units;
        public string title;

        public NodeResult(double[] tValues, string tUnits, string tTitle)
        {
            values = (double[])tValues.Clone();
            units = tUnits;
            title = tTitle;
        }
    }
}
