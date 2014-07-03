using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    /// <summary>
    /// Provides canonical names for security pacakges.
    /// </summary>
    public static class PackageNames
    {
        /// <summary>
        /// Indicates the Negotiate security package.
        /// </summary>
        public const string Negotiate = "Negotiate";

        /// <summary>
        /// Indicates the Kerberos security package.
        /// </summary>
        public const string Kerberos = "Kerberos";

        /// <summary>
        /// Indicates the NTLM security package.
        /// </summary>
        public const string Ntlm = "NTLM";
    }
}
