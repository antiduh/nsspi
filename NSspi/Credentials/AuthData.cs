using System;
using System.Runtime.InteropServices;

namespace NSspi.Credentials
{
    /// <summary>
    /// Provides authentication data in native method calls.
    /// </summary>
    /// <remarks>
    /// Implements the 'SEC_WINNT_AUTH_IDENTITY' structure. See:
    ///
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa380131(v=vs.85).aspx
    /// </remarks>
    [StructLayout( LayoutKind.Sequential )]
    internal struct NativeAuthData
    {
        public NativeAuthData( string domain, string username, string password, NativeAuthDataFlag flag )
        {
            this.Domain = domain;
            this.DomainLength = domain.Length;

            this.User = username;
            this.UserLength = username.Length;

            this.Password = password;
            this.PasswordLength = password.Length;

            this.Flags = flag;
        }

        [MarshalAs( UnmanagedType.LPWStr )]
        public string User;

        public int UserLength;

        [MarshalAs( UnmanagedType.LPWStr )]
        public string Domain;

        public int DomainLength;

        [MarshalAs( UnmanagedType.LPWStr )]
        public string Password;

        public int PasswordLength;

        public NativeAuthDataFlag Flags;
    }

    internal enum NativeAuthDataFlag : int
    {
        Ansi = 1,

        Unicode = 2
    }
}