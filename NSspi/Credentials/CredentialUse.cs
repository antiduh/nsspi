using System;

namespace NSspi.Credentials
{
    /// <summary>
    /// Indicates the manner in which a credential will be used for SSPI authentication.
    /// </summary>
    public enum CredentialUse : uint
    {
        /// <summary>
        /// The credentials will be used for establishing a security context with an inbound request, eg,
        /// the credentials will be used by a server building a security context with a client.
        /// </summary>
        Inbound = 1,

        /// <summary>
        /// The credentials will be used for establishing a security context as an outbound request,
        /// eg, the credentials will be used by a client to build a security context with a server.
        /// </summary>
        Outbound = 2,

        /// <summary>
        /// The credentials may be used to to either build a client's security context or a server's
        /// security context.
        /// </summary>
        Both = 3,
    }
}