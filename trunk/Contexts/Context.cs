using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public class Context : IDisposable
    {
        private bool disposed;

        public Context( Credential cred )
        {
            this.Credential = cred;

            this.disposed = false;
        }

        ~Context()
        {
            Dispose( false );
        }

        protected Credential Credential { get; private set; }

        public long ContextHandle { get; protected set; }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if( this.disposed ) { return; }

            if( disposing )
            {
                this.Credential.Dispose();
            }

            long contextHandleCopy = this.ContextHandle;
            NativeMethods.DeleteSecurityContext( ref contextHandleCopy );

            this.ContextHandle = 0;

            this.disposed = true;
        }
    }
}
