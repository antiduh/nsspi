using System;

namespace NSspi
{
    /// <summary>
    /// Reads and writes value types to byte arrays with explicit endianness.
    /// </summary>
    public static class ByteWriter
    {
        // Big endian: Most significant byte at lowest address in memory.

        /// <summary>
        /// Writes a 2-byte signed integer to the buffer in big-endian format.
        /// </summary>
        /// <param name="value">The value to write to the buffer.</param>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="position">The index of the first byte to write to.</param>
        public static void WriteInt16_BE( Int16 value, byte[] buffer, int position )
        {
            buffer[position + 0] = (byte)( value >> 8 );
            buffer[position + 1] = (byte)( value );
        }

        /// <summary>
        /// Writes a 4-byte signed integer to the buffer in big-endian format.
        /// </summary>
        /// <param name="value">The value to write to the buffer.</param>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="position">The index of the first byte to write to.</param>
        public static void WriteInt32_BE( Int32 value, byte[] buffer, int position )
        {
            buffer[position + 0] = (byte)( value >> 24 );
            buffer[position + 1] = (byte)( value >> 16 );
            buffer[position + 2] = (byte)( value >> 8 );
            buffer[position + 3] = (byte)( value );
        }

        /// <summary>
        /// Reads a 2-byte signed integer that is stored in the buffer in big-endian format.
        /// The returned value is in the native endianness.
        /// </summary>
        /// <param name="buffer">The buffer to read.</param>
        /// <param name="position">The index of the first byte to read.</param>
        /// <returns></returns>
        public static Int16 ReadInt16_BE( byte[] buffer, int position )
        {
            Int16 value;

            value = (Int16)( buffer[position + 0] << 8 );
            value += (Int16)( buffer[position + 1] );

            return value;
        }

        /// <summary>
        /// Reads a 4-byte signed integer that is stored in the buffer in big-endian format.
        /// The returned value is in the native endianness.
        /// </summary>
        /// <param name="buffer">The buffer to read.</param>
        /// <param name="position">The index of the first byte to read.</param>
        /// <returns></returns>
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