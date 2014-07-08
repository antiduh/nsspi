using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    /// <summary>
    /// Captures an unmanaged security context handle.
    /// </summary>
    public class SafeContextHandle : SafeSspiHandle
    {
        public SafeContextHandle()
            : base()
        { }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
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
