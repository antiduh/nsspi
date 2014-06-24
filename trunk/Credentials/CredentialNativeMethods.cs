using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NSspi.Credentials;

namespace NSspi
{
    public static class CredentialNativeMethods
    {
        /*
        SECURITY_STATUS SEC_Entry AcquireCredentialsHandle(
           _In_   SEC_CHAR *pszPrincipal,                   // [in] name of principal. NULL = principal of current security context
           _In_   SEC_CHAR *pszPackage,                     // [in] name of security package - "Kerberos", "Negotiate", "NTLM", etc
           _In_   ULONG fCredentialUse,                     // [in] flags indicating use.
           _In_   PLUID pvLogonID,                          // [in] pointer to logon identifier.  NULL = we're not specifying the id of another logon session
           _In_   PVOID pAuthData,                          // [in] package-specific data.  NULL = default credentials for security package
           _In_   SEC_GET_KEY_FN pGetKeyFn,                 // [in] pointer to GetKey function.  NULL = we're not using a callback to retrieve the credentials
           _In_   PVOID pvGetKeyArgument,                   // [in] value to pass to GetKey
           _Out_  PCredHandle phCredential,                 // [out] credential handle (this must be already allocated)
           _Out_  PTimeStamp ptsExpiry                      // [out] lifetime of the returned credentials
        );
          
        SECURITY_STATUS SEC_Entry FreeCredentialsHandle(
            _In_  PCredHandle phCredential
        );
          
        SECURITY_STATUS SEC_Entry QueryCredentialsAttributes(
          _In_   PCredHandle phCredential,
          _In_   ULONG ulAttribute,
          _Out_  PVOID pBuffer
        );
        */

        [DllImport( "Secur32.dll", EntryPoint = "AcquireCredentialsHandle", CharSet = CharSet.Unicode )]
        public static extern SecurityStatus AcquireCredentialsHandle(
            string principleName,
            string packageName,
            CredentialUse credentialUse,
            IntPtr loginId,
            IntPtr packageData,
            IntPtr getKeyFunc,
            IntPtr getKeyData,
            ref RawSspiHandle credentialHandle,
            ref long expiry
        );

        
        [DllImport( "Secur32.dll", EntryPoint = "FreeCredentialsHandle", CharSet = CharSet.Unicode )]
        public static extern SecurityStatus FreeCredentialsHandle(
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
        [DllImport( "Secur32.dll", EntryPoint = "QueryCredentialsAttributes", CharSet = CharSet.Unicode )]
        public static extern SecurityStatus QueryCredentialsAttribute_Name(
            ref RawSspiHandle credentialHandle,
            CredentialQueryAttrib attributeName,
            ref QueryNameAttribCarrier name
        );
    }
}
