using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Common
{
    public struct Delay
    {
        public const int ShorterMilliseconds = 500;
        public const int ShortMilliseconds = 1000;
        public const int MediumMilliseconds = 3000;
        public const int LongMilliseconds = 8000;

        public static void Shorter()
        {
            Thread.Sleep(ShorterMilliseconds);
        }

        public static void Short()
        {
            Thread.Sleep(ShortMilliseconds);
        }
        internal static async Task ShortAsync()
        {
            await Task.Delay(ShortMilliseconds);
        }

        public static void Medium()
        {
            Thread.Sleep(MediumMilliseconds);
        }
        internal static async Task MediumAsync()
        {
            await Task.Delay(MediumMilliseconds);
        }

        public static void Long()
        {
            Thread.Sleep(LongMilliseconds);
        }

        internal static async Task LongAsync()
        {
            await Task.Delay(LongMilliseconds);
        }
    }
}
