using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    static class Extensions
    {
        public static bool InRange(this double d, double min, double max)
        {
            return d <= max && d >= min;
        }
    }
}
