using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    /// <summary>
    /// Identifies credential query types.
    /// </summary>
    public enum CredentialQueryAttrib : uint
    {
        /// <summary>
        /// Queries the credential's principle name.
        /// </summary>
        Names = 1,
    }
}
