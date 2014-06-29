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
        }

        public SecurityStatus AcceptToken( byte[] clientToken, out byte[] nextToken )
        {
            SecureBuffer clientBuffer = new SecureBuffer( clientToken, BufferType.Token );
            SecureBuffer outBuffer = new SecureBuffer( new byte[12288], BufferType.Token );

            SecurityStatus status;
            long rawExpiry = 0;

            SecureBufferAdapter clientAdapter;
            SecureBufferAdapter outAdapter;

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
                this.Initialized = true;

                if ( outBuffer.Length != 0 )
                {
                    nextToken = new byte[outBuffer.Length];
                    Array.Copy( outBuffer.Buffer, nextToken, nextToken.Length );
                }
                else
                {
                    nextToken = null;
                }

                this.Expiry = TimeStamp.Calc( rawExpiry );

                InitProviderCapabilities();
            }
            else if ( status == SecurityStatus.ContinueNeeded )
            {
                this.Initialized = false;

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
            ImpersonationHandle handle = new ImpersonationHandle( this );
            SecurityStatus status = SecurityStatus.InternalError;
            bool gotRef = false;

            if( impersonating )
            {
                throw new InvalidOperationException( "Cannot impersonate again while already impersonating." );
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

        private void InitProviderCapabilities()
        {
        }
    }
}
