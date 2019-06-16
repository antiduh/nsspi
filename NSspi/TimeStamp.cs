using System;
using System.Runtime.InteropServices;

namespace NSspi
{
    /// <summary>
    /// Represents a Windows API Timestamp structure, which stores time in units of 100 nanosecond
    /// ticks, counting from January 1st, year 1601 at 00:00 UTC. Time is stored as a 64-bit value.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct TimeStamp
    {
        /// <summary>
        /// Returns the calendar date and time corresponding a zero timestamp.
        /// </summary>
        public static readonly DateTime Epoch = new DateTime( 1601, 1, 1, 0, 0, 0, DateTimeKind.Utc );

        /// <summary>
        /// Stores the time value. Infinite times are often represented as values near, but not exactly
        /// at the maximum signed 64-bit 2's complement value.
        /// </summary>
        private long time;

        /// <summary>
        /// Converts the TimeStamp to an equivalant DateTime object. If the TimeStamp represents
        /// a value larger than DateTime.MaxValue, then DateTime.MaxValue is returned.
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            ulong test = (ulong)this.time + (ulong)( Epoch.Ticks );

            // Sometimes the value returned is massive, eg, 0x7fffff154e84ffff, which is a value
            // somewhere in the year 30848. This would overflow DateTime, since it peaks at 31-Dec-9999.
            // It turns out that this value corresponds to a TimeStamp's maximum value, reduced by my local timezone
            // http://stackoverflow.com/questions/24478056/
            if( test > (ulong)DateTime.MaxValue.Ticks )
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