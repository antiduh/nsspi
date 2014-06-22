using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    public enum SecureBufferDataRep : int
    {
        /*
        #define SECURITY_NATIVE_DREP        0x00000010
        #define SECURITY_NETWORK_DREP       0x00000000
        */
        Nativee = 0x10,
        Network = 0x00
    }
}
