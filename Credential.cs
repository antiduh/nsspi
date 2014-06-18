using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public class Credential : IDisposable
    {
        private bool disposed;

        private SecurityPackage securityPackage;

        public Credential(SecurityPackage package, CredentialType credentialType)
        {
            this.disposed = false;

            this.securityPackage = package;
            
            Init();
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

        public string GetName()
        {
            return null;
        }

        // TODO use safe handle ...
        public IntPtr CredentialHandle
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            this.disposed = true;
        }

        private void Init()
        {
        }

    }
}
