using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NSspi.Buffers;
using NSspi.Contexts;
using NSspi.Credentials;

namespace NSspi.Contexts
{
    public class Context : IDisposable
    {
        private bool disposed;

        public Context( Credential cred )
        {
            this.Credential = cred;

            this.ContextHandle = new SafeContextHandle();

            this.disposed = false;
            this.Initialized = false;
        }

        ~Context()
        {
            Dispose( false );
        }
        
        /// <summary>
        /// Whether or not the context is fully formed.
        /// </summary>
        public bool Initialized { get; protected set; }

        protected Credential Credential { get; private set; }

        public SafeContextHandle ContextHandle { get; protected set; }

        public string AuthorityName
        {
            get
            {
                return QueryContextString( ContextQueryAttrib.Authority );
            }
        }

        public string ContextUserName
        {
            get
            {
                return QueryContextString( ContextQueryAttrib.Names );
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if( this.disposed ) { return; }

            if( disposing )
            {
                this.Credential.Dispose();
                this.ContextHandle.Dispose();
            }


            this.disposed = true;
        }

        /// <summary>
        /// Encrypts the byte array using the context's session key. The encrypted data is stored in a new
        /// byte array, which is formatted such that the first four bytes are the original message length 
        /// as an unsigned integer and the remaining bytes are the encrypted bytes of the original message.
        /// </summary>
        /// <remarks>
        /// The resulting byte array stores the SSPI buffer data in the following buffer format:
        ///  - Token
        ///  - Data
        ///  - Padding
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Encrypt( byte[] input )
        {
            // The message is encrypted in place in the buffer we provide to Win32 EncryptMessage
            SecPkgContext_Sizes sizes;

            SecureBuffer trailerBuffer;
            SecureBuffer dataBuffer;
            SecureBuffer paddingBuffer;
            SecureBufferAdapter adapter;

            SecurityStatus status = SecurityStatus.InvalidHandle;
            byte[] result;

            if ( this.Initialized == false )
            {
                throw new InvalidOperationException( "The context is not fully formed." );
            }

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
            result = new byte[2 + 4 + 2 + trailerBuffer.Length + dataBuffer.Length + paddingBuffer.Length];

            ByteWriter.WriteInt16_BE( (short)trailerBuffer.Length, result, position );
            position += 2;

            ByteWriter.WriteInt32_BE( dataBuffer.Length, result, position );
            position += 4;

            ByteWriter.WriteInt16_BE( (short)paddingBuffer.Length, result, position );
            position += 2;

            Array.Copy( trailerBuffer.Buffer, 0, result, position, trailerBuffer.Length );
            position += trailerBuffer.Length;

            Array.Copy( dataBuffer.Buffer, 0, result, position, dataBuffer.Length );
            position += dataBuffer.Length;

            Array.Copy( paddingBuffer.Buffer, 0, result, position, paddingBuffer.Length );
            position += paddingBuffer.Length;
                        
            return result;
        }

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

            if ( this.Initialized == false )
            {
                throw new InvalidOperationException( "The context is not fully formed." );
            }

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
            

            using( adapter = new SecureBufferAdapter( new [] { trailerBuffer, dataBuffer, paddingBuffer } ) )
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
            catch ( Exception )
            {
                if ( gotRef )
                {
                    this.ContextHandle.DangerousRelease();
                    gotRef = false;
                }

                throw;
            }
            finally
            {
                if ( gotRef )
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

        private string QueryContextString(ContextQueryAttrib attrib)
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
            catch ( Exception )
            {
                if ( gotRef )
                {
                    this.ContextHandle.DangerousRelease();
                    gotRef = false;
                }
                throw;
            }
            finally
            {
                if ( gotRef )
                {
                    status = ContextNativeMethods.QueryContextAttributes_String(
                        ref this.ContextHandle.rawHandle,
                        attrib,
                        ref stringAttrib
                    );

                    this.ContextHandle.DangerousRelease();

                    if ( status == SecurityStatus.OK )
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
        
    }
}
