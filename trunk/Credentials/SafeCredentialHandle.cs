using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{

    public class SafeCredentialHandle : SafeSspiHandle
    {
        public SafeCredentialHandle()
            : base()
        { }

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
