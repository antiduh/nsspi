using System;

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
        [EnumString( "No error" )]
        OK = 0x00000000,

        /// <summary>
        /// The token returned by the context needs to be provided to the cooperating party
        /// to continue construction of the context.
        /// </summary>
        [EnumString( "Authentication cycle needs to continue" )]
        ContinueNeeded = 0x00090312,

        /// <summary>
        /// Occurs after a client calls InitializeSecurityContext to indicate that the client
        /// must call CompleteAuthToken.
        /// </summary>
        [EnumString( "Authentication cycle needs to perform a 'complete'." )]
        CompleteNeeded = 0x00090313,

        /// <summary>
        /// Occurs after a client calls InitializeSecurityContext to indicate that the client
        /// must call CompleteAuthToken and pass the result to the server.
        /// </summary>
        [EnumString( "Authentication cycle needs to perform a 'complete' and then continue." )]
        CompAndContinue = 0x00090314,

        /// <summary>
        /// An attempt to use the context was performed after the context's expiration time elapsed.
        /// </summary>
        [EnumString( "The security context was used after its expiration time passed." )]
        ContextExpired = 0x00090317,

        /// <summary>
        /// The credentials supplied to the security context were not fully initialized.
        /// </summary>
        [EnumString( "The credentials supplied to the security context were not fully initialized." )]
        CredentialsNeeded = 0x00090320,

        /// <summary>
        /// The context data must be re-negotiated with the peer.
        /// </summary>
        [EnumString( "The context data must be re-negotiated with the peer." )]
        Renegotiate = 0x00090321,

        // -------------- Errors --------------

        /// <summary>
        /// The SSPI operation failed due to insufficient memory resources.
        /// </summary>
        [EnumString( "Not enough memory." )]
        OutOfMemory = 0x80090300,

        /// <summary>
        /// The handle provided to the API was invalid.
        /// </summary>
        [EnumString( "The handle provided to the API was invalid." )]
        InvalidHandle = 0x80090301,

        /// <summary>
        /// The attempted operation is not supported.
        /// </summary>
        [EnumString( "The attempted operation is not supported." )]
        Unsupported = 0x80090302,

        /// <summary>
        /// The specified principle is not known in the authentication system.
        /// </summary>
        [EnumString( "The specified principle is not known in the authentication system." )]
        TargetUnknown = 0x80090303,

        /// <summary>
        /// An internal error occurred
        /// </summary>
        [EnumString( "An internal error occurred." )]
        InternalError = 0x80090304,

        /// <summary>
        /// No security provider package was found with the given name.
        /// </summary>
        [EnumString( "The requested security package was not found." )]
        PackageNotFound = 0x80090305,

        /// <summary>
        /// Cannot use the provided credentials, the caller is not the owner of the credentials.
        /// </summary>
        [EnumString( "The caller is not the owner of the desired credentials." )]
        NotOwner = 0x80090306,

        /// <summary>
        /// The requested security package failed to initalize, and thus cannot be used.
        /// </summary>
        [EnumString( "The requested security package failed to initalize, and thus cannot be used." )]
        CannotInstall = 0x80090307,

        /// <summary>
        /// A token was provided that contained incorrect or corrupted data.
        /// </summary>
        [EnumString( "The provided authentication token is invalid or corrupted." )]
        InvalidToken = 0x80090308,

        /// <summary>
        /// The security package is not able to marshall the logon buffer, so the logon attempt has failed
        /// </summary>
        [EnumString( "The security package is not able to marshall the logon buffer, so the logon attempt has failed." )]
        CannotPack = 0x80090309,

        /// <summary>
        /// The per-message Quality of Protection is not supported by the security package.
        /// </summary>
        [EnumString( "The per-message Quality of Protection is not supported by the security package." )]
        QopNotSupported = 0x8009030A,

        /// <summary>
        /// Impersonation is not supported.
        /// </summary>
        [EnumString( "Impersonation is not supported with the current security package." )]
        NoImpersonation = 0x8009030B,

        /// <summary>
        /// The logon was denied, perhaps because the provided credentials were incorrect.
        /// </summary>
        [EnumString( "The logon was denied, perhaps because the provided credentials were incorrect." )]
        LogonDenied = 0x8009030C,

        /// <summary>
        /// The credentials provided are not recognized by the selected security package.
        /// </summary>
        [EnumString( "The credentials provided are not recognized by the selected security package." )]
        UnknownCredentials = 0x8009030D,

        /// <summary>
        /// No credentials are available in the selected security package.
        /// </summary>
        [EnumString( "No credentials are available in the selected security package." )]
        NoCredentials = 0x8009030E,

        /// <summary>
        /// A message that was provided to the Decrypt or VerifySignature functions was altered after
        /// it was created.
        /// </summary>
        [EnumString( "A message that was provided to the Decrypt or VerifySignature functions was altered " +
            "after it was created." )]
        MessageAltered = 0x8009030F,

        /// <summary>
        /// A message was received out of the expected order.
        /// </summary>
        [EnumString( "A message was received out of the expected order." )]
        OutOfSequence = 0x80090310,

        /// <summary>
        /// The current security package cannot contact an authenticating authority.
        /// </summary>
        [EnumString( "The current security package cannot contact an authenticating authority." )]
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
        [EnumString( "The buffer provided to an SSPI API call contained a message that was not complete." )]
        IncompleteMessage = 0x80090318,

        /// <summary>
        /// The credentials supplied were not complete, and could not be verified. The context could not be initialized.
        /// </summary>
        [EnumString( "The credentials supplied were not complete, and could not be verified. The context could not be initialized." )]
        IncompleteCredentials = 0x80090320,

        /// <summary>
        /// The buffers supplied to a security function were too small.
        /// </summary>
        [EnumString( "The buffers supplied to a security function were too small." )]
        BufferNotEnough = 0x80090321,

        /// <summary>
        /// The target principal name is incorrect.
        /// </summary>
        [EnumString( "The target principal name is incorrect." )]
        WrongPrincipal = 0x80090322,

        /// <summary>
        /// The clocks on the client and server machines are skewed.
        /// </summary>
        [EnumString( "The clocks on the client and server machines are skewed." )]
        TimeSkew = 0x80090324,

        /// <summary>
        /// The certificate chain was issued by an authority that is not trusted.
        /// </summary>
        [EnumString( "The certificate chain was issued by an authority that is not trusted." )]
        UntrustedRoot = 0x80090325,

        /// <summary>
        /// The message received was unexpected or badly formatted.
        /// </summary>
        [EnumString( "The message received was unexpected or badly formatted." )]
        IllegalMessage = 0x80090326,

        /// <summary>
        /// An unknown error occurred while processing the certificate.
        /// </summary>
        [EnumString( "An unknown error occurred while processing the certificate." )]
        CertUnknown = 0x80090327,

        /// <summary>
        /// The received certificate has expired.
        /// </summary>
        [EnumString( "The received certificate has expired." )]
        CertExpired = 0x80090328,

        /// <summary>
        /// The client and server cannot communicate, because they do not possess a common algorithm.
        /// </summary>
        [EnumString( "The client and server cannot communicate, because they do not possess a common algorithm." )]
        AlgorithmMismatch = 0x80090331,

        /// <summary>
        /// The security context could not be established due to a failure in the requested quality
        /// of service (e.g. mutual authentication or delegation).
        /// </summary>
        [EnumString( "The security context could not be established due to a failure in the requested " + 
            "quality of service (e.g. mutual authentication or delegation)." )]
        SecurityQosFailed = 0x80090332,

        /// <summary>
        /// Smartcard logon is required and was not used.
        /// </summary>
        [EnumString( "Smartcard logon is required and was not used." )]
        SmartcardLogonRequired = 0x8009033E,

        /// <summary>
        /// An unsupported preauthentication mechanism was presented to the Kerberos package.
        /// </summary>
        [EnumString( "An unsupported preauthentication mechanism was presented to the Kerberos package." )]
        UnsupportedPreauth = 0x80090343,

        /// <summary>
        /// Client's supplied SSPI channel bindings were incorrect.
        /// </summary>
        [EnumString( "Client's supplied SSPI channel bindings were incorrect." )]
        BadBinding = 0x80090346
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