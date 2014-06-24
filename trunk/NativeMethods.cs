﻿using NSspi.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        [DllImport(
            "Secur32.dll",
            EntryPoint = "FreeContextBuffer",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus FreeContextBuffer( IntPtr buffer );

    }
}