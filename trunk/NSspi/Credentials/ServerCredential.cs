using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Credentials
{
    public class ServerCredential : Credential
    {
        public ServerCredential( SecurityPackage package ) : base( package, CredentialType.Server ) { }
    }
}
