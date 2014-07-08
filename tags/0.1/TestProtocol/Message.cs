using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProtocol
{
    public class Message
    {
        public Message(ProtocolOp op, byte[] data)
        {
            this.Operation = op;
            this.Data = data;
        }

        public ProtocolOp Operation { get; private set; }

        public byte[] Data { get; private set; }
    }
}
