﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using NSspi.Buffers;
using NSspi.Credentials;

namespace NSspi.Contexts
{
    /// <summary>
    /// Represents a security context and provides common functionality required for all security
    /// contexts.
    /// </summary>
    /// <remarks>
    /// This class is abstract and has a protected constructor and Initialize method. The exact
    /// initialization implementation is provided by a subclasses, which may perform initialization
    /// in a variety of manners.
    /// </remarks>
    public abstract class Context : IDisposable
    {
        /// <summary>
        /// Performs basic initialization of a new instance of the Context class.
        /// Initialization is not complete until the ContextHandle property has been set
        /// and the Initialize method has been called.
        /// </summary>
        /// <param name="cred"></param>
        protected Context( Credential cred )
        {
            this.Credential = cred;

            this.ContextHandle = new SafeContextHandle();

            this.Disposed = false;
            this.Initialized = false;
        }

        /// <summary>
        /// Whether or not the context is fully formed.
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        /// The credential being used by the context to authenticate itself to other actors.
        /// </summary>
        protected Credential Credential { get; private set; }

        /// <summary>
        /// A reference to the security context's handle.
        /// </summary>
        public SafeContextHandle ContextHandle { get; private set; }

        /// <summary>
        /// The name of the authenticating authority for the context.
        /// </summary>
        public string AuthorityName
        {
            get
            {
                CheckLifecycle();
                return QueryContextString( ContextQueryAttrib.Authority );
            }
        }

        /// <summary>
        /// The logon username that the context represents.
        /// </summary>
        public string ContextUserName
        {
            get
            {
                CheckLifecycle();
                return QueryContextString( ContextQueryAttrib.Names );
            }
        }

        /// <summary>
        /// The UTC time when the context expires.
        /// </summary>
        public DateTime Expiry { get; private set; }

        /// <summary>
        /// Whether the context has been disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Marks the context as having completed the initialization process, ie, exchanging of authentication tokens.
        /// </summary>
        /// <param name="expiry">The date and time that the context will expire.</param>
        protected void Initialize( DateTime expiry )
        {
            this.Expiry = expiry;
            this.Initialized = true;
        }

        /// <summary>
        /// Releases all resources associated with the context.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Releases resources associated with the context.
        /// </summary>
        /// <param name="disposing">If true, release managed resources, else release only unmanaged resources.</param>
        protected virtual void Dispose( bool disposing )
        {
            if( this.Disposed ) { return; }

            if( disposing )
            {
                this.ContextHandle.Dispose();
            }

            this.Disposed = true;
        }

        /// <summary>
        /// Returns the identity of the remote entity.
        /// </summary>
        /// <returns></returns>
        public IIdentity GetRemoteIdentity()
        {
            IIdentity result = null;

            using( var tokenHandle = GetContextToken() )
            {
                bool gotRef = false;

                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    tokenHandle.DangerousAddRef( ref gotRef );
                }
                catch( Exception )
                {
                    if( gotRef )
                    {
                        tokenHandle.DangerousRelease();
                        gotRef = false;
                    }

                    throw;
                }
                finally
                {
                    try
                    {
                        result = new WindowsIdentity(
                            tokenHandle.DangerousGetHandle(),
                            this.Credential.SecurityPackage
                        );
                    }
                    finally
                    {
                        // Make sure we release the handle, even if the allocation for
                        // WindowsIdentity fails.
                        tokenHandle.DangerousRelease();
                    }
                }
            }

            return result;
        }

