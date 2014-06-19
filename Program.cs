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
            Credential cred = null;
            try
            {
                cred = new Credential( SecurityPackage.Negotiate, CredentialType.Client );

                string name = cred.GetName();
                Console.Out.WriteLine( name );
                Console.Out.Flush();
            }
            finally
            {
                if ( cred != null )
                {
                    cred.Dispose();
                }
            }
        }
    }
}
