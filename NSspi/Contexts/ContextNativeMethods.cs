using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using NSspi.Buffers;

namespace NSspi.Contexts
{
    /// <summary>
    /// Declares native methods calls for security context-related win32 functions.
    /// </summary>
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

        [DllImport( "Secur32.dll", EntryPoint = "AcceptSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus AcceptSecurityContext_1(
            ref RawSspiHandle credHandle,
            IntPtr oldContextHandle,
            IntPtr inputBuffer,
            ContextAttrib requestedAttribs,
            SecureBufferDataRep dataRep,
            ref RawSspiHandle newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib outputAttribs,
            ref TimeStamp expiry
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
            ref TimeStamp expiry
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
            ref TimeStamp expiry
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
            ref TimeStamp expiry
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "DeleteSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus DeleteSecurityContext( ref RawSspiHandle contextHandle );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
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

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "Secur32.dll", EntryPoint = "MakeSignature", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus MakeSignature(
            ref RawSspiHandle contextHandle,
            int qualityOfProtection,
            IntPtr bufferDescriptor,
            int sequenceNumber
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "Secur32.dll", EntryPoint = "VerifySignature", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus VerifySignature(
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

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "QueryContextAttributes", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus QueryContextAttributes_String(
            ref RawSspiHandle contextHandle,
            ContextQueryAttrib attrib,
            ref SecPkgContext_String names
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "QueryContextAttributes", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus QueryContextAttributes(
           ref RawSspiHandle contextHandle,
           ContextQueryAttrib attrib,
           IntPtr attribute
       );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "FreeContextBuffer", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus FreeContextBuffer( IntPtr handle );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "ImpersonateSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus ImpersonateSecurityContext( ref RawSspiHandle contextHandle );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "RevertSecurityContext", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus RevertSecurityContext( ref RawSspiHandle contextHandle );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "QuerySecurityContextToken", SetLastError = true )]
        internal static extern SecurityStatus QuerySecurityContextToken( ref RawSspiHandle contextHandle, [Out] out SafeTokenHandle handle );

        [StructLayout( LayoutKind.Sequential )]
        private class KeyStruct
        {
            public int size;
            public IntPtr data;
        }

        internal static SecurityStatus SafeQueryContextAttribute(
            SafeContextHandle handle,
            ContextQueryAttrib attribute,
            ref byte[] buffer
            )
        {
            bool gotRef = false;

            SecurityStatus status = SecurityStatus.InternalError;
            RuntimeHelpers.PrepareConstrainedRegions();

            int pointerSize = System.Environment.Is64BitOperatingSystem ? 8 : 4; //NOTE: update this when 128 bit processors exist
            IntPtr alloc_buffer = Marshal.AllocHGlobal( sizeof( uint ) + pointerSize ); //NOTE: this is at most 4 + sizeof(void*) bytes
                                                                                        //see struct SecPkgContext_SessionKey
                                                                                        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa380096(v=vs.85).aspx
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
                    buffer = null;
                }

                throw;
            }
            finally
            {
                if( gotRef )
                {
                    status = ContextNativeMethods.QueryContextAttributes(
                        ref handle.rawHandle,
                        attribute,
                        alloc_buffer
                    );
                    if( status == SecurityStatus.OK )
                    {
                        KeyStruct key = new KeyStruct();

                        Marshal.PtrToStructure( alloc_buffer, key ); // fit to the proper size, read a byte[]

                        byte[] sizedBuffer = new byte[key.size];

                        for( int i = 0; i < key.size; i++ )
                            sizedBuffer[i] = Marshal.ReadByte( key.data, i );

                        buffer = sizedBuffer;
                    }
                    handle.DangerousRelease();
                }
            }
            Marshal.FreeHGlobal( alloc_buffer );
            return status;
        }

        /// <summary>
        /// Safely invokes the native EncryptMessage function, making sure that handle ref counting is
        /// performed in a proper CER.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="qualityOfProtection"></param>
        /// <param name="bufferAdapter"></param>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
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
                    status = ContextNativeMethods.EncryptMessage(
                        ref handle.rawHandle,
                        qualityOfProtection,
                        bufferAdapter.Handle,
                        sequenceNumber
                    );

                    handle.DangerousRelease();
                }
            }

            return status;
        }

        /// <summary>
        /// Safely invokes the native DecryptMessage function, making sure that handle ref counting is
        /// performed in a proper CER.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="qualityOfProtection"></param>
        /// <param name="bufferAdapter"></param>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
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
                        sequenceNumber,
                        qualityOfProtection
                    );

                    handle.DangerousRelease();
                }
            }

            return status;
        }

        /// <summary>
        /// Safely invokes the native MakeSignature function, making sure that handle ref counting is
        /// performed in a proper CER.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="qualityOfProtection"></param>
        /// <param name="adapter"></param>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        internal static SecurityStatus SafeMakeSignature(
            SafeContextHandle handle,
            int qualityOfProtection,
            SecureBufferAdapter adapter,
            int sequenceNumber )
        {
            bool gotRef = false;
            SecurityStatus status = SecurityStatus.InternalError;

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
                    status = ContextNativeMethods.MakeSignature(
                        ref handle.rawHandle,
                        qualityOfProtection,
                        adapter.Handle,
                        sequenceNumber
                    );

                    handle.DangerousRelease();
                }
            }

            return status;
        }

        /// <summary>
        /// Safely invokes the native VerifySignature function, making sure that handle ref counting is
        /// performed in a proper CER.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="qualityOfProtection"></param>
        /// <param name="adapter"></param>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        internal static SecurityStatus SafeVerifySignature(
            SafeContextHandle handle,
            int qualityOfProtection,
            SecureBufferAdapter adapter,
            int sequenceNumber )
        {
            bool gotRef = false;
            SecurityStatus status = SecurityStatus.InternalError;

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
                    status = ContextNativeMethods.VerifySignature(
                        ref handle.rawHandle,
                        adapter.Handle,
                        sequenceNumber,
                        qualityOfProtection
                    );

                    handle.DangerousRelease();
                }
            }

            return status;
        }
    }
}