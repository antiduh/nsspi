using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public class ClientContext : Context
    {
        public ClientContext( ClientCredential cred, string serverPrinc, ContextReqAttrib attribs )
            : base( cred )
        {
            long credHandle = base.Credential.CredentialHandle;

            long prevContextHandle = 0;
            long newContextHandle = 0;

            long expiry = 0;
            int newContextAttribs = 0;

            SecurityStatus status;
            
            
            status = NativeMethods.InitializeSecurityContext_Client(
                ref credHandle,
                ref prevContextHandle,
                serverPrinc,
                0,
                0,
                0,
                IntPtr.Zero,
                0,
                ref newContextHandle,
                IntPtr.Zero,
                ref newContextAttribs,
                ref expiry
            );

            
        }
    }
}
