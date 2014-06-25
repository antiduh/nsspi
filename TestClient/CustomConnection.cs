using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NSspi;

namespace TestClient
{
    public class CustomConnection
    {
        private string server;
        private int port;

        private Thread receiveThread;
        
        private Socket socket;

        private bool running;

        public CustomConnection( string server, int port )
        {
            this.running = false;
        }

        public delegate void ReceivedAction( Message message );
        
        public event ReceivedAction Received;

        public void StartClient()
        {
            if( this.running )
            {
                throw new InvalidOperationException("Already running");
            }

            this.socket = new Socket( SocketType.Stream, ProtocolType.Tcp );

            this.socket.Connect( this.server, this.port );

            this.running = true;

            this.receiveThread = new Thread( ReceiveThreadEntry );
            this.receiveThread.Name = "SSPI Client Receive Thread";
            this.receiveThread.Start();
        }

        public void Stop()
        {
            if( this.running == false )
            {
                throw new InvalidOperationException( "Already stopped" );
            }
        }

        public void Send( byte[] buffer, int start, int length )
        {
            if( this.running == false )
            {
                throw new InvalidOperationException( "Not connected" );
            }

            this.socket.Send( buffer, start, length, SocketFlags.None );
        }

        private void ReceiveThreadEntry()
        {
            try
            {
                ReadLoop();
            }
            catch( Exception e )
            {
                MessageBox.Show( "The SspiConnection receive thread crashed:\r\n\r\n" + e.ToString() );
                this.running = false;
            }
        }

        private void ReadLoop()
        {
            byte[] readBuffer = new byte[65536];

            ProtocolOp operation;
            int length;

            while( this.running )
            {
                try
                {
                    //                          |--4 bytes--|--4 bytes--|---N--|
                    // Every command is a TLV - | Operation |  Length   | Data |

                    // Read the operation.
                    this.socket.Receive( readBuffer, 4, SocketFlags.None );

                    operation = (ProtocolOp)ByteWriter.ReadInt32_BE( readBuffer, 0 );

                    // Read the length 
                    this.socket.Receive( readBuffer, 4, SocketFlags.None );
                    length = ByteWriter.ReadInt32_BE( readBuffer, 0 );

                    // Read the data
                    this.socket.Receive( readBuffer, length, SocketFlags.None );

                }
                catch( SocketException e )
                {
                    if( e.SocketErrorCode == SocketError.ConnectionAborted ||
                        e.SocketErrorCode == SocketError.Interrupted ||
                        e.SocketErrorCode == SocketError.OperationAborted ||
                        e.SocketErrorCode == SocketError.Shutdown )
                    {
                        // Shutting down.
                        break;
                    }
                    else
                    {
                        throw;
                    }
                }

                try
                {
                    if( this.Received != null )
                    {
                        byte[] dataCopy = new byte[length];
                        Array.Copy( readBuffer, 0, dataCopy, 0, length );
                        Message message = new Message( operation, dataCopy );

                        this.Received( message );
                    }
                }
                catch( Exception e )
                {

                }
            }
        }
    }
}
