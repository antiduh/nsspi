using System;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents a handle to the credentials of the user running the current process, to be used to
    /// authenticate as a client.
    /// </summary>
    public class ClientCurrentCredential : CurrentCredential
    {
        /// <summary>
        /// Initializes a new instance of the ClientCurrentCredential class.
        /// </summary>
        /// <param name="package">The security package to acquire the credential handle from.</param>
        public ClientCurrentCredential( string package )
            : base( package, CredentialUse.Outbound )
        {
        }
    }
}