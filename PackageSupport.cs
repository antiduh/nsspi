using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public static class PackageSupport
    {
        public static SecPkgInfo GetPackageCapabilities( string packageName )
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

                if( status == SecurityStatus.OK && rawInfoPtr != IntPtr.Zero )
                {
                    Marshal.PtrToStructure( rawInfoPtr, info );
                    freeStatus = NativeMethods.FreeContextBuffer( rawInfoPtr );
                }
            }

            return info;
        }

    }
}
