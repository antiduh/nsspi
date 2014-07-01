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
    public class ServerContext : Context
    {
        private ContextAttrib requestedAttribs;
        private ContextAttrib finalAttribs;

        private bool impersonating;

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
            outBuffer = new SecureBuffer( new byte[12288], BufferType.Token );

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

        public ImpersonationHandle ImpersonateClient()
        {
            ImpersonationHandle handle;
            SecurityStatus status = SecurityStatus.InternalError;
            bool gotRef = false;

            if( this.Disposed )
            {
                throw new ObjectDisposedException( "ServerContext" );
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

        internal void RevertImpersonate()
        {
            bool gotRef = false;
            SecurityStatus status = SecurityStatus.InternalError;

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
                    status = ContextNativeMethods.RevertSecurityContext(
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
