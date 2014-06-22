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
        public ClientContext( ClientCredential cred, string serverPrinc, ContextAttrib attribs )
            : base( cred )
        {
            long credHandle = base.Credential.CredentialHandle;

            long prevContextHandle = 0;
            long newContextHandle = 0;

            long expiry = 0;
            ContextAttrib newContextAttribs = 0;

            SecurityStatus status;
            SecureBuffer tokenBuffer = new SecureBuffer( new byte[12288], BufferType.Token );
            SecureBufferAdapter list = new SecureBufferAdapter( tokenBuffer );

            using ( list )
            {
                status = NativeMethods.InitializeSecurityContext_Client1(
                    ref credHandle,
                    IntPtr.Zero,
                    serverPrinc,
                    attribs,
                    0,
                    SecureBufferDataRep.Network,
                    IntPtr.Zero,
                    0,
                    ref newContextHandle,
                    list.Handle,
                    ref newContextAttribs,
                    ref expiry
                );
            }
            
            Console.Out.WriteLine( "Call status: " + status );
            Console.Out.WriteLine( "Buffer length: " + tokenBuffer.Length );
            Console.Out.WriteLine( "First bytes: " + tokenBuffer.Buffer[0] );
            base.ContextHandle = newContextHandle;
            
        }
    }
}
