using System;
using System.Runtime.ConstrainedExecution;

namespace NSspi.Credentials
{
    /// <summary>
    /// Provides a managed handle to an SSPI credential.
    /// </summary>
    public class SafeCredentialHandle : SafeSspiHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeCredentialHandle"/> class.
        /// </summary>
        public SafeCredentialHandle()
            : base()
        { }

        /// <summary>
        /// Releases the resources held by the credential handle.
        /// </summary>
        /// <returns></returns>
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        protected override bool ReleaseHandle()
        {
            SecurityStatus status = CredentialNativeMethods.FreeCredentialsHandle(
                ref base.rawHandle
            );

            base.ReleaseHandle();

            return status == SecurityStatus.OK;
        }
    }
}