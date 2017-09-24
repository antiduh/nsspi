using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace NSspi.Credentials
{
    internal static class CredentialNativeMethods
    {
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "Secur32.dll", EntryPoint = "AcquireCredentialsHandle", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus AcquireCredentialsHandle(
            string principleName,
            string packageName,
            CredentialUse credentialUse,
            IntPtr loginId,
            IntPtr packageData,
            IntPtr getKeyFunc,
            IntPtr getKeyData,
            ref RawSspiHandle credentialHandle,
            ref TimeStamp expiry
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "Secur32.dll", EntryPoint = "AcquireCredentialsHandle", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus AcquireCredentialsHandle_AuthData(
            string principleName,
            string packageName,
            CredentialUse credentialUse,
            IntPtr loginId,
            ref NativeAuthData authData,
            IntPtr getKeyFunc,
            IntPtr getKeyData,
            ref RawSspiHandle credentialHandle,
            ref TimeStamp expiry
        );


        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "FreeCredentialsHandle", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus FreeCredentialsHandle(
            ref RawSspiHandle credentialHandle
        );

        /// <summary>
        /// The overload of the QueryCredentialsAttribute method that is used for querying the name attribute.
        /// In this call, it takes a void* to a structure that contains a wide char pointer. The wide character
        /// pointer is allocated by the SSPI api, and thus needs to be released by a call to FreeContextBuffer().
        /// </summary>
        /// <param name="credentialHandle"></param>
        /// <param name="attributeName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [DllImport( "Secur32.dll", EntryPoint = "QueryCredentialsAttributes", CharSet = CharSet.Unicode )]
        internal static extern SecurityStatus QueryCredentialsAttribute_Name(
            ref RawSspiHandle credentialHandle,
            CredentialQueryAttrib attributeName,
            ref QueryNameAttribCarrier name
        );
    }
}