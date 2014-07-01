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
        public ServerCredential( string package )
            : base( package, CredentialUse.Inbound )
        {
        }
    }
}
