using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NSspi.Credentials
{
    /// <summary>
    /// Represents credentials acquired by providing a username, password, and domain.
    /// </summary>
    public class PasswordCredential : Credential
    {
        /// <summary>
        /// Initializes a new instance of the PasswordCredential class.
        /// </summary>
        /// <remarks>
        /// It is possible to acquire a valid handle to credentials that do not provide a valid
        /// username-password combination. The username and password are not validation until the
        /// authentication cycle begins.
        /// </remarks>
        /// <param name="domain">The domain to authenticate to.</param>
        /// <param name="username">The username of the user to authenticate as.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="secPackage">The SSPI security package to create credentials for.</param>
        /// <param name="use">
        /// Specify inbound when acquiring credentials for a server; outbound for a client.
        /// </param>
        public PasswordCredential( string domain, string username, string password, string secPackage, CredentialUse use ) 
            : base( secPackage )
        {
            NativeAuthData authData = new NativeAuthData( domain, username, password, NativeAuthDataFlag.Unicode );

            Init( authData, secPackage, use );
        }

        private void Init( NativeAuthData authData, string secPackage, CredentialUse use )
        {
            string packageName;
            TimeStamp rawExpiry = new TimeStamp();
            SecurityStatus status = SecurityStatus.InternalError;

            // -- Package --
            // Copy off for the call, since this.SecurityPackage is a property.
            packageName = this.SecurityPackage;

            this.Handle = new SafeCredentialHandle();


            // The finally clause is the actual constrained region. The VM pre-allocates any stack space,
            // performs any allocations it needs to prepare methods for execution, and postpones any
            // instances of the 'uncatchable' exceptions (ThreadAbort, StackOverflow, OutOfMemory).
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                status = CredentialNativeMethods.AcquireCredentialsHandle_AuthData(
                   null,
                   packageName,
                   use,
                   IntPtr.Zero,
                   ref authData,
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
