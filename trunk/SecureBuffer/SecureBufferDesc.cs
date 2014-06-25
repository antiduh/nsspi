using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Buffers
{
    [StructLayout( LayoutKind.Sequential)]
    public struct SecureBufferDescInternal
    {
        public int Version;
        public int NumBuffers;

        // A pointer to a SecureBuffer[]
        public IntPtr Buffers;

        public const int ApiVersion = 0;
    }
}
