using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    [StructLayout( LayoutKind.Sequential )]
    public struct TimeStamp
    {
        public static readonly DateTime Epoch = new DateTime( 1601, 1, 1, 0, 0, 0, DateTimeKind.Utc );

        private long time;

        public DateTime ToDateTime()
        {
            ulong test = (ulong)this.time + (ulong)(Epoch.Ticks);

            // Sometimes the value returned is massive, eg, 0x7fffff154e84ffff, which is a value 
            // somewhere in the year 30848. This would overflow DateTime, since it peaks at 31-Dec-9999.
            // http://stackoverflow.com/questions/24478056/
            if ( test > (ulong)DateTime.MaxValue.Ticks )
            {
                return DateTime.MaxValue;
            }
            else
            {
                return DateTime.FromFileTimeUtc( this.time );
            }
        }
    }
}
