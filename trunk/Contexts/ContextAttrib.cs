using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    /// <summary>
    /// Defines options for creating a security context via win32 InitializeSecurityContext 
    /// (used by clients) and AcceptSecurityContext (used by servers).
    /// Required attribute flags are specified when creating the context. InitializeSecurityContext
    /// and AcceptSecurityContext returns a value indicating what final attributes the created context 
    /// actually has.
    /// </summary>
    [Flags]
    public enum ContextAttrib : int 
    {
        /// <summary>
        /// No additional attributes are provided.
        /// </summary>
        Zero = 0,
        
        /// <summary>
        /// The server can use the context to authenticate to other servers as the client. The
        /// MutualAuth flag must be set for this flag to work. Valid for Kerberos. Ignore this flag for 
        /// constrained delegation, (TODO)(which is handled through a separate mechanism?).
        /// </summary>
        Delegate = 0x00000001,

        /// <summary>
        /// The mutual authentication policy of the service will be satisfied.
        /// *Caution* - This does not necessarily mean that mutual authentication is performed, only that
        /// the authentication policy of the service is satisfied. To ensure that mutual authentication is
        /// performed, query the context attributes after it is created.
        /// </summary>
        MutualAuth = 0x00000002,


        /// <summary>
        /// Detect replayed messages that have been encoded by using the EncryptMessage or MakeSignature 
        /// message support functionality.
        /// </summary>
        ReplayDetect = 0x00000004,

        // The context must be allowed to detect out-of-order
        // delivery of packets later through the message support
        // functions. Use of this flag implies all of the
        // conditions specified by the Integrity flag.

        /// <summary>
        /// Detect messages received out of sequence when using the message support functionality. 
        /// This flag implies all of the conditions specified by the Integrity flag - out-of-order sequence 
        /// detection can only be trusted if the integrity of any underlying sequence detection mechanism 
        /// in transmitted data can be trusted.
        /// </summary>
        SequenceDetect = 0x00000008,

        // The context must protect data while in transit.
        // Confidentiality is supported for NTLM with Microsoft
        // Windows NT version 4.0, SP4 and later and with the
        // Kerberos protocol in Microsoft Windows 2000 and later.
        
        /// <summary>
        /// The context must protect data while in transit. Encrypt messages by using the EncryptMessage function.
        /// </summary>
        Confidentiality = 0x00000010,
        
        /// <summary>
        /// A new session key must be negotiated.
        /// This value is supported only by the Kerberos security package.
        /// </summary>
        UseSessionKey = 0x00000020,

        /// <summary>
        /// The security package allocates output buffers for you. Buffers allocated by the security package have 
        /// to be released by the context memory management functions.
        /// </summary>
        AllocateMemory = 0x00000100,

        /// <summary>
        /// The security context will not handle formatting messages. This value is the default for the Kerberos, 
        /// Negotiate, and NTLM security packages.
        /// </summary>
        Connection = 0x00000800,

        /// <summary>
        /// When errors occur, the remote party will be notified.
        /// </summary>
        /// <remarks>
        /// A client specifies InitExtendedError in InitializeSecurityContext
        /// and the server specifies AcceptExtendedError in AcceptSecurityContext. 
        /// </remarks>
        InitExtendedError = 0x00004000,

        /// <summary>
        /// When errors occur, the remote party will be notified.
        /// </summary>
        /// <remarks>
        /// A client specifies InitExtendedError in InitializeSecurityContext
        /// and the server specifies AcceptExtendedError in AcceptSecurityContext. 
        /// </remarks>
        AcceptExtendedError = 0x00008000,

        /// <summary>
        /// Support a stream-oriented connection. Provided by clients.
        /// </summary>
        InitStream = 0x00008000,

        /// <summary>
        /// Support a stream-oriented connection. Provided by servers.
        /// </summary>
        AcceptStream = 0x00010000,

        /// <summary>
        /// Sign messages and verify signatures by using the EncryptMessage and MakeSignature functions.
        /// Replayed and out-of-sequence messages will not be detected with the setting of this attribute.
        /// Set ReplayDetect and SequenceDetect also if these behaviors are desired.
        /// </summary>
        InitIntegrity = 0x00010000, 

        /// <summary>
        /// Sign messages and verify signatures by using the EncryptMessage and MakeSignature functions.
        /// Replayed and out-of-sequence messages will not be detected with the setting of this attribute.
        /// Set ReplayDetect and SequenceDetect also if these behaviors are desired.
        /// </summary>
        AcceptIntegrity = 0x00020000,

        /// <summary>
        /// An Schannel provider connection is instructed to not authenticate the server automatically.
        /// </summary>
        InitManualCredValidation = 0x00080000,

        /// <summary>
        /// An Schannel provider connection is instructed to not authenticate the client automatically.
        /// </summary>
        InitUseSuppliedCreds = 0x00000080,
    }
}
