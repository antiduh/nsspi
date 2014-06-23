using NSspi.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NSspi
{
    public class Program
    {
        public static void Main( string[] args )
        {
            CredTest();
        }

        private static void IdentTest()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent( TokenAccessLevels.MaximumAllowed );
            Stream stream = new MemoryStream();
            StringWriter writer = new StringWriter();

            ISerializable serializable = current;
            SerializationInfo info = new SerializationInfo( current.GetType(), new FormatterConverter() );
            StreamingContext streamingContext = new StreamingContext();

            serializable.GetObjectData( info, streamingContext );


            WindowsIdentity newId = new WindowsIdentity( info, streamingContext );
        }



        private static void CredTest()
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
                clientCred = new ClientCredential( SecurityPackage.Negotiate );
                Console.Out.WriteLine( clientCred.Name );

                client = new ClientContext( 
                    clientCred, 
                    "", 
                    ContextAttrib.MutualAuth | 
                    ContextAttrib.InitIdentify |
                    ContextAttrib.Confidentiality |
                    ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect
                );

                serverCred = new ServerCredential( SecurityPackage.Negotiate );

                server = new ServerContext(
                    serverCred,
                    ContextAttrib.MutualAuth |
                    ContextAttrib.AcceptIdentify |
                    ContextAttrib.Confidentiality |
                    ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect
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
