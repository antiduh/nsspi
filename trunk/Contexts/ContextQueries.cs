using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{

    [StructLayout( LayoutKind.Sequential )]
    internal struct SecPkgContext_Sizes
    {
        public int MaxToken;
        public int MaxSignature;
        public int BlockSize;
        public int SecurityTrailer;
    }

    [StructLayout( LayoutKind.Sequential )]
    internal struct SecPkgContext_String
    {
        public IntPtr StringResult;
    }
}
