using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NSspi.Buffers;
using NSspi.Credentials;

namespace NSspi.Contexts
{
    /// <summary>
    /// Represents a security context used in a server role.
    /// </summary>
    public class ServerContext : Context
    {
        private ContextAttrib requestedAttribs;
        private ContextAttrib finalAttribs;

        private bool impersonating;

        /// <summary>
        /// Performs basic initialization of a new instance of the ServerContext class. The ServerContext
        /// is not ready for message manipulation until a security context has been established with a client.
        /// </summary>
        /// <param name="cred"></param>
        /// <param name="requestedAttribs"></param>
        public ServerContext(ServerCredential cred, ContextAttrib requestedAttribs) : base ( cred )
        {
            this.requestedAttribs = requestedAttribs;
            this.finalAttribs = ContextAttrib.Zero;

            this.impersonating = false;

            this.SupportsImpersonate = this.Credential.PackageInfo.Capabilities.HasFlag( SecPkgCapability.Impersonation );
        }

        /// <summary>
        /// Whether or not the server can impersonate an authenticated client.
        /// </summary>
        /// <remarks>
        /// This depends on the security package that was used to create the server and client's credentials.
        /// </remarks>
        public bool SupportsImpersonate { get; private set; }

        /// <summary>
        /// Performs and continues the authentication cycle.
        /// </summary>
        /// <remarks>
        /// This method is performed iteratively to continue and end the authentication cycle with the
        /// client. Each stage works by acquiring a token from one side, presenting it to the other side
        /// which in turn may generate a new token.
        /// 
        /// The cycle typically starts and ends with the client. On the first invocation on the client,
        /// no server token exists, and null is provided in its place. The client returns its status, providing
        /// its output token for the server. The server accepts the clients token as input and provides a 
        /// token as output to send back to the client. This cycle continues until the server and client 
        /// both indicate, typically, a SecurityStatus of 'OK'.
        /// </remarks>
        /// <param name="clientToken">The most recently received token from the client.</param>
        /// <param name="nextToken">The servers next authentication token in the cycle, that must
        /// be sent to the client.</param>
        /// <returns>A status message indicating the progression of the authentication cycle.
        /// A status of 'OK' indicates that the cycle is complete, from the servers's perspective. If the nextToken
        /// is not null, it must be sent to the client.
        /// A status of 'Continue' indicates that the output token should be sent to the client and 
        /// a response should be anticipated.</returns>
        public SecurityStatus AcceptToken( byte[] clientToken, out byte[] nextToken )
        {
            SecureBuffer clientBuffer;
            SecureBuffer outBuffer;

            SecurityStatus status;
            TimeStamp rawExpiry = new TimeStamp();

            SecureBufferAdapter clientAdapter;
            SecureBufferAdapter outAdapter;

            if( this.Disposed )
            {
                throw new ObjectDisposedException( "ServerContext" );
            }
            else if( this.Initialized )
            {
                throw new InvalidOperationException( 
                    "Attempted to continue initialization of a ServerContext after initialization had completed."
                );
            }

            clientBuffer = new SecureBuffer( clientToken, BufferType.Token );

            outBuffer = new SecureBuffer( 
                new byte[ this.Credential.PackageInfo.MaxTokenLength ], 
                BufferType.Token 
            );

            using ( clientAdapter = new SecureBufferAdapter( clientBuffer ) )
            {
                using ( outAdapter = new SecureBufferAdapter( outBuffer ) )
                {
                    if( this.ContextHandle.IsInvalid )
                    {
                        status = ContextNativeMethods.AcceptSecurityContext_1(
                            ref this.Credential.Handle.rawHandle,
                            IntPtr.Zero,
                            clientAdapter.Handle,
                            requestedAttribs,
                            SecureBufferDataRep.Network,
                            ref this.ContextHandle.rawHandle,
                            outAdapter.Handle,
                            ref this.finalAttribs,
                            ref rawExpiry
                        );
                    }
                    else
                    {
                        status = ContextNativeMethods.AcceptSecurityContext_2(
                            ref this.Credential.Handle.rawHandle,
                            ref this.ContextHandle.rawHandle,
                            clientAdapter.Handle,
                            requestedAttribs,
                            SecureBufferDataRep.Network,
                            ref this.ContextHandle.rawHandle,
                            outAdapter.Handle,
                            ref this.finalAttribs,
                            ref rawExpiry
                        );


                    }
                }
            }

            if ( status == SecurityStatus.OK )
            {
                nextToken = null;

                base.Initialize( rawExpiry.ToDateTime() );

                if ( outBuffer.Length != 0 )
                {
                    nextToken = new byte[outBuffer.Length];
                    Array.Copy( outBuffer.Buffer, nextToken, nextToken.Length );
                }
                else
                {
                    nextToken = null;
                }
            }
            else if ( status == SecurityStatus.ContinueNeeded )
            {
                nextToken = new byte[outBuffer.Length];
                Array.Copy( outBuffer.Buffer, nextToken, nextToken.Length );
            }
            else
            {
                throw new SSPIException( "Failed to call AcceptSecurityContext", status );
            }

            return status;
        }

