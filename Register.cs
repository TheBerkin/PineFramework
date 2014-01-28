using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    public enum CogRegister : int
    {
        A = 0, // A-L = persistent values
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11,
        Offset = 12, // Output offset
        Reset = 13, // Non-zero value will set X to 0 next cycle
        EvA = 14, // Ev A-F: Event registers. Can be used to pass values from EventArgs modified by external code.
        EvB = 15,
        EvC = 16,
        EvD = 17,
        EvE = 18,
        EvF = 19
    }
}
