using System;
using System.Runtime.ConstrainedExecution;

namespace NSspi.Contexts
{
    /// <summary>
    /// Captures an unmanaged security context handle.
    /// </summary>
    public class SafeContextHandle : SafeSspiHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeContextHandle"/> class.
        /// </summary>
        public SafeContextHandle()
            : base()
        { }

        /// <summary>
        /// Releases the safe context handle.
        /// </summary>
        /// <returns></returns>
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