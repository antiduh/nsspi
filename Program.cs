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
            ClientCredential cred = null;
            ClientContext client;
            try
            {
                cred = new ClientCredential( SecurityPackage.Negotiate );
                Console.Out.WriteLine( cred.Name );

                client = new ClientContext( 
                    cred, 
                    "", 
                    ContextAttrib.MutualAuth | 
                    ContextAttrib.InitIdentify |
                    ContextAttrib.Confidentiality |
                    ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect
                );
                
                Console.Out.Flush();
            }
            finally
            {
                if( cred != null )
                {
                    cred.Dispose();
                }
            }
        }
    }
}
