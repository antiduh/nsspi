using System;

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