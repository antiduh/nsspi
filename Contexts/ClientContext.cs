using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public class ClientContext : Context
    {
        public ClientContext( ClientCredential cred, string serverPrinc, ContextAttrib attribs )
            : base( cred )
        {
            long credHandle = base.Credential.CredentialHandle;

            long prevContextHandle = 0;
            long newContextHandle = 0;

            long expiry = 0;
            ContextAttrib newContextAttribs = 0;

            SecurityStatus status;
            SecureBufferDesc descriptor;
            SecureBuffer secureBuffer;
            byte[] tokenBuffer = new byte[100000];
            GCHandle tokenBufferHandle;
            GCHandle bufferArrayHandle;
            GCHandle descriptorHandle;
            SecureBuffer[] bufferArray;

            tokenBufferHandle = GCHandle.Alloc( tokenBuffer, GCHandleType.Pinned );

            secureBuffer = new SecureBuffer();
            secureBuffer.Type = BufferType.Token;
            secureBuffer.Count = tokenBuffer.Length;
            secureBuffer.Buffer = tokenBufferHandle.AddrOfPinnedObject();

            bufferArray = new SecureBuffer[1];
            bufferArray[0] = secureBuffer;
            bufferArrayHandle = GCHandle.Alloc( bufferArray, GCHandleType.Pinned );
            
            descriptor = new SecureBufferDesc();
            descriptor.Version = SecureBufferDesc.ApiVersion;
            descriptor.NumBuffers = bufferArray.Length;
            descriptor.Buffers = bufferArrayHandle.AddrOfPinnedObject();

            descriptorHandle = GCHandle.Alloc( descriptor, GCHandleType.Pinned );

            status = NativeMethods.InitializeSecurityContext_Client1(
                ref credHandle,
                IntPtr.Zero,
                serverPrinc,
                attribs,
                0,
                SecureBufferDataRep.Network,
                IntPtr.Zero,
                0,
                ref newContextHandle,
                descriptorHandle.AddrOfPinnedObject(),
                ref newContextAttribs,
                ref expiry
            );

            descriptorHandle.Free();
            bufferArrayHandle.Free();
            tokenBufferHandle.Free();

            secureBuffer = bufferArray[0];

            Console.Out.WriteLine( status );
            base.ContextHandle = newContextHandle;
            
        }
    }
}
