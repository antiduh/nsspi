using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    [StructLayout( LayoutKind.Sequential )]
    public unsafe struct SecureBuffer
    {
        public int Count;

        public BufferType Type;

        // A pointer to a byte[]
        public IntPtr Buffer;
    }
}
