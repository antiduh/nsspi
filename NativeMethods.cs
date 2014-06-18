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

        [DllImport( "Secur32.dll", CallingConvention = CallingConvention.Winapi, SetLastError=true)]
        public extern int AcquireCredentialHandle(
            string principleName,
            string packageName,
            CredentialUse credentialUse,
            IntPtr loginId,
            IntPtr packageData,
            IntPtr getKeyFunc,
            IntPtr getKeyData,
            IntPtr credentialHandle,
            ref long expiry
        );
    }
}
