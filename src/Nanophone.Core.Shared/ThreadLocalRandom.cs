using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nanophone.Core
{
    public static class ThreadLocalRandom
    {
        private static int s_seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> s_random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref s_seed)));

        public static Random Current => s_random.Value;
    }
}
