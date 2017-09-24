using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using NSspi;
using NSspi.Contexts;
using NSspi.Credentials;
using TestProtocol;

namespace TestClient
{
    using Message = TestProtocol.Message;

    public partial class ClientForm : Form
    {
        private ClientContext context;
        private ClientCredential cred;

        private CustomConnection connection;

        private bool connected;

        private bool initializing;

        private byte[] lastServerToken;

        public ClientForm()
        {
            this.connected = false;
            this.initializing = false;
            this.lastServerToken = null;

            // --- UI ---
            InitializeComponent();

            this.connectButton.Click += connectButton_Click;
            this.disconnectButton.Click += disconnectButton_Click;

            this.encryptButton.Click += encryptButton_Click;
            this.signButton.Click += signButton_Click;

            this.FormClosing += Form1_FormClosing;

            // --- SSPI ---
            this.cred = new ClientCredential( PackageNames.Negotiate );

            this.context = new ClientContext(
                cred,
                "",
                ContextAttrib.InitIntegrity |
                ContextAttrib.ReplayDetect |
                ContextAttrib.SequenceDetect |
                ContextAttrib.MutualAuth |
                ContextAttrib.Delegate |
                ContextAttrib.Confidentiality
            );

            this.connection = new CustomConnection();
            this.connection.Received += connection_Received;
            this.connection.Disconnected += connection_Disconnected;

            // --- UI Fillout ---
            this.usernameTextbox.Text = this.cred.PrincipleName;

            UpdateButtons();
        }

        private void Form1_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.connection.Stop();
        }

        private void connectButton_Click( object sender, EventArgs e )
        {
            if( string.IsNullOrWhiteSpace( this.serverTextBox.Text ) )
            {
                MessageBox.Show( "Please enter a server to connect to" );
            }
            else
            {
                try
                {
                    this.connection.StartClient( this.serverTextBox.Text, (int)this.portNumeric.Value );
                    this.initializing = true;
                    DoInit();
                }
                catch( SocketException socketExcept )
                {
                    if( socketExcept.SocketErrorCode == SocketError.ConnectionRefused ||
                         socketExcept.SocketErrorCode == SocketError.HostDown ||
                         socketExcept.SocketErrorCode == SocketError.HostNotFound ||
                         socketExcept.SocketErrorCode == SocketError.HostUnreachable ||
                         socketExcept.SocketErrorCode == SocketError.NetworkUnreachable ||
                         socketExcept.SocketErrorCode == SocketError.TimedOut
                    )
                    {
                        MessageBox.Show( "Could not connect to the server:\r\n\r\n\t" + socketExcept.Message );
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private void disconnectButton_Click( object sender, EventArgs e )
        {
            this.connection.Stop();
        }

        private void encryptButton_Click( object sender, EventArgs e )
        {
            byte[] plaintext;
            byte[] cipherText;
            Message message;

            plaintext = Encoding.UTF8.GetBytes( this.sendTextbox.Text );

            cipherText = this.context.Encrypt( plaintext );

            message = new Message( ProtocolOp.EncryptedMessage, cipherText );

            this.connection.Send( message );
        }

        private void signButton_Click( object sender, EventArgs e )
        {
            byte[] plaintext;
            byte[] cipherText;
            Message message;

            plaintext = Encoding.UTF8.GetBytes( this.sendTextbox.Text );

            cipherText = this.context.MakeSignature( plaintext );

            message = new Message( ProtocolOp.SignedMessage, cipherText );

            this.connection.Send( message );
        }

        private void connection_Received( Message message )
        {
            this.Invoke( (Action)delegate ()
            {
                if( message.Operation == ProtocolOp.ServerToken )
                {
                    if( initializing )
                    {
                        this.lastServerToken = message.Data;
                        DoInit();
                    }
                    else
                    {
                        MessageBox.Show( "Read unexpected operation from server: " + message.Operation );
                    }
                }
                else if( message.Operation == ProtocolOp.EncryptedMessage )
                {
                    HandleEncrypted( message );
                }
                else if( message.Operation == ProtocolOp.SignedMessage )
                {
                    HandleSigned( message );
                }
            } );
        }

        private void connection_Disconnected()
        {
            this.connected = false;
            this.initializing = false;
            this.lastServerToken = null;

            this.BeginInvoke( (Action)delegate ()
            {
                this.context.Dispose();
                this.context = new ClientContext(
                    this.cred,
                    "",
                    ContextAttrib.InitIntegrity |
                    ContextAttrib.ReplayDetect |
                    ContextAttrib.SequenceDetect |
                    ContextAttrib.MutualAuth |
                    ContextAttrib.Delegate |
                    ContextAttrib.Confidentiality
                );

                UpdateButtons();
            } );
        }

        private void DoInit()
        {
            SecurityStatus status;
            byte[] outToken;

            status = this.context.Init( this.lastServerToken, out outToken );

            if( status == SecurityStatus.ContinueNeeded )
            {
                Message message = new Message( ProtocolOp.ClientToken, outToken );
                this.connection.Send( message );
            }
            else if( status == SecurityStatus.OK )
            {
                this.lastServerToken = null;
                this.initializing = false;
                this.connected = true;
                UpdateButtons();
            }
        }

        private void HandleEncrypted( Message message )
        {
            byte[] plainText = this.context.Decrypt( message.Data );

            string text = Encoding.UTF8.GetString( plainText );

            this.receiveTextbox.Text += "Received encrypted message from server:\r\n" + text + "\r\n";
        }

        private void HandleSigned( Message message )
        {
            byte[] plaintext;
            string text;

            if( this.context.VerifySignature( message.Data, out plaintext ) )
            {
                text = Encoding.UTF8.GetString( plaintext );
                this.receiveTextbox.Text += "Received valid signed message from server:\r\n" + text + "\r\n";
            }
            else
            {
                this.receiveTextbox.Text += "Received *** invalid *** signed message from server.\r\n";
            }
        }

        private void UpdateButtons()
        {
            this.connectButton.Enabled = this.connected == false;
            this.disconnectButton.Enabled = this.connected;

            this.encryptButton.Enabled = this.connected;
            this.signButton.Enabled = this.connected;
        }
    }
}