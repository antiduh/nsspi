using System;

namespace NSspi.Contexts
{
    /// <summary>
    /// Represents impersonation performed on a server on behalf of a client.
    /// </summary>
    /// <remarks>
    /// The handle controls the lifetime of impersonation, and will revert the impersonation
    /// if it is disposed, or if it is finalized ie by being leaked and garbage collected.
    ///
    /// If the handle is accidentally leaked while operations are performed on behalf of the user,
    /// impersonation may be reverted at any arbitrary time, perhaps during those operations.
    /// This may lead to operations being performed in the security context of the server,
    /// potentially leading to security vulnerabilities.
    /// </remarks>
    public class ImpersonationHandle : IDisposable
    {
        private bool disposed;
        private ServerContext server;

        /// <summary>
        /// Initializes a new instance of the ImpersonationHandle. Does not perform impersonation.
        /// </summary>
        /// <param name="server">The server context that is performing impersonation.</param>
        internal ImpersonationHandle( ServerContext server )
        {
            this.server = server;
            this.disposed = false;
        }

        ~ImpersonationHandle()
        {
            Dispose( false );
        }

        /// <summary>
        /// Reverts the impersonation.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if( disposing && this.disposed == false && this.server != null && this.server.Disposed == false )
            {
                this.server.RevertImpersonate();
            }
        }
    }
}