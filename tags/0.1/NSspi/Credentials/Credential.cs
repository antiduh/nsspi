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

namespace NSspi.Credentials
{
    /// <summary>
    /// Provides access to the pre-existing credentials of a security principle.
    /// </summary>
    public class Credential : IDisposable
    {
        /// <summary>
        /// Whether the Credential has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The name of the security package that controls the credential.
        /// </summary>
        private string securityPackage;

        /// <summary>
        /// A safe handle to the credential's handle.
        /// </summary>
        private SafeCredentialHandle safeCredHandle;

        /// <summary>
        /// The UTC time the credentials expire.
        /// </summary>
        private DateTime expiry;

        /// <summary>
        /// Initializes a new instance of the Credential class.
        /// </summary>
        /// <param name="package">The security package to acquire the credential from.</param>
        public Credential( string package )
        {
            this.disposed = false;
            this.securityPackage = package;

            this.expiry = DateTime.MinValue;

            this.PackageInfo = PackageSupport.GetPackageCapabilities( this.SecurityPackage );
        }
        
        /// <summary>
        /// Gets metadata for the security package associated with the credential.
        /// </summary>
        public SecPkgInfo PackageInfo { get; private set; }

        /// <summary>
        /// Gets the name of the security package that owns the credential.
        /// </summary>
        public string SecurityPackage
        {
            get
            {
                CheckLifecycle();

                return this.securityPackage;
            }
        }

        /// <summary>
        /// Returns the User Principle Name of the credential. Depending on the underlying security
        /// package used by the credential, this may not be the same as the Down-Level Logon Name
        /// for the user.
        /// </summary>
        public string PrincipleName
        {
            get
            {
                QueryNameAttribCarrier carrier;
                SecurityStatus status;
                string name = null;
                bool gotRef = false;

                CheckLifecycle();

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

        /// <summary>
        /// Gets the UTC time the credentials expire.
        /// </summary>
        public DateTime Expiry
        {
            get
            {
                CheckLifecycle();

                return this.expiry;
            }

            protected set
            {
                CheckLifecycle();
                                
                this.expiry = value;
            }
        }

        /// <summary>
        /// Gets a handle to the credential.
        /// </summary>
        public SafeCredentialHandle Handle
        {
            get
            {
                CheckLifecycle();

                return this.safeCredHandle;
            }

            protected set
            {
                CheckLifecycle();

                this.safeCredHandle = value;
            }
        }

        /// <summary>
        /// Releases all resources associated with the credential.
        /// </summary>
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

        private void CheckLifecycle()
        {
            if( this.disposed )
            {
                throw new ObjectDisposedException( "Credential" );
            }
        }
    }
}
