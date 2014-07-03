using System;
using System.Text;
using NSspi.Contexts;
using NSspi.Credentials;

namespace NSspi
{
    public class Program
    {
        public static void Main( string[] args )
        {
            CredTest( PackageNames.Negotiate );
            CredTest( PackageNames.Kerberos );
            CredTest( PackageNames.Ntlm );
        }

        private static void CredTest( string packageName )
        {
            ClientCredential clientCred = null;
            ClientContext client = null;

            ServerCredential serverCred = null;
            ServerContext server = null;

            byte[] clientToken;
            byte[] serverToken;

            SecurityStatus clientStatus;
            SecurityStatus serverStatus;

            try
            {
                clientCred = new ClientCredential( packageName );
                serverCred = new ServerCredential( packageName );

                Console.Out.WriteLine( clientCred.PrincipleName );

                client = new ClientContext( 
                    clientCred, 
                    serverCred.PrincipleName, 
                    ContextAttrib.MutualAuth | 
                    ContextAttrib.InitIdentify |
                    ContextAttrib.Confidentiality |
                    ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect | 
                    ContextAttrib.Connection |
                    ContextAttrib.Delegate
                );


                server = new ServerContext(
                    serverCred,
                    ContextAttrib.MutualAuth |
                    ContextAttrib.AcceptIdentify |
                    ContextAttrib.Confidentiality |
                    ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect |
                    ContextAttrib.Connection |
                    ContextAttrib.Delegate
                );

                clientToken = null;
                serverToken = null;

                clientStatus = client.Init( serverToken, out clientToken );

                while ( true )
                {
                    serverStatus = server.AcceptToken( clientToken, out serverToken );

                    if ( serverStatus != SecurityStatus.ContinueNeeded && clientStatus != SecurityStatus.ContinueNeeded ) { break; }

                    clientStatus = client.Init( serverToken, out clientToken );

                    if ( serverStatus != SecurityStatus.ContinueNeeded && clientStatus != SecurityStatus.ContinueNeeded ) { break; }
                }


                Console.Out.WriteLine( "Server authority: " + server.AuthorityName );
                Console.Out.WriteLine( "Server context user: " + server.ContextUserName );

                Console.Out.WriteLine();

                Console.Out.WriteLine( "Client authority: " + client.AuthorityName );
                Console.Out.WriteLine( "Client context user: " + client.ContextUserName );

                string message = "Hello, world. This is a long message that will be encrypted";
                string rtMessage;

                byte[] plainText = new byte[Encoding.UTF8.GetByteCount( message )];
                byte[] cipherText;
                byte[] roundTripPlaintext;

                Encoding.UTF8.GetBytes( message, 0, message.Length, plainText, 0 );

                cipherText = client.Encrypt( plainText );

                roundTripPlaintext = server.Decrypt( cipherText );

                if( roundTripPlaintext.Length != plainText.Length )
                {
                    throw new Exception();
                }

                for( int i= 0; i < plainText.Length; i++ )
                {
                    if( plainText[i] != roundTripPlaintext[i] )
                    {
                        throw new Exception();
                    }
                }

                rtMessage = Encoding.UTF8.GetString( roundTripPlaintext, 0, roundTripPlaintext.Length );

                if( rtMessage.Equals( message ) == false )
                {
                    throw new Exception();
                }


                using( server.ImpersonateClient() )
                {

                }

                cipherText = client.MakeSignature( plainText );

                bool goodSig = server.VerifySignature( cipherText, out roundTripPlaintext );

                if ( goodSig == false ||
                     roundTripPlaintext.Length != plainText.Length )
                {
                    throw new Exception();
                }
                
                for ( int i = 0; i < plainText.Length; i++ )
                {
                    if ( plainText[i] != roundTripPlaintext[i] )
                    {
                        throw new Exception();
                    }
                }

                Console.Out.Flush();
            }
            finally
            {
                if ( server != null )
                {
                    server.Dispose();
                }

                if ( client != null )
                {
                    client.Dispose();
                }

                if( clientCred != null )
                {
                    clientCred.Dispose();
                }

                if ( serverCred != null )
                {
                    serverCred.Dispose();
                }
            }
        }
    }
}
