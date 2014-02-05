using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    struct CogTimer
    {
        public bool Active;
        public uint Ticks;
        public uint Limit;

        public void Inc()
        {
            this.Ticks++;
        }

        public int Status()
        {
            if (!this.Active) return -1;
            if (Ticks < Limit)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public void Reset()
        {
            Ticks = 0;
            Active = false;
        }
    }
}
