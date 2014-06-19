using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public class ClientCredential : Credential
    {
        public ClientCredential( SecurityPackage package ) : base( package, CredentialType.Client ) { }
    }
}
