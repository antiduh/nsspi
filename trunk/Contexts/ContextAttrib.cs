using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSspi.Contexts
{
    [Flags]
    public enum ContextReqAttrib : int 
    {
        None = 0,
        Delegate = 1,
        Identify = 2,
        MutualAuth = 4,
    }

    public enum ContextResultAttrib : int
    {
    }
}
