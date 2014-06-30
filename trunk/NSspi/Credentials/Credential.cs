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

        private string securityPackage;

        private SafeCredentialHandle safeCredHandle;

        private DateTime expiry;

        public Credential( string package )
        {
            this.disposed = false;
            this.securityPackage = package;

            this.expiry = DateTime.MinValue;
        }
      
        ~Credential()
        {
            Dispose( false );
        }

        public string SecurityPackage
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
                QueryNameAttribCarrier carrier;
                SecurityStatus status;
                string name = null;
                bool gotRef = false;

                if( this.disposed )
                {
                    throw new ObjectDisposedException( "Credential" );
                }

                status = SecurityStatus.InternalError;
                carrier = new QueryNameAttribCarrier();

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
                            try
                            {
                                name = Marshal.PtrToStringUni( carrier.Name );
                            }
                            finally
                            {
                                NativeMethods.FreeContextBuffer( carrier.Name );
                            }
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

        public DateTime Expiry
        {
            get
            {
                if( this.disposed )
                {
                    throw new ObjectDisposedException( "Credential" );
                }

                return this.expiry;
            }

            protected set
            {
                if( this.disposed )
                {
                    throw new ObjectDisposedException( "Credential" );
                } 
                
                this.expiry = value;
            }
        }

        public SafeCredentialHandle Handle
        {
            get
            {
                if( this.disposed )
                {
                    throw new ObjectDisposedException( "Credential" );
                }

                return this.safeCredHandle;
            }

            protected set
            {
                if( this.disposed )
                {
                    throw new ObjectDisposedException( "Credential" );
                }

                this.safeCredHandle = value;
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
