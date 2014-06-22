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
    }
}
