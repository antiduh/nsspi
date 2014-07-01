using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NSspi.Contexts;

namespace NSspi
{
    internal static class NativeMethods
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa374713(v=vs.85).aspx

        // The REMSSPI sample:

        // A C++ pure client/server example:
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa380536(v=vs.85).aspx

 
        /*
        SECURITY_STATUS SEC_Entry FreeContextBuffer(
          _In_  PVOID pvContextBuffer
        );
        */

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport( "Secur32.dll", EntryPoint = "FreeContextBuffer", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus FreeContextBuffer( IntPtr buffer );


        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "QuerySecurityPackageInfo", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus QuerySecurityPackageInfo( string packageName, ref IntPtr pkgInfo );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "EnumerateSecurityPackages", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus EnumerateSecurityPackages( ref int numPackages, ref IntPtr pkgInfoArry );

    }
}
