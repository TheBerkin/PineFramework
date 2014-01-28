using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    public enum PineBlendTechnique
    {
        /// <summary>
        /// Interpolates between two values at a fixed rate.
        /// </summary>
        Step,
        /// <summary>
        /// Interpolates at a steadily decreasing rate.
        /// </summary>
        Taper,
        /// <summary>
        /// Interpolates at a steadily increasing rate.
        /// </summary>
        InverseTaper
    }
}
