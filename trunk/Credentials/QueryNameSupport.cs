﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials.Credentials
{
    [StructLayout( LayoutKind.Sequential )]
    public struct QueryNameAttribCarrier
    {
        public IntPtr Name;
    }
}
