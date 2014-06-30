using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    public class ServerCredential : Credential
    {
        public ServerCredential( string package )
            : base( package )
        {
            Init();
        }

        private void Init()
        {
            string packageName;
            CredentialUse use;
            TimeStamp rawExpiry = new TimeStamp();

            // -- Package --
            // Copy off for the call, since this.SecurityPackage is a property.
            packageName = this.SecurityPackage;

            // -- Credential --
            // Server uses Inbound credentials.
            use = CredentialUse.Inbound;

            // -- Invoke --

            SecurityStatus status = SecurityStatus.InternalError;

            this.Handle = new SafeCredentialHandle();

            // The finally clause is the actual constrained region. The VM pre-allocates any stack space,
            // performs any allocations it needs to prepare methods for execution, and postpones any 
            // instances of the 'uncatchable' exceptions (ThreadAbort, StackOverflow, OutOfMemory).
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                status = CredentialNativeMethods.AcquireCredentialsHandle(
                   null,
                   packageName,
                   use,
                   IntPtr.Zero,
                   IntPtr.Zero,
                   IntPtr.Zero,
                   IntPtr.Zero,
                   ref this.Handle.rawHandle,
                   ref rawExpiry
               );
            }

            if( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to call AcquireCredentialHandle", status );
            }

            this.Expiry = rawExpiry.ToDateTime();
        }

    }
}
