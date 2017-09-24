using System;
using System.Runtime.InteropServices;

namespace NSspi.Credentials
{
    /// <summary>
    /// Stores the result from a query of a credential's principle name.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct QueryNameAttribCarrier
    {
        /// <summary>
        /// A pointer to a null-terminated ascii-encoded containing the principle name
        /// associated with a credential
        /// </summary>
        public IntPtr Name;
    }
}