using System;
using System.Security.Principal;
using System.Threading;

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

        /// <summary>
        /// Finalizes the ImpersonationHandle by reverting the impersonation.
        /// </summary>
        ~ImpersonationHandle()
        {
            Dispose( false );
        }

        /// <summary>
        /// Reverts impersonation.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Reverts impersonation.
        /// </summary>
        /// <param name="disposing">True if being disposed, false if being finalized.</param>
        private void Dispose( bool disposing )
        {
            // This implements a variant of the typical dispose pattern. Always try to revert
            // impersonation, even if finalizing. Don't do anything if we're already reverted.

            if( this.disposed == false )
            {
                this.disposed = true;

                // Just in case the reference is being pulled out from under us, pull a stable copy
                // of the reference while we're null-checking.
                var serverCopy = this.server;

                if( serverCopy != null && serverCopy.Disposed == false )
                {
                    serverCopy.RevertImpersonate();
                }
            }
        }
    }
}