using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    /// <summary>
    /// Defines the types of queries that can be performed with QueryContextAttribute.
    /// Each query has a different result buffer.
    /// </summary>
    internal enum ContextQueryAttrib : int
    {
        /// <summary>
        /// Queries the buffer size parameters when performing message functions, such
        /// as encryption, decryption, signing and signature validation.
        /// </summary>
        /// <remarks>
        /// Results for a query of this type are stored in a Win32 SecPkgContext_Sizes structure.
        /// </remarks>
        Sizes = 0,

        /// <summary>
        /// Queries the context for the name of the user assocated with a security context.
        /// </summary>
        /// <remarks>
        /// Results for a query of this type are stored in a Win32 SecPkgContext_Name structure.
        /// </remarks>
        Names = 1,

        /// <summary>
        /// Queries the name of the authenticating authority for the security context.
        /// </summary>
        /// <remarks>
        /// Results for a query of this type are stored in a Win32 SecPkgContext_Authority structure.
        /// </remarks>
        Authority = 6,
    }
}
