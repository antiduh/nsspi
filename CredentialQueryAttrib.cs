using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    /*
     #define SECPKG_CRED_ATTR_NAMES        1
     #define SECPKG_CRED_ATTR_SSI_PROVIDER 2
     #define SECPKG_CRED_ATTR_KDC_PROXY_SETTINGS 3
     #define SECPKG_CRED_ATTR_CERT         4
     */

    public enum CredentialQueryAttrib : uint
    {
        Names = 1,
        SsiProvider = 2,
        KdcProxySettings = 3,
        Cert = 4
    }
}
