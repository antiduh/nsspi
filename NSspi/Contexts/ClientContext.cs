using System;
using NSspi.Buffers;
using NSspi.Credentials;

namespace NSspi.Contexts
{
    /// <summary>
    /// Represents a client security context. Provides the means to establish a shared security context
    /// with the server and to encrypt, decrypt, sign and verify messages to and from the server.
    /// </summary>
    /// <remarks>
    /// A client and server establish a shared security context by exchanging authentication tokens. Once
    /// the shared context is established, the client and server can pass messages to each other, encrypted,
    /// signed, etc, using the established parameters of the shared context.
    /// </remarks>
    public class ClientContext : Context
    {
        private ContextAttrib requestedAttribs;
        private ContextAttrib finalAttribs;
        private string serverPrinc;

        /// <summary>
        /// Initializes a new instance of the ClientContext class. The context is not fully initialized and usable
        /// until the authentication cycle has been completed.
        /// </summary>
        /// <param name="cred">The security credential to authenticate as.</param>
        /// <param name="serverPrinc">The principle name of the server to connect to, or null for any.</param>
        /// <param name="requestedAttribs">Requested attributes that describe the desired properties of the
        /// context once it is established. If a context cannot be established that satisfies the indicated
        /// properties, the context initialization is aborted.</param>
        public ClientContext( Credential cred, string serverPrinc, ContextAttrib requestedAttribs )
            : base( cred )
        {
            this.serverPrinc = serverPrinc;
            this.requestedAttribs = requestedAttribs;
        }

        /// <summary>
        /// Performs and continues the authentication cycle.
        /// </summary>
        /// <remarks>
        /// This method is performed iteratively to start, continue, and end the authentication cycle with the
        /// server. Each stage works by acquiring a token from one side, presenting it to the other side
        /// which in turn may generate a new token.
        ///
        /// The cycle typically starts and ends with the client. On the first invocation on the client,
        /// no server token exists, and null is provided in its place. The client returns its status, providing
        /// its output token for the server. The server accepts the clients token as input and provides a
        /// token as output to send back to the client. This cycle continues until the server and client
        /// both indicate, typically, a SecurityStatus of 'OK'.
        /// </remarks>
        /// <param name="serverToken">The most recently received token from the server, or null if beginning
        /// the authentication cycle.</param>
        /// <param name="outToken">The clients next authentication token in the authentication cycle.</param>
        /// <returns>A status message indicating the progression of the authentication cycle.
        /// A status of 'OK' indicates that the cycle is complete, from the client's perspective. If the outToken
        /// is not null, it must be sent to the server.
        /// A status of 'Continue' indicates that the output token should be sent to the server and
        /// a response should be anticipated.</returns>
        public SecurityStatus Init( byte[] serverToken, out byte[] outToken )
        {
            TimeStamp rawExpiry = new TimeStamp();

            SecurityStatus status;

            SecureBuffer outTokenBuffer;
            SecureBufferAdapter outAdapter;

            SecureBuffer serverBuffer;
            SecureBufferAdapter serverAdapter;

            if( this.Disposed )
            {
                throw new ObjectDisposedException( "ClientContext" );
            }
            else if( ( serverToken != null ) && ( this.ContextHandle.IsInvalid ) )
            {
                throw new InvalidOperationException( "Out-of-order usage detected - have a server token, but no previous client token had been created." );
            }
            else if( ( serverToken == null ) && ( this.ContextHandle.IsInvalid == false ) )
            {
                throw new InvalidOperationException( "Must provide the server's response when continuing the init process." );
            }

            // The security package tells us how big its biggest token will be. We'll allocate a buffer
            // that size, and it'll tell us how much it used.
            outTokenBuffer = new SecureBuffer(
                new byte[this.Credential.PackageInfo.MaxTokenLength],
                BufferType.Token
            );

            serverBuffer = null;
            if( serverToken != null )
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
            using( outAdapter = new SecureBufferAdapter( outTokenBuffer ) )
            {
                if( this.ContextHandle.IsInvalid )
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
                        ref rawExpiry
                    );
                }
                else
                {
                    using( serverAdapter = new SecureBufferAdapter( serverBuffer ) )
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
                            ref rawExpiry
                        );
                    }
                }

                if (status.IsError() == false)
                {
                    if (status == SecurityStatus.OK)
                    {
                        base.Initialize(rawExpiry.ToDateTime());
                    }

                    outToken = null;

                    if (outTokenBuffer.Length != 0)
                    {
                        outToken = new byte[outTokenBuffer.Length];
                        Array.Copy(outTokenBuffer.Buffer, outToken, outToken.Length);
                    }

                    if (status == SecurityStatus.CompleteNeeded || status == SecurityStatus.CompAndContinue)
                    {
                        ContextNativeMethods.CompleteAuthToken(ref this.ContextHandle.rawHandle, outAdapter.Handle);
                        status = (status == SecurityStatus.CompleteNeeded) ? SecurityStatus.OK : SecurityStatus.ContinueNeeded;
                    }
                }
                else
                {
                    throw new SSPIException("Failed to invoke InitializeSecurityContext for a client", status);
                }
            }

            return status;
        }
    }
}