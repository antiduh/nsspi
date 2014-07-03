using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Buffers
{
    /// <summary>
    /// Represents a native SecureBuffer structure, which is used for communicating
    /// buffers to the native APIs.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct SecureBufferInternal
    {
        /// <summary>
        /// When provided to the native API, the total number of bytes available in the buffer.
        /// On return from the native API, the number of bytes that were filled or used by the
        /// native API.
        /// </summary>
        public int Count;

        /// <summary>
        /// The type or purpose of the buffer.
        /// </summary>
        public BufferType Type;

        /// <summary>
        /// An pointer to a pinned byte[] buffer.
        /// </summary>
        public IntPtr Buffer;
    }

    /// <summary>
    /// Stores buffers to provide tokens and data to the native SSPI APIs.
    /// </summary>
    /// <remarks>The buffer is translated into a SecureBufferInternal for the actual call.
    /// To keep the call setup code simple, and to centralize the buffer pinning code,
    /// this class stores and returns buffers as regular byte arrays. The buffer 
    /// pinning support code in SecureBufferAdapter handles conversion to SecureBufferInternal
    /// for pass to the managed api, as well as pinning relevant chunks of memory.
    /// 
    /// Furthermore, the native API may not use the entire buffer, and so a mechanism
    /// is needed to communicate the usage of the buffer separate from the length
    /// of the buffer.</remarks>
    internal class SecureBuffer
    {
        /// <summary>
        /// Initializes a new instance of the SecureBuffer class.
        /// </summary>
        /// <param name="buffer">The buffer to wrap.</param>
        /// <param name="type">The type or purpose of the buffer, for purposes of 
        /// invoking the native API.</param>
        public SecureBuffer( byte[] buffer, BufferType type )
        {
            this.Buffer = buffer;
            this.Type = type;
            this.Length = this.Buffer.Length;
        }

        /// <summary>
        /// The type or purposes of the API, for invoking the native API.
        /// </summary>
        public BufferType Type { get; set; }

        /// <summary>
        /// The buffer to provide to the native API.
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// The number of elements that were actually filled or used by the native API,
        /// which may be less than the total length of the buffer.
        /// </summary>
        public int Length { get; internal set; }
    }
}
