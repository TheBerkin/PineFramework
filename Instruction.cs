using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    internal enum Instruction : byte
    {
        PushC =         0x00,
        PushX =         0x01,
        Add =           0x02, // a,b
        Subtract =      0x03, // a,b
        Multiply =      0x04, // a,b
        Divide =        0x05, // a,b
        Lerp =          0x06, // f,low,high
        Clamp =         0x07, // n,low,high
        Modulo =        0x08, // a,b
        SquareRoot =    0x09, // a
        Power =         0x0A, // a,exp
        Ceiling =       0x0B, // a
        Floor =         0x0C, // a
        RangeStart =    0x0D, // a,b
        RangeEnd =      0x0E,
        Sin =           0x0F, // a
        Cos =           0x10, // a
        Tan =           0x11, // a
        Atan =          0x12, // a
        Abs =           0x13, // a
        Rand =          0x14, // min,max
        Copy =          0x15,
        LE =            0x16, // a,b
        GE =            0x17, // a,b
        LT =            0x18, // a,b
        GT =            0x19, // a,b
        PushReg =       0x1A,
        Pop =           0x1B,
        Fire =          0x1C,
        Zero =          0x1D,
        JumpNotZero =   0x1E,
        Jump =          0x1F,
        Not =           0x20,
        JumpAlternate = 0x21,
        Return =        0x22,
        Call =          0x23,
        Equal =         0x24,
        NotEqual =      0x25,
        Sum =           0x26,
        Product =       0x27,
        InvLerp =       0x28,
        Stop =          0x29,
        OutS =          0x2A,
        OutR =          0x2B,
        OutC =          0x2C
    }
}
