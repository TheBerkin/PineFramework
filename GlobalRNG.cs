using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace PineFramework
{
    /// <summary>
    /// Provides a thread-safe method for generating random numbers.
    /// </summary>
    internal static class GlobalRNG
    {
        private static RNGCryptoServiceProvider _global =
        new RNGCryptoServiceProvider();

        [ThreadStatic]
        private static Random _local;

        public static double NextDouble()
        {            
            Random inst = _local;
            if (inst == null)
            {
                byte[] buffer = new byte[4];
                _global.GetBytes(buffer);
                _local = inst = new Random(
                    BitConverter.ToInt32(buffer, 0));
            }
            return inst.NextDouble();
        }
    }
}
