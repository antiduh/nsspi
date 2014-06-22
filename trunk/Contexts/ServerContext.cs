using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public class ServerContext : Context
    {
        private ContextAttrib requestedAttribs;
        private ContextAttrib finalAttribs;
        private bool complete;

        public ServerContext(ServerCredential cred, ContextAttrib requestedAttribs) : base ( cred )
        {
            this.requestedAttribs = requestedAttribs;
            this.finalAttribs = ContextAttrib.Zero;

        }

        public SecurityStatus AcceptToken( byte[] clientToken, out byte[] nextToken )
        {
            SecureBuffer clientBuffer = new SecureBuffer( clientToken, BufferType.Token );
            SecureBuffer outBuffer = new SecureBuffer( new byte[12288], BufferType.Token );

            long credHandle = this.Credential.CredentialHandle;
            long oldContextHandle = base.ContextHandle;
            long newContextHandle = 0;

            SecurityStatus status;
            ContextAttrib outAttribs = ContextAttrib.Zero;
            long expiry = 0;

            SecureBufferAdapter clientAdapter;
            SecureBufferAdapter outAdapter;


            using ( clientAdapter = new SecureBufferAdapter( clientBuffer ) )
            {
                using ( outAdapter = new SecureBufferAdapter( outBuffer ) )
                {
                    if ( oldContextHandle == 0 )
                    {
                        status = ContextNativeMethods.AcceptSecurityContext_1(
                            ref credHandle,
                            IntPtr.Zero,
                            clientAdapter.Handle,
                            requestedAttribs,
                            SecureBufferDataRep.Network,
                            ref newContextHandle,
                            outAdapter.Handle,
                            ref outAttribs,
                            ref expiry
                        );
                    }
                    else
                    {
                        status = ContextNativeMethods.AcceptSecurityContext_2(
                            ref credHandle,
                            ref oldContextHandle,
                            clientAdapter.Handle,
                            requestedAttribs,
                            SecureBufferDataRep.Network,
                            ref newContextHandle,
                            outAdapter.Handle,
                            ref outAttribs,
                            ref expiry
                        );
                    }
                }
            }

            if ( status == SecurityStatus.OK )
            {
                nextToken = null;
                this.complete = true;
            }
            else if ( status == SecurityStatus.ContinueNeeded )
            {
                this.complete = false;

                nextToken = new byte[outBuffer.Length];
                Array.Copy( outBuffer.Buffer, nextToken, nextToken.Length );
            }
            else
            {
                throw new SSPIException( "Failed to call AcceptSecurityContext", status );
            }

            base.ContextHandle = newContextHandle;

            return status;
        }
    }
}
