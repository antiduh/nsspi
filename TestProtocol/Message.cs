using System;

namespace TestProtocol
{
    public class Message
    {
        public Message( ProtocolOp op, byte[] data )
        {
            this.Operation = op;
            this.Data = data;
        }

        public ProtocolOp Operation { get; private set; }

        public byte[] Data { get; private set; }
    }
}