using System;
using System.Runtime.InteropServices;

namespace NSspi.Contexts
{
    public class SafeTokenHandle : SafeHandle
    {
        public SafeTokenHandle() : base( IntPtr.Zero, true )
        {
        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero || handle == new IntPtr( -1 );
            }
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.CloseHandle( this.handle );

            return true;
        }
    }
}
