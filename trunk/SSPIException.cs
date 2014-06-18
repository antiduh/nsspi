using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    [Serializable]
    public class SSPIException : Exception
    {
        private int errorCode;
        private string message;

        public SSPIException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
            this.message = info.GetString( "messsage" );
            this.errorCode = info.GetInt32( "errorCode" );
        }

        public SSPIException( string message, int errorCode )
        {
            this.message = message;
            this.errorCode = errorCode;
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );

            info.AddValue( "message", this.message );
            info.AddValue( "errorCode", this.errorCode );
        }

        public int ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        public override string Message
        {
            get
            {
                return string.Format( "{0}. Error Code = '{1:X}'.", this.message, this.errorCode );
            }
        }
    }
}
