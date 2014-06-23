using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public static class ContextNativeMethods
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
        */

        [DllImport(
            "Secur32.dll",
            EntryPoint = "AcceptSecurityContext",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus AcceptSecurityContext_1(
            ref long credHandle,
            IntPtr oldContextHandle,
            IntPtr inputBuffer,
            ContextAttrib requestedAttribs,
            SecureBufferDataRep dataRep,
            ref long newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib outputAttribs,
            ref long expiry
        );


        [DllImport(
            "Secur32.dll",
            EntryPoint = "AcceptSecurityContext",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus AcceptSecurityContext_2(
            ref long credHandle,
            ref long oldContextHandle,
            IntPtr inputBuffer,
            ContextAttrib requestedAttribs,
            SecureBufferDataRep dataRep,
            ref long newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib outputAttribs,
            ref long expiry
        );

        // When used in the ClientContext:
        /*
			SECURITY_STATUS sResult = InitializeSecurityContext(
				phCredential,										// [in] handle to the credentials
				NULL,												// [in/out] handle of partially formed context. Always NULL the first time through
				pwszServerPrincipalName,							// [in] name of the target of the context. Not needed by NTLM
				reqContextAttributes,								// [in] required context attributes
				0,													// [reserved] reserved; must be zero
				SECURITY_NATIVE_DREP,								// [in] data representation on the target
				NULL,												// [in/out] pointer to the input buffers.  Always NULL the first time through
				0,													// [reserved] reserved; must be zero
				this->contextHandle,								// [in/out] receives the new context handle (must be pre-allocated)
				&outBuffDesc,										// [out] pointer to the output buffers
				pulContextAttributes,								// [out] receives the context attributes
				&tsLifeSpan											// [out] receives the life span of the security context
			);
        */
        /*
        SECURITY_STATUS SEC_Entry InitializeSecurityContext(
          _In_opt_     PCredHandle phCredential,
          _In_opt_     PCtxtHandle phContext,
          _In_opt_     SEC_CHAR *pszTargetName,
          _In_         ULONG fContextReq,
          _In_         ULONG Reserved1,
          _In_         ULONG TargetDataRep,
          _In_opt_     PSecBufferDesc pInput,
          _In_         ULONG Reserved2,
          _Inout_opt_  PCtxtHandle phNewContext,
          _Inout_opt_  PSecBufferDesc pOutput,
          _Out_        PULONG pfContextAttr,
          _Out_opt_    PTimeStamp ptsExpiry
        );
        */

        [DllImport(
            "Secur32.dll",
            EntryPoint = "InitializeSecurityContext",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus InitializeSecurityContext_1(
            ref long credentialHandle,
            IntPtr zero,
            string serverPrincipleName,
            ContextAttrib requiredAttribs,
            int reserved1,
            SecureBufferDataRep dataRep,
            IntPtr inputBuffer,
            int reserved2,
            ref long newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib contextAttribs,
            ref long expiry
        );

        [DllImport(
            "Secur32.dll",
            EntryPoint = "InitializeSecurityContext",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus InitializeSecurityContext_2(
            ref long credentialHandle,
            ref long previousHandle,
            string serverPrincipleName,
            ContextAttrib requiredAttribs,
            int reserved1,
            SecureBufferDataRep dataRep,
            IntPtr inputBuffer,
            int reserved2,
            ref long newContextHandle,
            IntPtr outputBuffer,
            ref ContextAttrib contextAttribs,
            ref long expiry
        );

        [DllImport(
            "Secur32.dll",
            EntryPoint = "DeleteSecurityContext",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus DeleteSecurityContext( ref long contextHandle );

        [DllImport(
            "Secur32.dll",
            EntryPoint = "EncryptMessage",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus EncryptMessage(
            ref long contextHandle,
            int qualityOfProtection,
            IntPtr bufferDescriptor,
            int sequenceNumber
        );


        [DllImport(
            "Secur32.dll",
            EntryPoint = "DecryptMessage",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus DecryptMessage(
            ref long contextHandle,
            IntPtr bufferDescriptor,
            int sequenceNumber,
            int qualityOfProtection
        );

        [DllImport(
            "Secur32.dll",
            EntryPoint = "QueryContextAttributes",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode )]
        public static extern SecurityStatus QueryContextAttributes_Sizes(
            ref long contextHandle,
            ContextQueryAttrib attrib,
            ref SecPkgContext_Sizes sizes
        );

        [DllImport(
            "Secur32.dll",
            EntryPoint = "QueryContextAttributes",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode )]
        public static extern SecurityStatus QueryContextAttributes_String(
            ref long contextHandle,
            ContextQueryAttrib attrib,
            ref SecPkgContext_String names
        );


        [DllImport( 
            "Secur32.dll",
            EntryPoint = "FreeContextBuffer", 
            CallingConvention = CallingConvention.Winapi, 
            CharSet = CharSet.Unicode )]
        public static extern SecurityStatus FreeContextBuffer( IntPtr handle );
    }
}
