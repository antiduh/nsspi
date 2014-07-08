using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    /*
    // From winerror.h 
    #define SEC_E_OK                         ((HRESULT)0x00000000L)
    #define SEC_E_INSUFFICIENT_MEMORY        _HRESULT_TYPEDEF_(0x80090300L)
    #define SEC_E_INVALID_HANDLE             _HRESULT_TYPEDEF_(0x80090301L)
    #define SEC_E_UNSUPPORTED_FUNCTION       _HRESULT_TYPEDEF_(0x80090302L)
    #define SEC_E_TARGET_UNKNOWN             _HRESULT_TYPEDEF_(0x80090303L)
    #define SEC_E_INTERNAL_ERROR             _HRESULT_TYPEDEF_(0x80090304L)
    #define SEC_E_SECPKG_NOT_FOUND           _HRESULT_TYPEDEF_(0x80090305L)
    #define SEC_E_NOT_OWNER                  _HRESULT_TYPEDEF_(0x80090306L)
    #define SEC_E_UNKNOWN_CREDENTIALS        _HRESULT_TYPEDEF_(0x8009030DL)
    #define SEC_E_NO_CREDENTIALS             _HRESULT_TYPEDEF_(0x8009030EL)
    */

    /// <summary>
    /// Defines the results of invoking the SSPI api.
    /// </summary>
    public enum SecurityStatus : uint
    {
        // --- Success / Informational ---

        /// <summary>
        /// The request completed successfully
        /// </summary>
        OK                  = 0x00000000,
        
        /// <summary>
        /// The token returned by the context needs to be provided to the cooperating party
        /// to continue construction of the context.
        /// </summary>
        ContinueNeeded      = 0x00090312,

        /// <summary>
        /// Occurs after a client calls InitializeSecurityContext to indicate that the client
        /// must call CompleteAuthToken.
        /// </summary>
        CompleteNeeded      = 0x00090313,

        /// <summary>
        /// Occurs after a client calls InitializeSecurityContext to indicate that the client
        /// must call CompleteAuthToken and pass the result to the server.
        /// </summary>
        CompAndContinue     = 0x00090314,

        /// <summary>
        /// An attempt to use the context was performed after the context's expiration time elapsed.
        /// </summary>
        ContextExpired      = 0x00090317,
        CredentialsNeeded   = 0x00090320,
        Renegotiate         = 0x00090321,

        // Errors
        OutOfMemory         = 0x80090300,
        InvalidHandle       = 0x80090301,
        Unsupported         = 0x80090302,
        TargetUnknown       = 0x80090303,
        InternalError       = 0x80090304,

        /// <summary>
        /// No security provider package was found with the given name.
        /// </summary>
        PackageNotFound     = 0x80090305,

        NotOwner            = 0x80090306,
        CannotInstall       = 0x80090307,
        InvalidToken        = 0x80090308,
        CannotPack          = 0x80090309,
        QopNotSupported     = 0x8009030A,
        NoImpersonation     = 0x8009030B,
        LogonDenied         = 0x8009030C,
        UnknownCredentials  = 0x8009030D,
        NoCredentials       = 0x8009030E,
        MessageAltered      = 0x8009030F,
        OutOfSequence       = 0x80090310,
        NoAuthenticatingAuthority = 0x80090311,

        /// <summary>
        /// The buffer provided to an SSPI API call contained a message that was not complete.
        /// </summary>
        /// <remarks>
        /// This occurs regularly with SSPI contexts that exchange data using a streaming context,
        /// where the data returned from the streaming communications channel, such as a TCP socket,
        /// did not contain the complete message. 
        /// Similarly, a streaming channel may return too much data, in which case the API function
        /// will indicate success, but will save off the extra, unrelated data in a buffer of
        /// type 'extra'.
        /// </remarks>
        IncompleteMessage   = 0x80090318,
        IncompleteCredentials = 0x80090320,
        BufferNotEnough     = 0x80090321,
        WrongPrincipal      = 0x80090322,
        TimeSkew            = 0x80090324,
        UntrustedRoot       = 0x80090325,
        IllegalMessage      = 0x80090326,
        CertUnknown         = 0x80090327,
        CertExpired         = 0x80090328,
        AlgorithmMismatch   = 0x80090331,
        SecurityQosFailed   = 0x80090332,
        SmartcardLogonRequired = 0x8009033E,
        UnsupportedPreauth  = 0x80090343,
        BadBinding          = 0x80090346
    }

    /// <summary>
    /// Provides extension methods for the SecurityStatus enumeration.
    /// </summary>
    public static class SecurityStatusExtensions
    {
        /// <summary>
        /// Returns whether or not the status represents an error.
        /// </summary>
        /// <param name="status"></param>
        /// <returns>True if the status represents an error condition.</returns>
        public static bool IsError( this SecurityStatus status )
        {
            return (uint)status > 0x80000000u;
        }
    }

}
