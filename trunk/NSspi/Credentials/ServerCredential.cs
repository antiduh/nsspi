using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    public class ServerCredential : CurrentCredential
    {
        public ServerCredential( string package )
            : base( package, CredentialUse.Inbound )
        {
        }
    }
}
