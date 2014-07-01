using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents the credentials of the user running the current process, for use as an SSPI client.
    /// </summary>
    public class ClientCredential : CurrentCredential
    {
        /// <summary>
        /// Initializes a new instance of the ClientCredential class.
        /// </summary>
        /// <param name="package">The security package to acquire the credential handle from.</param>
        public ClientCredential( string package )
            : base( package, CredentialUse.Outbound )
        {
        }
    }
}
