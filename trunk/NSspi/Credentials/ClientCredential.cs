using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    public class ClientCredential : Credential
    {
        public ClientCredential( SecurityPackage package )
            : base( package )
        {
            Init();
        }

        private void Init( )
        {
            string packageName;
            CredentialUse use;
            TimeStamp rawExpiry = new TimeStamp();

            // -- Package --
            if( this.SecurityPackage == SecurityPackage.Kerberos )
            {
                packageName = PackageNames.Kerberos;
            }
            else if( this.SecurityPackage == SecurityPackage.Negotiate )
            {
                packageName = PackageNames.Negotiate;
            }
            else if( this.SecurityPackage == SecurityPackage.NTLM )
            {
                packageName = PackageNames.Ntlm;
            }
            else
            {
                throw new ArgumentException( "Invalid value provided for the 'package' parameter." );
            }

            // -- Credential --
            // Client uses outbound credentials.
            use = CredentialUse.Outbound;

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
