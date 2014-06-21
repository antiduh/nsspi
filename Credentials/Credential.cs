using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public class Credential : IDisposable
    {
        private bool disposed;

        private SecurityPackage securityPackage;

        private long credHandle;
        private long expiry;

        public Credential(SecurityPackage package, CredentialType credentialType)
        {
            this.disposed = false;
            this.securityPackage = package;

            this.credHandle = 0;
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
            else if ( package == NSspi.SecurityPackage.Negotiate )
            {
                packageName = PackageNames.Negotiate;
            }
            else if ( package == NSspi.SecurityPackage.NTLM )
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
            SecurityStatus status = NativeMethods.AcquireCredentialsHandle(
                null,
                packageName,
                use,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                ref this.credHandle,
                ref this.expiry
            );

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
                NativeMethods.QueryNameAttribCarrier carrier = new NativeMethods.QueryNameAttribCarrier();

                SecurityStatus status;
                string name = null;

                status = NativeMethods.QueryCredentialsAttribute_Name(
                    ref this.credHandle,
                    CredentialQueryAttrib.Names,
                    ref carrier
                );

                if ( status == SecurityStatus.OK )
                {
                    name = Marshal.PtrToStringUni( carrier.Name );
                    NativeMethods.FreeContextBuffer( carrier.Name );
                }
                else
                {
                    throw new SSPIException( "Failed to query credential name", status );
                }

                return name;
            }
        }

        public long CredentialHandle
        {
            get
            {
                return this.credHandle;
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
                SecurityStatus result;

                result = NativeMethods.FreeCredentialsHandle( ref this.credHandle );

                this.disposed = true;

                if ( disposing && result != SecurityStatus.OK )
                {
                    throw new SSPIException( "Failed to release credentials handle", result );
                }
            }
        }
    }
}