        private SafeTokenHandle GetContextToken()
        {
            bool gotRef = false;
            SecurityStatus status = SecurityStatus.InternalError;
            SafeTokenHandle token;

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
                    try
                    {
                        status = ContextNativeMethods.QuerySecurityContextToken(
                            ref this.ContextHandle.rawHandle,
                            out token
                        );
                    }
                    finally
                    {
                        this.ContextHandle.DangerousRelease();
                    }
                }
                else
                {
                    token = null;
                }
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to query context token.", status );
            }

            return token;
        }

        /// <summary>
        /// Encrypts the byte array using the context's session key.
        /// </summary>
        /// <remarks>
        /// If <paramref name="stream"/> is <c>false</c>, this is equivalent to calling <see cref="Encrypt(byte[])"/> and
        /// the returned array contains encoded versions of the trailer, message and padding buffers.
        /// 
        /// If <paramref name="stream"/> is <c>true</c> the lengths will be ommitted and the returned array contains only:
        ///  - The trailer buffer
        ///  - The message buffer
        ///  - The padding buffer.
        /// </remarks>
        /// <param name="input">The raw message to encrypt.</param>
        /// <param name="stream">Indicates if the returned data should contain only the encrypted data (<c>true</c>) or also an encoded version of the length of each buffer (<c>false</c>)</param>
        /// <returns>The packed and encrypted message.</returns>
        public byte[] Encrypt( byte[] input, bool stream )
        {
            // The message is encrypted in place in the buffer we provide to Win32 EncryptMessage
            SecPkgContext_Sizes sizes;

            SecureBuffer trailerBuffer;
            SecureBuffer dataBuffer;
            SecureBuffer paddingBuffer;
            SecureBufferAdapter adapter;

            SecurityStatus status = SecurityStatus.InvalidHandle;
            byte[] result;

            CheckLifecycle();

            sizes = QueryBufferSizes();

            trailerBuffer = new SecureBuffer( new byte[sizes.SecurityTrailer], BufferType.Token );
            dataBuffer = new SecureBuffer( new byte[input.Length], BufferType.Data );
            paddingBuffer = new SecureBuffer( new byte[sizes.BlockSize], BufferType.Padding );

            Array.Copy( input, dataBuffer.Buffer, input.Length );

            using( adapter = new SecureBufferAdapter( new[] { trailerBuffer, dataBuffer, paddingBuffer } ) )
            {
                status = ContextNativeMethods.SafeEncryptMessage(
                    this.ContextHandle,
                    0,
                    adapter,
                    0
                );
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to encrypt message", status );
            }

            int position = 0;

            // Enough room to fit:
            //  -- 2 bytes for the trailer buffer size
            //  -- 4 bytes for the message size
            //  -- 2 bytes for the padding size.
            //  -- The encrypted message
            result = new byte[(stream ? 0 : (2 + 4 + 2)) + trailerBuffer.Length + dataBuffer.Length + paddingBuffer.Length];

            if( !stream )
            { 
                ByteWriter.WriteInt16_BE( (short)trailerBuffer.Length, result, position );
                position += 2;

                ByteWriter.WriteInt32_BE( dataBuffer.Length, result, position );
                position += 4;

                ByteWriter.WriteInt16_BE( (short)paddingBuffer.Length, result, position );
                position += 2;
            }

            Array.Copy( trailerBuffer.Buffer, 0, result, position, trailerBuffer.Length );
            position += trailerBuffer.Length;

            Array.Copy( dataBuffer.Buffer, 0, result, position, dataBuffer.Length );
            position += dataBuffer.Length;

            Array.Copy( paddingBuffer.Buffer, 0, result, position, paddingBuffer.Length );
            position += paddingBuffer.Length;

            return result;
        }

        /// <summary>
        /// Encrypts the byte array using the context's session key.
        /// </summary>
        /// <remarks>
        /// The structure of the returned data is as follows:
        ///  - 2 bytes, an unsigned big-endian integer indicating the length of the trailer buffer size
        ///  - 4 bytes, an unsigned big-endian integer indicating the length of the message buffer size.
        ///  - 2 bytes, an unsigned big-endian integer indicating the length of the encryption padding buffer size.
        ///  - The trailer buffer
        ///  - The message buffer
        ///  - The padding buffer.
        /// </remarks>
        /// <param name="input">The raw message to encrypt.</param>
        /// <returns>The packed and encrypted message.</returns>
        public byte[] Encrypt( byte[] input )
        {
            return Encrypt( input, false );
        }

        /// <summary>
        /// Decrypts a previously encrypted message.
        /// </summary>
        /// <remarks>
        /// If <paramref name="stream"/> is <c>false</c>, this is equivalent to calling <see cref="Decrypt(byte[])"/> and
        /// the <paramref name="input"/> must contain encoded versions of the trailer, message and padding buffers.
        /// 
        /// If <paramref name="stream"/> is <c>true</c> the lengths should be ommitted and the <paramref name="input"/>
        /// should contain only:
        ///  - The trailer buffer
        ///  - The message buffer
        ///  - The padding buffer.
        /// </remarks>
        /// <param name="input">The packed and encrypted data.</param>
        /// <param name="stream">Indicates if the data contains only the encrypted data (<c>true</c>) or also contains an encoded version of the length of each buffer (<c>false</c>)</param>
        /// <returns>The original plaintext message.</returns>
        public byte[] Decrypt( byte[] input, bool stream )
        {
            if( !stream )
            { 
                return Decrypt( input );
            }

            byte[] inputCopy = new byte[input.Length];
            Array.Copy( input, 0, inputCopy, 0, input.Length );

            SecureBuffer inputBuffer;
            SecureBuffer outputBuffer;
            SecureBufferAdapter adapter;
            SecurityStatus status;

            inputBuffer = new SecureBuffer( inputCopy, BufferType.Stream );
            outputBuffer = new SecureBuffer( null, BufferType.Data );

            using( adapter = new SecureBufferAdapter( new[] { inputBuffer, outputBuffer } ) )
            {
                status = ContextNativeMethods.SafeDecryptMessage(
                    this.ContextHandle,
                    0,
                    adapter,
                    0
                );

                return adapter.ExtractData(1);
            }
        }

        /// <summary>
        /// Decrypts a previously encrypted message.
        /// </summary>
        /// <remarks>
        /// The expected format of the buffer is as follows:
        ///  - 2 bytes, an unsigned big-endian integer indicating the length of the trailer buffer size
        ///  - 4 bytes, an unsigned big-endian integer indicating the length of the message buffer size.
        ///  - 2 bytes, an unsigned big-endian integer indicating the length of the encryption padding buffer size.
        ///  - The trailer buffer
        ///  - The message buffer
        ///  - The padding buffer.
        /// </remarks>
        /// <param name="input">The packed and encrypted data.</param>
        /// <returns>The original plaintext message.</returns>
        public byte[] Decrypt( byte[] input )
        {
            SecPkgContext_Sizes sizes;

            SecureBuffer trailerBuffer;
            SecureBuffer dataBuffer;
            SecureBuffer paddingBuffer;
            SecureBufferAdapter adapter;

            SecurityStatus status;
            byte[] result = null;
            int remaining;
            int position;

            int trailerLength;
            int dataLength;
            int paddingLength;

            CheckLifecycle();

            sizes = QueryBufferSizes();

            // This check is required, but not sufficient. We could be stricter.
            if( input.Length < 2 + 4 + 2 + sizes.SecurityTrailer )
            {
                throw new ArgumentException( "Buffer is too small to possibly contain an encrypted message" );
            }

            position = 0;

            trailerLength = ByteWriter.ReadInt16_BE( input, position );
            position += 2;

            dataLength = ByteWriter.ReadInt32_BE( input, position );
            position += 4;

            paddingLength = ByteWriter.ReadInt16_BE( input, position );
            position += 2;

            if( trailerLength + dataLength + paddingLength + 2 + 4 + 2 > input.Length )
            {
                throw new ArgumentException( "The buffer contains invalid data - the embedded length data does not add up." );
            }

            trailerBuffer = new SecureBuffer( new byte[trailerLength], BufferType.Token );
            dataBuffer = new SecureBuffer( new byte[dataLength], BufferType.Data );
            paddingBuffer = new SecureBuffer( new byte[paddingLength], BufferType.Padding );

            remaining = input.Length - position;

            if( trailerBuffer.Length <= remaining )
            {
                Array.Copy( input, position, trailerBuffer.Buffer, 0, trailerBuffer.Length );
                position += trailerBuffer.Length;
                remaining -= trailerBuffer.Length;
            }
            else
            {
                throw new ArgumentException( "Input is missing data - it is not long enough to contain a fully encrypted message" );
            }

            if( dataBuffer.Length <= remaining )
            {
                Array.Copy( input, position, dataBuffer.Buffer, 0, dataBuffer.Length );
                position += dataBuffer.Length;
                remaining -= dataBuffer.Length;
            }
            else
            {
                throw new ArgumentException( "Input is missing data - it is not long enough to contain a fully encrypted message" );
            }

            if( paddingBuffer.Length <= remaining )
            {
                Array.Copy( input, position, paddingBuffer.Buffer, 0, paddingBuffer.Length );
            }
            // else there was no padding.

            using( adapter = new SecureBufferAdapter( new[] { trailerBuffer, dataBuffer, paddingBuffer } ) )
            {
                status = ContextNativeMethods.SafeDecryptMessage(
                    this.ContextHandle,
                    0,
                    adapter,
                    0
                );
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to encrypt message", status );
            }

            result = new byte[dataBuffer.Length];
            Array.Copy( dataBuffer.Buffer, 0, result, 0, dataBuffer.Length );

            return result;
        }

        /// <summary>
        /// Signs the message using the context's session key.
        /// </summary>
        /// <remarks>
        /// The structure of the returned buffer is as follows:
        ///  - 4 bytes, unsigned big-endian integer indicating the length of the plaintext message
        ///  - 2 bytes, unsigned big-endian integer indicating the length of the signture
        ///  - The plaintext message
        ///  - The message's signature.
        /// </remarks>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] MakeSignature( byte[] message )
        {
            SecurityStatus status = SecurityStatus.InternalError;

            SecPkgContext_Sizes sizes;
            SecureBuffer dataBuffer;
            SecureBuffer signatureBuffer;
            SecureBufferAdapter adapter;

            CheckLifecycle();

            sizes = QueryBufferSizes();

            dataBuffer = new SecureBuffer( new byte[message.Length], BufferType.Data );
            signatureBuffer = new SecureBuffer( new byte[sizes.MaxSignature], BufferType.Token );

            Array.Copy( message, dataBuffer.Buffer, message.Length );

            using( adapter = new SecureBufferAdapter( new[] { dataBuffer, signatureBuffer } ) )
            {
                status = ContextNativeMethods.SafeMakeSignature(
                    this.ContextHandle,
                    0,
                    adapter,
                    0
                );
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to create message signature.", status );
            }

            byte[] outMessage;
            int position = 0;

            // Enough room for
            //  - original message length (4 bytes)
            //  - signature length        (2 bytes)
            //  - original message
            //  - signature

            outMessage = new byte[4 + 2 + dataBuffer.Length + signatureBuffer.Length];

            ByteWriter.WriteInt32_BE( dataBuffer.Length, outMessage, position );
            position += 4;

            ByteWriter.WriteInt16_BE( (Int16)signatureBuffer.Length, outMessage, position );
            position += 2;

            Array.Copy( dataBuffer.Buffer, 0, outMessage, position, dataBuffer.Length );
            position += dataBuffer.Length;

            Array.Copy( signatureBuffer.Buffer, 0, outMessage, position, signatureBuffer.Length );
            position += signatureBuffer.Length;

            return outMessage;
        }

        /// <summary>
        /// Returns the Session Key from a context or null on failure.
        /// </summary>
        /// <remarks>
        /// Session keys are sometimes needed for other purposes or HMAC functions. This function
        /// will run QueryAttribute to get the session key struct, and read and return the key from
        /// that struct.
        /// </remarks>
        /// <returns>byte[] with the session key data or null on failure</returns>
        public byte[] QuerySessionKey()
        {
            SecurityStatus status;

            byte[] SessionKey = null;

            status = ContextNativeMethods.SafeQueryContextAttribute(
                this.ContextHandle,
                ContextQueryAttrib.SessionKey,
                ref SessionKey
            );

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to query session key.", status );
            }

            return SessionKey;
        }

        /// <summary>
        /// Verifies the signature of a signed message
        /// </summary>
        /// <remarks>
        /// The expected structure of the signed message buffer is as follows:
        ///  - 4 bytes, unsigned integer in big endian format indicating the length of the plaintext message
        ///  - 2 bytes, unsigned integer in big endian format indicating the length of the signture
        ///  - The plaintext message
        ///  - The message's signature.
        /// </remarks>
        /// <param name="signedMessage">The packed signed message.</param>
        /// <param name="origMessage">The extracted original message.</param>
        /// <returns>True if the message has a valid signature, false otherwise.</returns>
        public bool VerifySignature( byte[] signedMessage, out byte[] origMessage )
        {
            SecurityStatus status = SecurityStatus.InternalError;

            SecPkgContext_Sizes sizes;
            SecureBuffer dataBuffer;
            SecureBuffer signatureBuffer;
            SecureBufferAdapter adapter;

            CheckLifecycle();

            sizes = QueryBufferSizes();

            if( signedMessage.Length < 2 + 4 + sizes.MaxSignature )
            {
                throw new ArgumentException( "Input message is too small to possibly fit a valid message" );
            }

            int position = 0;
            int messageLen;
            int sigLen;

            messageLen = ByteWriter.ReadInt32_BE( signedMessage, 0 );
            position += 4;

            sigLen = ByteWriter.ReadInt16_BE( signedMessage, position );
            position += 2;

            if( messageLen + sigLen + 2 + 4 > signedMessage.Length )
            {
                throw new ArgumentException( "The buffer contains invalid data - the embedded length data does not add up." );
            }

            dataBuffer = new SecureBuffer( new byte[messageLen], BufferType.Data );
            Array.Copy( signedMessage, position, dataBuffer.Buffer, 0, messageLen );
            position += messageLen;

            signatureBuffer = new SecureBuffer( new byte[sigLen], BufferType.Token );
            Array.Copy( signedMessage, position, signatureBuffer.Buffer, 0, sigLen );
            position += sigLen;

            using( adapter = new SecureBufferAdapter( new[] { dataBuffer, signatureBuffer } ) )
            {
                status = ContextNativeMethods.SafeVerifySignature(
                    this.ContextHandle,
                    0,
                    adapter,
                    0
                );
            }

            if( status == SecurityStatus.OK )
            {
                origMessage = dataBuffer.Buffer;
                return true;
            }
            else if( status == SecurityStatus.MessageAltered ||
                      status == SecurityStatus.OutOfSequence )
            {
                origMessage = null;
                return false;
            }
            else
            {
                throw new SSPIException( "Failed to determine the veracity of a signed message.", status );
            }
        }

        /// <summary>
        /// Queries the security package's expections regarding message/token/signature/padding buffer sizes.
        /// </summary>
        /// <returns></returns>
        private SecPkgContext_Sizes QueryBufferSizes()
        {
            SecPkgContext_Sizes sizes = new SecPkgContext_Sizes();
            SecurityStatus status = SecurityStatus.InternalError;
            bool gotRef = false;

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
                    status = ContextNativeMethods.QueryContextAttributes_Sizes(
                        ref this.ContextHandle.rawHandle,
                        ContextQueryAttrib.Sizes,
                        ref sizes
                    );
                    this.ContextHandle.DangerousRelease();
                }
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to query context buffer size attributes", status );
            }

            return sizes;
        }

        /// <summary>
        /// Queries a string-valued context attribute by the named attribute.
        /// </summary>
        /// <param name="attrib">The string-valued attribute to query.</param>
        /// <returns></returns>
        private string QueryContextString( ContextQueryAttrib attrib )
        {
            SecPkgContext_String stringAttrib;
            SecurityStatus status = SecurityStatus.InternalError;
            string result = null;
            bool gotRef = false;

            if( attrib != ContextQueryAttrib.Names && attrib != ContextQueryAttrib.Authority )
            {
                throw new InvalidOperationException( "QueryContextString can only be used to query context Name and Authority attributes" );
            }

            stringAttrib = new SecPkgContext_String();

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
                    status = ContextNativeMethods.QueryContextAttributes_String(
                        ref this.ContextHandle.rawHandle,
                        attrib,
                        ref stringAttrib
                    );

                    this.ContextHandle.DangerousRelease();

                    if( status == SecurityStatus.OK )
                    {
                        result = Marshal.PtrToStringUni( stringAttrib.StringResult );
                        ContextNativeMethods.FreeContextBuffer( stringAttrib.StringResult );
                    }
                }
            }

            if( status == SecurityStatus.Unsupported )
            {
                return null;
            }
            else if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to query the context's associated user name", status );
            }

            return result;
        }

        /// <summary>
        /// Verifies that the object's lifecycle (initialization / disposition) state is suitable for using the
        /// object.
        /// </summary>
        private void CheckLifecycle()
        {
            if( this.Initialized == false )
            {
                throw new InvalidOperationException( "The context is not yet fully formed." );
            }
            else if( this.Disposed )
            {
                throw new ObjectDisposedException( "Context" );
            }
        }
    }
}