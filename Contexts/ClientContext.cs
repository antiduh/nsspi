using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public class ClientContext : Context
    {
        private bool complete;
        private ContextAttrib requestedAttribs;
        private ContextAttrib finalAttribs;
        private string serverPrinc;

        public ClientContext( ClientCredential cred, string serverPrinc, ContextAttrib requestedAttribs )
            : base( cred )
        {
            this.complete = false;

            this.serverPrinc = serverPrinc;
            this.requestedAttribs = requestedAttribs;
        }

        public SecurityStatus Init( byte[] serverToken, out byte[] outToken )
        {
            long prevContextHandle = base.ContextHandle;
            long newContextHandle = 0;

            long expiry = 0;

            SecurityStatus status;

            SecureBuffer outTokenBuffer;
            SecureBufferAdapter outAdapter;

            SecureBuffer serverBuffer;
            SecureBufferAdapter serverAdapter;

            if ( (serverToken != null) && (prevContextHandle == 0) )
            {
                throw new InvalidOperationException( "Out-of-order usage detected - have a server token, but no previous client token had been created." );
            }
            else if ( (serverToken == null) && (prevContextHandle != 0) )
            {
                throw new InvalidOperationException( "Must provide the server's response when continuing the init process." );
            }

            outTokenBuffer = new SecureBuffer( new byte[12288], BufferType.Token );

            serverBuffer = null;
            if ( serverToken != null )
            {
                serverBuffer = new SecureBuffer( serverToken, BufferType.Token );
            }

            using ( outAdapter = new SecureBufferAdapter( outTokenBuffer ) )
            {
                if ( prevContextHandle == 0 )
                {
                    status = ContextNativeMethods.InitializeSecurityContext_1(
                        ref this.Credential.Handle.rawHandle,
                        IntPtr.Zero,
                        this.serverPrinc,
                        this.requestedAttribs,
                        0,
                        SecureBufferDataRep.Network,
                        IntPtr.Zero,
                        0,
                        ref newContextHandle,
                        outAdapter.Handle,
                        ref this.finalAttribs,
                        ref expiry
                    );
                }
                else
                {
                    using ( serverAdapter = new SecureBufferAdapter( serverBuffer ) )
                    {
                        status = ContextNativeMethods.InitializeSecurityContext_2(
                            ref this.Credential.Handle.rawHandle,
                            ref prevContextHandle,
                            this.serverPrinc,
                            this.requestedAttribs,
                            0,
                            SecureBufferDataRep.Network,
                            serverAdapter.Handle,
                            0,
                            ref newContextHandle,
                            outAdapter.Handle,
                            ref this.finalAttribs,
                            ref expiry
                        );
                    }
                }

            }

            if ( status == SecurityStatus.OK )
            {
                this.complete = true;
                outToken = null;
            }
            else if ( status == SecurityStatus.ContinueNeeded )
            {
                this.complete = false;

                outToken = new byte[outTokenBuffer.Length];
                Array.Copy( outTokenBuffer.Buffer, outToken, outToken.Length );
            }
            else
            {
                throw new SSPIException( "Failed to invoke InitializeSecurityContext for a client", status );
            }

            base.ContextHandle = newContextHandle;

            return status;
        }
    }
}
