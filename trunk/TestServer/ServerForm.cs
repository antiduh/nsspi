﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestProtocol;

namespace TestServer
{
    using NSspi;
    using NSspi.Contexts;
    using NSspi.Credentials;
    using Message = TestProtocol.Message;

    public partial class ServerForm : Form
    {
        private ServerCredential serverCred;

        private ServerContext serverContext;

        private CustomServer server;

        private bool running;

        private bool initializing;

        private bool connected;

        public ServerForm()
        {
            InitializeComponent();

            this.serverCred = new ServerCredential( PackageNames.Negotiate );

            this.serverContext = new ServerContext( 
                serverCred,
                ContextAttrib.AcceptIntegrity | 
                ContextAttrib.ReplayDetect | 
                ContextAttrib.SequenceDetect | 
                ContextAttrib.MutualAuth | 
                ContextAttrib.Delegate | 
                ContextAttrib.Confidentiality
            );

            this.server = new CustomServer();
            this.server.Received += server_Received;
            this.server.Disconnected += server_Disconnected;

            this.FormClosing += Form1_FormClosing;

            this.startButton.Click += startButton_Click;
            this.stopButton.Click += stopButton_Click;

            this.encryptButton.Click += encryptButton_Click;
            this.signButton.Click += signButton_Click;

            this.running = false;
            this.initializing = false;
            this.connected = false;

            UpdateButtons();

            this.serverUsernameTextbox.Text = this.serverCred.Name;
        }

        private void Form1_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.server.Stop();
        }

        private void startButton_Click( object sender, EventArgs e )
        {
            this.server.StartServer( (int)this.portNumeric.Value );

            this.running = true;
            this.initializing = true;
            this.connected = false;

            UpdateButtons();
        }

        private void stopButton_Click( object sender, EventArgs e )
        {
            this.server.Stop();

            this.running = false;
            this.initializing = false;
            this.connected = false;

            UpdateButtons();
        }

        private void encryptButton_Click( object sender, EventArgs e )
        {
            Message message;

            byte[] plainText = Encoding.UTF8.GetBytes( this.sendTextbox.Text );
            byte[] cipherText = this.serverContext.Encrypt( plainText );

            message = new Message( ProtocolOp.EncryptedMessage, cipherText );

            this.server.Send( message );
        }

        private void signButton_Click( object sender, EventArgs e )
        {
            byte[] plainText = Encoding.UTF8.GetBytes( this.sendTextbox.Text );
            byte[] signedData;
            Message message;

            signedData = this.serverContext.MakeSignature( plainText );

            message = new Message( ProtocolOp.SignedMessage, signedData );

            this.server.Send( message );
        }
        
        private void UpdateButtons()
        {
            this.startButton.Enabled = this.running == false;
            this.stopButton.Enabled = this.running;

            this.encryptButton.Enabled = this.connected;
            this.signButton.Enabled = this.connected;
        }


        private void server_Received( Message message )
        {
            if( message.Operation == ProtocolOp.ClientToken )
            {
                HandleInit( message );
            }
            else if( message.Operation == ProtocolOp.EncryptedMessage )
            {
                HandleEncrypted( message );
            }
            else if( message.Operation == ProtocolOp.SignedMessage )
            {
                HandleSigned( message );
            }
            else
            {
                HandleUnknown( message );
            }
        }

        private void server_Disconnected()
        {
            this.running = false;
            this.initializing = true;
            this.connected = false;


            this.serverContext.Dispose();
            this.serverContext = new ServerContext(
                serverCred,
                ContextAttrib.AcceptIntegrity |
                ContextAttrib.ReplayDetect |
                ContextAttrib.SequenceDetect |
                ContextAttrib.MutualAuth |
                ContextAttrib.Delegate |
                ContextAttrib.Confidentiality
            );

            this.BeginInvoke( (Action)delegate() 
            {
                UpdateButtons();
                this.clientUsernameTextBox.Text = "";
            });
        }


        private void HandleInit( Message message )
        {
            byte[] nextToken;
            SecurityStatus status;

            if( initializing )
            {
                status = this.serverContext.AcceptToken( message.Data, out nextToken );

                if( status == SecurityStatus.OK || status == SecurityStatus.ContinueNeeded )
                {
                    if( nextToken != null )
                    {
                        this.server.Send( new Message( ProtocolOp.ServerToken, nextToken ) );
                    }

                    if( status == SecurityStatus.OK )
                    {
                        this.initializing = false;
                        this.connected = true;

                        this.Invoke( (Action)delegate() 
                        { 
                            UpdateButtons();
                            this.clientUsernameTextBox.Text = serverContext.ContextUserName;
                        } );
                    }
                }
                else
                {
                    this.Invoke( (Action)delegate() 
                    {
                        MessageBox.Show( "Failed to accept token from client. Sspi error code: " + status );
                    } );
                }

            }
        }

        private void HandleEncrypted( Message message )
        {
            this.Invoke( (Action)delegate()
            {
                byte[] plainText = this.serverContext.Decrypt( message.Data );
                string text = Encoding.UTF8.GetString( plainText );
                
                this.receivedTextbox.Text += "Received encrypted message from client:\r\n" + text + "\r\n";
            } );
        }

        private void HandleSigned( Message message )
        {
            this.Invoke( (Action)delegate()
            {
                byte[] plainText;

                if( this.serverContext.VerifySignature( message.Data, out plainText ) )
                {
                    string text = Encoding.UTF8.GetString( plainText );

                    this.receivedTextbox.Text += "Received valid signed message from client:\r\n" + text + "\r\n";
                }
                else
                {
                    this.receivedTextbox.Text += "Received *** invalid *** signed message from client.\r\n";
                }
            } );
        }

        private void HandleUnknown( Message message )
        {
            this.Invoke( (Action)delegate()
            {
                MessageBox.Show( "Received unexpected message from server. Message type: " + message.Operation );
            } );
        }

    }
}
