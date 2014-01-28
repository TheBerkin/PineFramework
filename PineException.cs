using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineFramework
{
    [Serializable]
    public sealed class PineException : Exception
    {
        public PineException(string message) : base(message)
        {

        }

        public PineException(string message, params object[] args)
            : base(string.Format(message, args))
        {

        }
    }
}
