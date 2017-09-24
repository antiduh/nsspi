using System;
using System.Runtime.ConstrainedExecution;

namespace NSspi.Credentials
{
    /// <summary>
    /// Provides a managed handle to an SSPI credential.
    /// </summary>
    public class SafeCredentialHandle : SafeSspiHandle
    {
        public SafeCredentialHandle()
            : base()
        { }

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