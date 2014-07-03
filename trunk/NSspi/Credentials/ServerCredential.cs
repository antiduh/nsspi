using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents the credentials of the user running the current process, for use as an SSPI server.
    /// </summary>
    public class ServerCredential : CurrentCredential
    {
        /// <summary>
        /// Initializes a new instance of the ServerCredential class, acquiring credentials from 
        /// the current thread's security context.
        /// </summary>
        /// <param name="package">The name of the security package to obtain credentials from.</param>
        public ServerCredential( string package )
            : base( package, CredentialUse.Inbound )
        {
        }
    }
}
