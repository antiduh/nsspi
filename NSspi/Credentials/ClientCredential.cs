using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    public class ClientCredential : CurrentCredential
    {
        public ClientCredential( string package )
            : base( package, CredentialUse.Outbound )
        {
        }
    }
}
