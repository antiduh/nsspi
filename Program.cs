using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    public class Program
    {
        public static void Main( string[] args )
        {
            Credential cred = new Credential( SecurityPackage.Negotiate, CredentialType.Client );
            cred.Dispose();

        }
    }
}
