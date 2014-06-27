using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Buffers
{
    internal sealed class SecureBufferAdapter : CriticalFinalizerObject, IDisposable
    {
        private bool disposed;

        private IList<SecureBuffer> buffers;

        private GCHandle descriptorHandle;

        private GCHandle[] bufferHandles;

        private SecureBufferDescInternal descriptor;
        private SecureBufferInternal[] bufferCarrier;
        private GCHandle bufferCarrierHandle;

        public SecureBufferAdapter( SecureBuffer buffer )
            : this( new[] { buffer } )
        {

        }

        public SecureBufferAdapter( IList<SecureBuffer> buffers ) : base()
        {
            this.buffers = buffers;

            this.disposed = false;

            this.bufferHandles = new GCHandle[this.buffers.Count];
            this.bufferCarrier = new SecureBufferInternal[this.buffers.Count];

            for ( int i = 0; i < this.buffers.Count; i++ )
            {
                this.bufferHandles[i] = GCHandle.Alloc( this.buffers[i].Buffer, GCHandleType.Pinned );

                this.bufferCarrier[i] = new SecureBufferInternal();
                this.bufferCarrier[i].Type = this.buffers[i].Type;
                this.bufferCarrier[i].Count = this.buffers[i].Buffer.Length;
                this.bufferCarrier[i].Buffer = bufferHandles[i].AddrOfPinnedObject();
            }

            this.bufferCarrierHandle = GCHandle.Alloc( bufferCarrier, GCHandleType.Pinned );

            this.descriptor = new SecureBufferDescInternal();
            this.descriptor.Version = SecureBufferDescInternal.ApiVersion;
            this.descriptor.NumBuffers = this.buffers.Count;
            this.descriptor.Buffers = bufferCarrierHandle.AddrOfPinnedObject();

            this.descriptorHandle = GCHandle.Alloc( descriptor, GCHandleType.Pinned );
        }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        ~SecureBufferAdapter()
        {
            // We bend the typical Dispose pattern here. This finalizer runs in a Constrained Execution Region,
            // and so we shouldn't call virtual methods. There's no need to extend this class, so we prevent it
            // and mark the protected Dispose method as non-virtual.
            Dispose( false );
        }

        public IntPtr Handle
        {
            get
            {
                if ( this.disposed )
                {
                    throw new ObjectDisposedException( "Cannot use SecureBufferListHandle after it has been disposed" );
                }

                return this.descriptorHandle.AddrOfPinnedObject();
            }
        }

        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        private void Dispose( bool disposing )
        {
            if ( this.disposed == true ) { return; }

            if ( disposing )
            {
                for( int i = 0; i < this.buffers.Count; i++ )
                {
                    this.buffers[i].Length = this.bufferCarrier[i].Count;
                }
            }

            for( int i = 0; i < this.bufferHandles.Length; i++ )
            {
                if( this.bufferHandles[i].IsAllocated )
                {
                    this.bufferHandles[i].Free();
                }
            }

            if( this.bufferCarrierHandle.IsAllocated )
            {
                this.bufferCarrierHandle.Free();
            }

            if( this.descriptorHandle.IsAllocated )
            {
                this.descriptorHandle.Free();
            }

            this.disposed = true;
        }

    }
}
