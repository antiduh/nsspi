using System;
using System.Runtime.InteropServices;

namespace NSspi.Contexts
{
    /// <summary>
    /// Stores the result of a context query for the context's buffer sizes.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct SecPkgContext_Sizes
    {
        public int MaxToken;
        public int MaxSignature;
        public int BlockSize;
        public int SecurityTrailer;
    }

    /// <summary>
    /// Stores the result of a context query for a string-valued context attribute.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct SecPkgContext_String
    {
        public IntPtr StringResult;
    }
}