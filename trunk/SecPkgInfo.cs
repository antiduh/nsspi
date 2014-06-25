using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSspi
{
    [StructLayout( LayoutKind.Sequential )]
    public class SecPkgInfo
    {
        public SecPkgCapability Capabilities;

        public short Version;

        public short RpcId;

        public int MaxTokenLength;

        [MarshalAs( UnmanagedType.LPWStr )]
        public string Name;

        [MarshalAs( UnmanagedType.LPWStr )]
        public string Comment;
    }

    [Flags]
    public enum SecPkgCapability : uint 
    {
        Integrity       = 0x1,

        Privacy         = 0x2,

        TokenOnly       = 0x4,

        Datagram        = 0x8,

        Connection      = 0x10,

        MultiLeg        = 0x20,

        ClientOnly      = 0x40,

        ExtendedError   = 0x80,

        Impersonation   = 0x100,

        AcceptWin32Name = 0x200,

        Stream          = 0x400,

        Negotiable      = 0x800,

        GssCompatible   = 0x1000,

        Logon           = 0x2000,

        AsciiBuffers    = 0x4000,

        Fragment        = 0x8000,

        MutualAuth      = 0x10000,

        Delegation      = 0x20000,

        ReadOnlyChecksum = 0x40000,

        RestrictedTokens = 0x80000,

        ExtendsNego     = 0x00100000,

        Negotiable2     = 0x00200000,
    }
}
