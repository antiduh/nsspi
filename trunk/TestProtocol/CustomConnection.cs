using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NSspi;

namespace TestProtocol
{
    public class CustomConnection
    {
        private Thread receiveThread;
        
        private Socket socket;

        private bool running;

        public CustomConnection()
        {
            this.running = false;
        }

        public delegate void ReceivedAction( Message message );
        
        public event ReceivedAction Received;

        public event Action Disconnected;

        public void StartClient( string server, int port )
        {
            if( this.running )
            {
                throw new InvalidOperationException("Already running");
            }

            this.socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            this.socket.Connect( server, port );

            this.running = true;

            this.receiveThread = new Thread( ReceiveThreadEntry );
            this.receiveThread.Name = "SSPI Client Receive Thread";
            this.receiveThread.Start();
        }

        public void Stop()
        {
            if( this.running == false )
            {
                return;
            }

            this.socket.Close();
            this.receiveThread.Join();
        }

        public void Send( Message message )
        {
            if( this.running == false )
            {
                throw new InvalidOperationException( "Not connected" );
            }

            byte[] outBuffer = new byte[ message.Data.Length + 8 ];
            int position = 0;

            ByteWriter.WriteInt32_BE( (int)message.Operation, outBuffer, position );
            position += 4;

            ByteWriter.WriteInt32_BE( message.Data.Length, outBuffer, position );
            position += 4;

            Array.Copy( message.Data, 0, outBuffer, position, message.Data.Length );

            this.socket.Send( outBuffer, 0, outBuffer.Length, SocketFlags.None );

            Console.Out.WriteLine( "Client: Sent " + message.Operation );
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
            }
            finally
            {
                this.running = false;

                try
                {
                    if( this.Disconnected != null )
                    {
                        this.Disconnected();
                    }
                }
                catch
                { }
            }
        }

        private void ReadLoop()
        {
            byte[] readBuffer = new byte[65536];

            ProtocolOp operation;
            int messageLength;
            int remaining;
            int chunkLength;
            int position;

            while( this.running )
            {
                try
                {
                    //                          |--4 bytes--|--4 bytes--|---N--|
                    // Every command is a TLV - | Operation |  Length   | Data |


                    // Read the operation.
                    this.socket.Receive( readBuffer, 4, SocketFlags.None );

                    // Check if we popped out of a receive call after we were shut down.
                    if( this.running == false ) { break; }
                    
                    operation = (ProtocolOp)ByteWriter.ReadInt32_BE( readBuffer, 0 );

                    // Read the length 
                    this.socket.Receive( readBuffer, 4, SocketFlags.None );
                    messageLength = ByteWriter.ReadInt32_BE( readBuffer, 0 );

                    if( readBuffer.Length < messageLength )
                    {
                        readBuffer = new byte[messageLength];
                    }

                    // Read the data
                    // Keep in mind that Socket.Receive may return less data than asked for.
                    remaining = messageLength;
                    chunkLength = 0;
                    position = 0;
                    while( remaining > 0 )
                    {
                        chunkLength = this.socket.Receive( readBuffer, position, remaining, SocketFlags.None );
                        remaining -= chunkLength;
                        position += chunkLength;
                    }

                }
                catch( SocketException e )
                {
                    if( e.SocketErrorCode == SocketError.ConnectionAborted ||
                        e.SocketErrorCode == SocketError.Interrupted ||
                        e.SocketErrorCode == SocketError.OperationAborted ||
                        e.SocketErrorCode == SocketError.Shutdown || 
                        e.SocketErrorCode == SocketError.ConnectionReset )
                    {
                        // Shutting down.
                        break;
                    }
                    else
                    {
                        throw;
                    }
                }

                Console.Out.WriteLine( "Client: Received " + operation );

                if( this.Received != null )
                {
                    byte[] dataCopy = new byte[messageLength];
                    Array.Copy( readBuffer, 0, dataCopy, 0, messageLength );
                    Message message = new Message( operation, dataCopy );
                 
                    try
                    {
                        this.Received( message );
                    }
                    catch( Exception e )
                    { }
                }
                
            }
        }
    }
}
