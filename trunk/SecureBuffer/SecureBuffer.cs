using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Buffers
{
    [StructLayout( LayoutKind.Sequential )]
    public struct SecureBufferInternal
    {
        public int Count;

        public BufferType Type;

        // A pointer to a byte[]
        public IntPtr Buffer;
    }

    public class SecureBuffer
    {
        public SecureBuffer( byte[] buffer, BufferType type )
        {
            this.Buffer = buffer;
            this.Type = type;
            this.Length = this.Buffer.Length;
        }

        public BufferType Type { get; set; }

        public byte[] Buffer { get; set; }

        public int Length { get; internal set; }
    }
}
