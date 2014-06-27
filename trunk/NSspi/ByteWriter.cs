using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public static class ByteWriter
    {
        // Big endian: Most significant byte at lowest address in memory.
        
        public static void WriteInt16_BE( Int16 value, byte[] buffer, int position )
        {
            buffer[position + 0] = (byte)( value >> 8 );
            buffer[position + 1] = (byte)( value );
        }

        public static void WriteInt32_BE( Int32 value, byte[] buffer, int position )
        {
            buffer[position + 0] = (byte)( value >> 24 );
            buffer[position + 1] = (byte)( value >> 16 );
            buffer[position + 2] = (byte)( value >> 8 );
            buffer[position + 3] = (byte)( value);

        }

        public static Int16 ReadInt16_BE( byte[] buffer, int position )
        {
            Int16 value;

            value = (Int16)( buffer[position + 0] << 8 );
            value += (Int16)( buffer[position + 1] );

            return value;
        }

        public static Int32 ReadInt32_BE( byte[] buffer, int position )
        {
            Int32 value;

            value = (Int32)( buffer[position + 0] << 24 );
            value |= (Int32)( buffer[position + 1] << 16 );
            value |= (Int32)( buffer[position + 2] << 8 );
            value |= (Int32)( buffer[position + 3] );

            return value;
        }

    }
}
