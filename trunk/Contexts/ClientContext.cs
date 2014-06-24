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
            long expiry = 0;

            SecurityStatus status;

            SecureBuffer outTokenBuffer;
            SecureBufferAdapter outAdapter;

            SecureBuffer serverBuffer;
            SecureBufferAdapter serverAdapter;

            if ( (serverToken != null) && ( this.ContextHandle.IsInvalid ) )
            {
                throw new InvalidOperationException( "Out-of-order usage detected - have a server token, but no previous client token had been created." );
            }
            else if ( (serverToken == null) && ( this.ContextHandle.IsInvalid == false ) )
            {
                throw new InvalidOperationException( "Must provide the server's response when continuing the init process." );
            }

            outTokenBuffer = new SecureBuffer( new byte[12288], BufferType.Token );

            serverBuffer = null;
            if ( serverToken != null )
            {
                serverBuffer = new SecureBuffer( serverToken, BufferType.Token );
            }

            // Some notes on handles and invoking InitializeSecurityContext
            //  - The first time around, the phContext parameter (the 'old' handle) is a null pointer to what 
            //    would be an RawSspiHandle, to indicate this is the first time it's being called. 
            //    The phNewContext is a pointer (reference) to an RawSspiHandle struct of where to write the 
            //    new handle's values.
            //  - The next time you invoke ISC, it takes a pointer to the handle it gave you last time in phContext,
            //    and takes a pointer to where it should write the new handle's values in phNewContext.
            //  - After the first time, you can provide the same handle to both parameters. From MSDN:
            //       "On the second call, phNewContext can be the same as the handle specified in the phContext
            //        parameter."
            //    It will overwrite the handle you gave it with the new handle value.
            //  - All handle structures themselves are actually *two* pointer variables, eg, 64 bits on 32-bit 
            //    Windows, 128 bits on 64-bit Windows.
            //  - So in the end, on a 64-bit machine, we're passing a 64-bit value (the pointer to the struct) that
            //    points to 128 bits of memory (the struct itself) for where to write the handle numbers.
            using ( outAdapter = new SecureBufferAdapter( outTokenBuffer ) )
            {
                if ( this.ContextHandle.IsInvalid )
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
                        ref this.ContextHandle.rawHandle,
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
                            ref this.ContextHandle.rawHandle,
                            this.serverPrinc,
                            this.requestedAttribs,
                            0,
                            SecureBufferDataRep.Network,
                            serverAdapter.Handle,
                            0,
                            ref this.ContextHandle.rawHandle,
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

            return status;
        }
    }
}
