using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    public enum CredentialUse : uint
    {
        Inbound = 1,
        Outbound = 2,
        Both = 3,
    }
}
