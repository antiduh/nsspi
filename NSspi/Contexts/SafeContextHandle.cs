using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{

    public class SafeContextHandle : SafeSspiHandle
    {
        public SafeContextHandle()
            : base()
        { }

        protected override bool ReleaseHandle()
        {
            SecurityStatus status = ContextNativeMethods.DeleteSecurityContext(
                ref base.rawHandle
            );

            base.ReleaseHandle();

            return status == SecurityStatus.OK;
        }
    }
}
