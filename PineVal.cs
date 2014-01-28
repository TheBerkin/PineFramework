using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Allows for implicit compatibility between PINE objects and constants. Cannot be linked to a device.
    /// </summary>
    public class PineVal : PineObject
    {
        private double _value;

        public PineVal(double value) : base(null)
        {
            _value = value;
        }

        public static implicit operator double(PineVal input)
        {
            return input._value;
        }

        public static implicit operator PineVal(double value)
        {
            return new PineVal(value);
        }

        public override double Output
        {
            get { return _value; }
        }
    }
}