        /// <summary>
        /// Changes the current thread's security context to impersonate the user of the client. 
        /// </summary>
        /// <remarks>
        /// Requires that the security package provided with the server's credentials, as well as the 
        /// client's credentials, support impersonation.
        /// 
        /// Currently, only one thread may initiate impersonation per security context. Impersonation may 
        /// follow threads created by the initial impersonation thread, however.
        /// </remarks>
        /// <returns>A handle to capture the lifetime of the impersonation. Dispose the handle to revert
        /// impersonation. If the handle is leaked, the impersonation will automatically revert at a 
        /// non-deterministic time when the handle is finalized by the Garbage Collector.</returns>
        public ImpersonationHandle ImpersonateClient()
        {
            ImpersonationHandle handle;
            SecurityStatus status = SecurityStatus.InternalError;
            bool gotRef = false;

            if( this.Disposed )
            {
                throw new ObjectDisposedException( "ServerContext" );
            }
            else if( this.Initialized == false )
            {
                throw new InvalidOperationException(
                    "The server context has not been completely initialized."
                );
            }
            else if( impersonating )
            {
                throw new InvalidOperationException( "Cannot impersonate again while already impersonating." );
            }
            else if( this.SupportsImpersonate == false )
            {
                throw new InvalidOperationException(
                    "The ServerContext is using a security package that does not support impersonation."
                );
            }

            handle = new ImpersonationHandle( this );
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                this.ContextHandle.DangerousAddRef( ref gotRef );
            }
            catch( Exception )
            {
                if( gotRef )
                {
                    this.ContextHandle.DangerousRelease();
                    gotRef = false;
                }

                throw;
            }
            finally
            {
                if( gotRef )
                {
                    status = ContextNativeMethods.ImpersonateSecurityContext(
                        ref this.ContextHandle.rawHandle
                    );

                    this.ContextHandle.DangerousRelease();

                    this.impersonating = true;
                }
            }

            if( status == SecurityStatus.NoImpersonation )
            {
                throw new SSPIException( "Impersonation could not be performed.", status );
            }
            else if( status == SecurityStatus.Unsupported )
            {
                throw new SSPIException( "Impersonation is not supported by the security context's Security Support Provider.", status );
            }
            else if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to impersonate the client", status );
            }
            
            return handle;
        }

        /// <summary>
        /// Called by the ImpersonationHandle when it is released, either by disposale or finalization.
        /// </summary>
        internal void RevertImpersonate()
        {
            bool gotRef = false;

            if( impersonating == false || this.Disposed )
            {
                return;
            }

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                this.ContextHandle.DangerousAddRef( ref gotRef );
            }
            catch( Exception )
            {
                if( gotRef )
                {
                    this.ContextHandle.DangerousRelease();
                    gotRef = false;
                }

                throw;
            }
            finally
            {
                if( gotRef )
                {
                    ContextNativeMethods.RevertSecurityContext(
                        ref this.ContextHandle.rawHandle
                    );

                    this.ContextHandle.DangerousRelease();

                    this.impersonating = false;
                }
            }
        }

        protected override void Dispose( bool disposing )
        {
            // We were disposed while impersonating. This means that the consumer that is currently holding
            // the impersonation handle allowed the context to be disposed or finalized while an impersonation handle
            // was held. We have to revert impersonation to restore the thread's behavior, since once the context
            // goes away, there's nothing left.
            //
            // When and if the impersonation handle is diposed/finalized, it'll see that the context has already been
            // disposed, will assume that we already reverted, and so will do nothing.

            if( this.impersonating )
            {
                RevertImpersonate();
            }

            base.Dispose( disposing );
        }
    }
}
