using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{

    [StructLayout( LayoutKind.Sequential )]
    public struct SecPkgContext_Sizes
    {
        public int MaxToken;
        public int MaxSignature;
        public int BlockSize;
        public int SecurityTrailer;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct SecPkgContext_String
    {
        public IntPtr StringResult;
    }


    public class ContextQueryHandle : SafeHandle
    {
        public ContextQueryHandle() :
            base( IntPtr.Zero, true )
        {
        }

        public override bool IsInvalid
        {
            get { return base.handle.Equals( IntPtr.Zero ); }
        }

        protected override bool ReleaseHandle()
        {
            return ContextNativeMethods.FreeContextBuffer( base.handle ) == SecurityStatus.OK;
        }
    }
}
