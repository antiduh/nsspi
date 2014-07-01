﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials.Credentials
{
    /// <summary>
    /// Stores the result from a query of a credential's principle name.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    internal struct QueryNameAttribCarrier
    {
        /// <summary>
        /// A pointer to a null-terminated ascii c-string containing the principle name 
        /// associated with a credential
        /// </summary>
        public IntPtr Name;
    }
}