using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NSspi.Credentials;
using NSspi.Credentials.Credentials;

namespace NSspi.Credentials
{
    public class Credential : IDisposable
    {
        private bool disposed;

        private SecurityPackage securityPackage;

        private SafeCredentialHandle safeCredHandle;
        private long expiry;

        public Credential(SecurityPackage package, CredentialType credentialType)
        {
            this.disposed = false;
            this.securityPackage = package;

            this.expiry = 0;

            Init( package, credentialType );
        }

        private void Init( SecurityPackage package, CredentialType credentialType )
        {
            string packageName;
            CredentialUse use;

            // -- Package --
            if ( package == SecurityPackage.Kerberos )
            {
                packageName = PackageNames.Kerberos;
            }
            else if ( package == SecurityPackage.Negotiate )
            {
                packageName = PackageNames.Negotiate;
            }
            else if ( package == SecurityPackage.NTLM )
            {
                packageName = PackageNames.Ntlm;
            }
            else
            {
                throw new ArgumentException( "Invalid value provided for the 'package' parameter." );
            }

            // -- Credential --
            if ( credentialType == CredentialType.Client )
            {
                use = CredentialUse.Outbound;
            }
            else if ( credentialType == CredentialType.Server )
            {
                use = CredentialUse.Inbound;
            }
            else
            {
                throw new ArgumentException( "Invalid value provided for the 'credentialType' parameter." );
            }

            // -- Invoke --
            
            SecurityStatus status = SecurityStatus.InternalError;

            this.safeCredHandle = new SafeCredentialHandle();

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
                   ref this.safeCredHandle.rawHandle,
                   ref this.expiry
               );
            }

            if ( status != SecurityStatus.OK )
            {
                throw new SSPIException( "Failed to call AcquireCredentialHandle", status );
            }
        }

        ~Credential()
        {
            Dispose( false );
        }

        public SecurityPackage SecurityPackage
        {
            get
            {
                if( this.disposed )
                {
                    throw new ObjectDisposedException( base.GetType().Name );
                }

                return this.securityPackage;
            }
        }

        public string Name
        {
            get
            {
                QueryNameAttribCarrier carrier = new QueryNameAttribCarrier();

                SecurityStatus status = SecurityStatus.InternalError;
                string name = null;
                bool gotRef = false;

                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    this.safeCredHandle.DangerousAddRef( ref gotRef );
                }
                catch( Exception )
                {
                    if( gotRef == true )
                    {
                        this.safeCredHandle.DangerousRelease();
                        gotRef = false;
                    }
                    throw;
                }
                finally
                {
                    if( gotRef )
                    {
                        status = CredentialNativeMethods.QueryCredentialsAttribute_Name(
                            ref this.safeCredHandle.rawHandle,
                            CredentialQueryAttrib.Names,
                            ref carrier
                        );

                        this.safeCredHandle.DangerousRelease();

                        if( status == SecurityStatus.OK && carrier.Name != IntPtr.Zero )
                        {
                            name = Marshal.PtrToStringUni( carrier.Name );
                            NativeMethods.FreeContextBuffer( carrier.Name );
                        }
                    }
                }

                if( status.IsError() )
                {
                    throw new SSPIException( "Failed to query credential name", status );
                }

                return name;
            }
        }

        public SafeCredentialHandle Handle
        {
            get
            {
                return this.safeCredHandle;
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( this.disposed == false )
            {
                if ( disposing )
                {
                    this.safeCredHandle.Dispose();
                }

                this.disposed = true;
            }
        }

    }
}
