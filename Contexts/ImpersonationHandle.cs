using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public class ImpersonationHandle : IDisposable
    {
        // Notes:
        // Impersonate:
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa375497(v=vs.85).aspx
        // 
        // Revert:
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa379446(v=vs.85).aspx
        //
        // QuerySecurityPkgInfo (to learn if it supports impersonation):
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa379359(v=vs.85).aspx

        private bool disposed;
        private ServerContext server;

        internal ImpersonationHandle(ServerContext server)
        {
            this.server = server;
        }

        ~ImpersonationHandle()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {

        }

    }
}
