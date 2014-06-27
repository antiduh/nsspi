using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    internal static class PackageSupport
    {
        internal static SecPkgInfo GetPackageCapabilities( string packageName )
        {
            SecPkgInfo info;
            SecurityStatus status;
            SecurityStatus freeStatus;

            IntPtr rawInfoPtr;
            
            rawInfoPtr = new IntPtr();
            info = new SecPkgInfo();

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            { }
            finally
            {
                status = NativeMethods.QuerySecurityPackageInfo( packageName, ref rawInfoPtr );

                if ( rawInfoPtr != IntPtr.Zero )
                {
                    try
                    {
                        if ( status == SecurityStatus.OK )
                        {
                            // This performs allocations as it makes room for the strings contained in the SecPkgInfo class.
                            Marshal.PtrToStructure( rawInfoPtr, info );
                        }
                    }
                    finally
                    {
                        freeStatus = NativeMethods.FreeContextBuffer( rawInfoPtr );
                    }
                }
            }

            return info;
        }

    }
}
