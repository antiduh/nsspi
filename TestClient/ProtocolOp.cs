using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public enum ProtocolOp : uint
    {
        ClientToken = 1,
        ServerToken = 2,

        EncryptedMessage = 3,
        SignedMessage = 4,
    }
}
