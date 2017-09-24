using System;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents the credentials of the user running the current process, for use as an SSPI client.
    /// </summary>
    public class ClientCurrentCredential : CurrentCredential
    {
        /// <summary>
        /// Initializes a new instance of the ClientCredential class.
        /// </summary>
        /// <param name="package">The security package to acquire the credential handle from.</param>
        public ClientCurrentCredential( string package )
            : base( package, CredentialUse.Outbound )
        {
        }
    }
}