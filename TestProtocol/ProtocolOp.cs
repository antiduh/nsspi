using System;

namespace TestProtocol
{
    public enum ProtocolOp : uint
    {
        ClientToken = 1,
        ServerToken = 2,

        EncryptedMessage = 3,
        SignedMessage = 4,
    }
}