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
    public class NativeMethods
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
        public static extern SecurityStatus FreeContextBuffer( IntPtr buffer );

    }
}
