using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NSspi.Contexts;

namespace NSspi
{
    /// <summary>
    /// Represents any SSPI handle created for credential handles, context handles, and security package 
    /// handles. Any SSPI handle is always the size of two native pointers. 
    /// </summary>
    /// <remarks>
    /// The documentation for SSPI handles can be found here:
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa380495(v=vs.85).aspx
    /// 
    /// This class is not reference safe - if used directly, or referenced directly, it may be leaked,
    /// or subject to finalizer races, or any of the hundred of things SafeHandles were designed to fix.
    /// Do not directly use this class - use only though SafeHandle wrapper objects. Any reference needed
    /// to this handle for performing work (InitializeSecurityContext, eg), should be done through
    /// a second class SafeSspiHandleReference so that reference counting is properly executed.
    /// </remarks>
    [StructLayout( LayoutKind.Sequential, Pack = 1 ) ]
    public struct RawSspiHandle
    {
        private IntPtr lowPart;
        private IntPtr highPart;

        public bool IsZero()
        {
            return this.lowPart == IntPtr.Zero && this.highPart == IntPtr.Zero;
        }

        // This guy has to be executed in a CER.
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success)]
        public void SetInvalid()
        {
            this.lowPart = IntPtr.Zero;
            this.highPart = IntPtr.Zero;
        }
    }


    public abstract class SafeSspiHandle : SafeHandle
    {
        internal RawSspiHandle rawHandle;

        protected SafeSspiHandle()
            : base( IntPtr.Zero, true )
        {
            this.rawHandle = new RawSspiHandle();
        }

        public override bool IsInvalid
        {
            get { return IsClosed || this.rawHandle.IsZero();  }
        }

        protected override bool ReleaseHandle()
        {
            this.rawHandle.SetInvalid();
            return true;
        }
    }

    public class SafeCredentialHandle : SafeSspiHandle
    {
        public SafeCredentialHandle()
            : base()
        { }

        protected override bool ReleaseHandle()
        {
            SecurityStatus status = NativeMethods.FreeCredentialsHandle(
                ref base.rawHandle
            );

            base.ReleaseHandle();

            return status == SecurityStatus.OK;
        }
    }

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
