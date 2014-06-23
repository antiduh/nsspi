using NSspi.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public class NativeMethods
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa374713(v=vs.85).aspx

        // The REMSSPI sample:

        // A C++ pure client/server example:
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa380536(v=vs.85).aspx

        /*
        SECURITY_STATUS sResult = AcquireCredentialsHandle(
		        NULL,											// [in] name of principal. NULL = principal of current security context
		        pszPackageName,									// [in] name of package
		        fCredentialUse,									// [in] flags indicating use.
		        NULL,											// [in] pointer to logon identifier.  NULL = we're not specifying the id of another logon session
		        NULL,											// [in] package-specific data.  NULL = default credentials for security package
		        NULL,											// [in] pointer to GetKey function.  NULL = we're not using a callback to retrieve the credentials
		        NULL,											// [in] value to pass to GetKey
		        this->credentialHandle,							// [out] credential handle (this must be already allocated)
		        &tsExpiry										// [out] lifetime of the returned credentials
        );
        
        SECURITY_STATUS SEC_Entry AcquireCredentialsHandle(
              _In_   SEC_CHAR *pszPrincipal,
              _In_   SEC_CHAR *pszPackage,
              _In_   ULONG fCredentialUse,
              _In_   PLUID pvLogonID,
              _In_   PVOID pAuthData,
              _In_   SEC_GET_KEY_FN pGetKeyFn,
              _In_   PVOID pvGetKeyArgument,
              _Out_  PCredHandle phCredential,
              _Out_  PTimeStamp ptsExpiry
            );
        */

        [DllImport(
            "Secur32.dll",
            EntryPoint = "AcquireCredentialsHandle",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus AcquireCredentialsHandle(
            string principleName,
            string packageName,
            CredentialUse credentialUse,
            IntPtr loginId,
            IntPtr packageData,
            IntPtr getKeyFunc,
            IntPtr getKeyData,
            ref long credentialHandle,
            ref long expiry
        );

        /*
        SECURITY_STATUS SEC_Entry FreeCredentialsHandle(
            _In_  PCredHandle phCredential
        );
        */
        [DllImport(
            "Secur32.dll",
            EntryPoint = "FreeCredentialsHandle",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus FreeCredentialsHandle(
            ref long credentialHandle
        );

        /*
        SECURITY_STATUS SEC_Entry FreeContextBuffer(
          _In_  PVOID pvContextBuffer
        );
        */
        [DllImport(
            "Secur32.dll",
            EntryPoint = "FreeContextBuffer",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus FreeContextBuffer( IntPtr buffer );
        

        /*
        SECURITY_STATUS SEC_Entry QueryCredentialsAttributes(
          _In_   PCredHandle phCredential,
          _In_   ULONG ulAttribute,
          _Out_  PVOID pBuffer
        );
        */

        /// <summary>
        /// The overload of the QueryCredentialsAttribute method that is used for querying the name attribute.
        /// In this call, it takes a void* to a structure that contains a wide char pointer. The wide character
        /// pointer is allocated by the SSPI api, and thus needs to be released by a call to FreeContextBuffer().
        /// </summary>
        /// <param name="credentialHandle"></param>
        /// <param name="attributeName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport(
            "Secur32.dll",
            EntryPoint = "QueryCredentialsAttributes",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true
        )]
        public static extern SecurityStatus QueryCredentialsAttribute_Name(
            ref long credentialHandle,
            CredentialQueryAttrib attributeName,
            ref QueryNameAttribCarrier name
        );

        [StructLayout( LayoutKind.Sequential )]
        public struct QueryNameAttribCarrier
        {
            public IntPtr Name;
        }

    }
}
