using System;
using System.Runtime.Serialization;

namespace NSspi
{
    /// <summary>
    /// The exception that is thrown when a problem occurs hwen using the SSPI system.
    /// </summary>
    [Serializable]
    public class SSPIException : Exception
    {
        private SecurityStatus errorCode;
        private string message;

        /// <summary>
        /// Initializes a new instance of the SSPIException class with the given message and status.
        /// </summary>
        /// <param name="message">A message explaining what part of the system failed.</param>
        /// <param name="errorCode">The error code observed during the failure.</param>
        public SSPIException( string message, SecurityStatus errorCode )
        {
            this.message = message;
            this.errorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the SSPIException class from serialization data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SSPIException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
            this.message = info.GetString( "message" );
            this.errorCode = (SecurityStatus)info.GetUInt32( "errorCode" );
        }

        /// <summary>
        /// Serializes the exception.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );

            info.AddValue( "message", this.message );
            info.AddValue( "errorCode", this.errorCode );
        }

        /// <summary>
        /// The error code that was observed during the SSPI call.
        /// </summary>
        public SecurityStatus ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        /// <summary>
        /// A human-readable message indicating the nature of the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format(
                    "{0}. Error Code = '0x{1:X}' - \"{2}\".",
                    this.message,
                    this.errorCode,
                    EnumMgr.ToText( this.errorCode )
                );
            }
        }
    }
}