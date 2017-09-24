using System;
using System.Runtime.InteropServices;

namespace NSspi.Buffers
{
    /// <summary>
    /// Represents the native layout of the secure buffer descriptor that is provided directly
    /// to native API calls.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct SecureBufferDescInternal
    {
        /// <summary>
        /// The buffer structure version.
        /// </summary>
        public int Version;

        /// <summary>
        /// The number of buffers represented by this descriptor.
        /// </summary>
        public int NumBuffers;

        /// <summary>
        /// A pointer to a array of buffers, where each buffer is a byte[].
        /// </summary>
        public IntPtr Buffers;

        /// <summary>
        /// Indicates the buffer structure version supported by this structure. Always 0.
        /// </summary>
        public const int ApiVersion = 0;
    }
}