using System;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents a handle to the credentials of the user running the current process, to be used to
    /// authenticate as a server.
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