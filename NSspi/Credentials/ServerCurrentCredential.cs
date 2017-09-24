using System;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents the credentials of the user running the current process, for use as an SSPI server.
    /// </summary>
    public class ServerCurrentCredential : CurrentCredential
    {
        /// <summary>
        /// Initializes a new instance of the ServerCredential class, acquiring credentials from
        /// the current thread's security context.
        /// </summary>
        /// <param name="package">The name of the security package to obtain credentials from.</param>
        public ServerCurrentCredential( string package )
            : base( package, CredentialUse.Inbound )
        {
        }
    }
}