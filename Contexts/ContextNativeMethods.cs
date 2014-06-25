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
    internal static class ContextNativeMethods
    {
        /*
        SECURITY_STATUS SEC_Entry AcceptSecurityContext(
          _In_opt_     PCredHandle phCredential,
          _Inout_      PCtxtHandle phContext,
          _In_opt_     PSecBufferDesc pInput,
          _In_         ULONG fContextReq,
          _In_         ULONG TargetDataRep,
          _Inout_opt_  PCtxtHandle phNewContext,
          _Inout_opt_  PSecBufferDesc pOutput,
          _Out_        PULONG pfContextAttr,
          _Out_opt_    PTimeStamp ptsTimeStamp
        );
         
        SECURITY_STATUS SEC_Entry InitializeSecurityContext(
          _In_opt_     PCredHandle phCredential,                // [in] handle to the credentials
          _In_opt_     PCtxtHandle phContext,                   // [in/out] handle of partially formed context. Always NULL the first time through
          _In_opt_     SEC_CHAR *pszTargetName,                 // [in] name of the target of the context. Not needed by NTLM
          _In_         ULONG fContextReq,                       // [in] required context attributes
          _In_         ULONG Reserved1,                         // [reserved] reserved; must be zero
          _In_         ULONG TargetDataRep,                     // [in] data representation on the target
          _In_opt_     PSecBufferDesc pInput,                   // [in/out] pointer to the input buffers.  Always NULL the first time through
          _In_         ULONG Reserved2,                         // [reserved] reserved; must be zero
          _Inout_opt_  PCtxtHandle phNewContext,                // [in/out] receives the new context handle (must be pre-allocated)
          _Inout_opt_  PSecBufferDesc pOutput,                  // [out] pointer to the output buffers
          _Out_        PULONG pfContextAttr,                    // [out] receives the context attributes
          _Out_opt_    PTimeStamp ptsExpiry                     // [out] receives the life span of the security context
        );
        */

        [DllImport( "Secur32.dll", EntryPoint = "AcceptSecurityContext",CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus AcceptSecurityContext_1(
            ref RawSspiHandle credHandle,
            IntPtr oldContextHandle,
            IntPtr inputBuffer,
            ContextAttrib requestedAttribs,
            SecureBufferDataRep dataRep,
            ref RawSspiHandle newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib outputAttribs,
            ref long expiry
        );


        [DllImport( "Secur32.dll", EntryPoint = "AcceptSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus AcceptSecurityContext_2(
            ref RawSspiHandle credHandle,
            ref RawSspiHandle oldContextHandle,
            IntPtr inputBuffer,
            ContextAttrib requestedAttribs,
            SecureBufferDataRep dataRep,
            ref RawSspiHandle newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib outputAttribs,
            ref long expiry
        );


        [DllImport( "Secur32.dll", EntryPoint = "InitializeSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus InitializeSecurityContext_1(
            ref RawSspiHandle credentialHandle,
            IntPtr zero,
            string serverPrincipleName,
            ContextAttrib requiredAttribs,
            int reserved1,
            SecureBufferDataRep dataRep,
            IntPtr inputBuffer,
            int reserved2,
            ref RawSspiHandle newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib contextAttribs,
            ref long expiry
        );


        [DllImport( "Secur32.dll", EntryPoint = "InitializeSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus InitializeSecurityContext_2(
            ref RawSspiHandle credentialHandle,
            ref RawSspiHandle previousHandle,
            string serverPrincipleName,
            ContextAttrib requiredAttribs,
            int reserved1,
            SecureBufferDataRep dataRep,
            IntPtr inputBuffer,
            int reserved2,
            ref RawSspiHandle newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib contextAttribs,
            ref long expiry
        );


        [DllImport( "Secur32.dll", EntryPoint = "DeleteSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus DeleteSecurityContext( ref RawSspiHandle contextHandle );


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "Secur32.dll", EntryPoint = "EncryptMessage", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus EncryptMessage(
            ref RawSspiHandle contextHandle,
            int qualityOfProtection,
            IntPtr bufferDescriptor,
            int sequenceNumber
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "Secur32.dll", EntryPoint = "DecryptMessage", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus DecryptMessage(
            ref RawSspiHandle contextHandle,
            IntPtr bufferDescriptor,
            int sequenceNumber,
            int qualityOfProtection
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "QueryContextAttributes", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus QueryContextAttributes_Sizes(
            ref RawSspiHandle contextHandle,
            ContextQueryAttrib attrib,
            ref SecPkgContext_Sizes sizes
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport( "Secur32.dll", EntryPoint = "QueryContextAttributes", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus QueryContextAttributes_String(
            ref RawSspiHandle contextHandle,
            ContextQueryAttrib attrib,
            ref SecPkgContext_String names
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "FreeContextBuffer", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus FreeContextBuffer( IntPtr handle );

        internal static SecurityStatus SafeEncryptMessage(
            SafeContextHandle handle,
            int qualityOfProtection,
            SecureBufferAdapter bufferAdapter,
            int sequenceNumber )
        {
            SecurityStatus status = SecurityStatus.InternalError;
            bool gotRef = false;

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                handle.DangerousAddRef( ref gotRef );
            }
            catch ( Exception )
            {
                if ( gotRef )
                {
                    handle.DangerousRelease();
                    gotRef = false;
                }

                throw;
            }
            finally
            {
                if ( gotRef )
                {
                    status = ContextNativeMethods.EncryptMessage(
                        ref handle.rawHandle,
                        0,
                        bufferAdapter.Handle,
                        0
                    );

                    handle.DangerousRelease();
                }
            }

            return status;
        }

        internal static SecurityStatus SafeDecryptMessage( 
            SafeContextHandle handle, 
            int qualityOfProtection, 
            SecureBufferAdapter bufferAdapter, 
            int sequenceNumber )
        {
            SecurityStatus status = SecurityStatus.InvalidHandle;
            bool gotRef = false;

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                handle.DangerousAddRef( ref gotRef );
            }
            catch( Exception )
            {
                if( gotRef )
                {
                    handle.DangerousRelease();
                    gotRef = false;
                }

                throw;
            }
            finally
            {
                if( gotRef )
                {
                    status = ContextNativeMethods.DecryptMessage(
                        ref handle.rawHandle,
                        bufferAdapter.Handle,
                        0,
                        0
                    );

                    handle.DangerousRelease();
                }
            }

            return status;
        }
    }
}
