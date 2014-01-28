using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    /// <summary>
    /// Contains information sent by the "fire" instruction, which can be modified and reapplied to the event registers.
    /// </summary>
    public class CogFireEventArgs : EventArgs
    {
        /// <summary>
        /// The EvA register value.
        /// </summary>
        public double A;
        /// <summary>
        /// The EvB register value.
        /// </summary>
        public double B;
        /// <summary>
        /// The EvC register value.
        /// </summary>
        public double C;
        /// <summary>
        /// The EvD register value.
        /// </summary>
        public double D;
        /// <summary>
        /// The EvE register value.
        /// </summary>
        public double E;
        /// <summary>
        /// The EvF register value.
        /// </summary>
        public double F;

        public CogFireEventArgs(double a, double b, double c, double d, double e, double f)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
            this.E = e;
            this.F = f;
        }

        /// <summary>
        /// Sets all event values to zero.
        /// </summary>
        public void ZeroAll()
        {
            this.A = 0;
            this.B = 0;
            this.C = 0;
            this.D = 0;
            this.E = 0;
            this.F = 0;
        }
    }
}
