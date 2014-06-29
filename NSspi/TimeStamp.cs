using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public static class TimeStamp
    {
        public static readonly DateTime Epoch = new DateTime( 1601, 1, 1, 0, 0, 0, DateTimeKind.Utc );

        public static DateTime Calc( long rawExpiry )
        {
            return TimeStamp.Epoch.AddTicks( rawExpiry );
        }
    }
}
