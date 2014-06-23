using NSspi.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NSspi
{
    public class Context : IDisposable
    {
        private bool disposed;

        public Context( Credential cred )
        {
            this.Credential = cred;

            this.disposed = false;
        }

        ~Context()
        {
            Dispose( false );
        }

        protected Credential Credential { get; private set; }

        public long ContextHandle { get; protected set; }

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
            }

            long contextHandleCopy = this.ContextHandle;
            ContextNativeMethods.DeleteSecurityContext( ref contextHandleCopy );

            this.ContextHandle = 0;

            this.disposed = true;
        }

        /// <summary>
        /// Encrypts the byte array using the context's session key. The encrypted data is stored in a new
        /// byte array, which is formatted such that the first four bytes are the original message length 
        /// as an unsigned integer and the remaining bytes are the encrypted bytes of the original message.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Encrypt( byte[] input )
        {
            // The message is encrypted in place in the buffer we provide to Win32 EncryptMessage
            return null;
        }

        internal SecPkgContext_Sizes QueryBufferSizes()
        {
            SecPkgContext_Sizes sizes = new SecPkgContext_Sizes();
            long contextHandle = this.ContextHandle;
            SecurityStatus status;

            status = ContextNativeMethods.QueryContextAttributes_Sizes(
                ref contextHandle,
                ContextQueryAttrib.Sizes,
                ref sizes 
            );

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to query context buffer size attributes", status );
            }

            return sizes;
        }

        internal string QueryContextString(ContextQueryAttrib attrib)
        {
            SecPkgContext_String stringAttrib;
            long contextHandle;
            SecurityStatus status;
            string result;

            if( attrib != ContextQueryAttrib.Names && attrib != ContextQueryAttrib.Authority )
            {
                throw new InvalidOperationException( "QueryContextString can only be used to query context Name and Authority attributes" );
            }

            stringAttrib = new SecPkgContext_String();

            contextHandle = this.ContextHandle;

            status = ContextNativeMethods.QueryContextAttributes_String(
                ref contextHandle,
                attrib,
                ref stringAttrib
            );

            
            if( status == SecurityStatus.Unsupported )
            {
                return null;
            }
            else if( status == SecurityStatus.OK )
            {
                // TODO handle this safely.
                result = Marshal.PtrToStringUni( stringAttrib.StringResult );
                ContextNativeMethods.FreeContextBuffer( stringAttrib.StringResult );
            }
            else
            {
                throw new SSPIException( "Failed to query the context's associated user name", status );
            }

            return result;
        }
        
    }
}
