using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace socket
{
    /// <summary>
    ///  to build application As Server we have to :
    ///  1- object form Tcplistener(Class) 
    ///  2- object from Socket(Class) that takes object from Tcplistener By AcceptSocket() method ; 
    ///  3- object from Networkstream that tackes object from socket class 
    ///  4- two  object from streamreader and streamwriter  Or  BinaryReader and BinaryWritter ; each of this take object from newrorkstram(Class)    /// 
    /// </summary>
    public partial class Server : Form
    {

        //Socket is Encapsulates object form Tcplistener (Ex: connection=new TCplistener().Acceptsocket(); )
        Socket connection; //Socket is used to Link  Transport layer(TCPlistener OR TCPClient) With Network layer(NetworkStream)  
        Thread read_thread; // to Run this Connection With anthor processes 
        NetworkStream socket_stream; // Encapsulate object from Socket Class(Connection) 
        BinaryReader reader; // Encapsulate object from Networkstream(socket_stream)  and  Reading from Network  
        BinaryWriter writer; // Encapsulate object from Networkstream(socket_stream)  and  Writing on Network 

       // Thread reciev_thread;
        public Server()
        {
            InitializeComponent();
            ThreadStart ts = new ThreadStart(Run_Server);
            ts += recive_file; //add this method to the method Queue

            read_thread = new Thread(ts);
            read_thread.Start();

        }

        public void recive_file()
        {            


        }


        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode); //Terminates All thread in the Application 
        }

        private void input_text_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.KeyCode == Keys.Enter) && (connection != null))
                {
                    // sends the text to the client
                    writer.Write("Server>>" + input_text.Text);
                    output_text.Text += "\r\nSERVER>> " + input_text.Text;

                    // if the Client(Sender) wants to End the session
                    if (input_text.Text == "terminate")
                    {
                        connection.Close();
                        Application.Exit();
                    }

                    input_text.Clear();
                }
            }
            catch(SocketException se)
            {
                MessageBox.Show(se.Message);
            }

        }



        // allows a client to connect and displays the text it sends
        public void Run_Server()
        {
            TcpListener Tcp_listener;
            int counter = 1;


            // wait for a client connection and display the text 
            // that the client sends 
            try
            {
                // Step 1: create TcpListener 
                Tcp_listener = new TcpListener(5000);

                // Step 2: TcpListener waits for connection request 
                Tcp_listener.Start(); // Start listening  for incomming connection Requests

                // Step 3: establish connection upon client request 
                while (true)
                {
                    output_text.Text = "Waiting for connection\r\n";

                    // accept an incoming connection ; Step 5
                    connection = Tcp_listener.AcceptSocket(); // Socket Class in Encapsulates TCPlistener Class

                    // create NetworkStream object associated with socket ; step 6
                    socket_stream = new NetworkStream(connection);// NetworkStream Class in Encapsulates Socket Class

                    // create objects for transferring data across stream  Step 7 & 8
                    writer = new BinaryWriter(socket_stream);

                    reader = new BinaryReader(socket_stream);


                    output_text.Text += "Connection " + counter + " received.\r\n";

                   // inform client that connection was successfull  ; Writin in the Stream (Network)

                    writer.Write("Server>> Connection Successful");

                    string theReply = "";

                    // Step 9: read String data sent from client 
                    do
                    {
                        try
                        {
                            // read the string sent to the server  
                            theReply = reader.ReadString(); // Reading from Network
                            
                            // display the message 
                            output_text.Text += "\r\n" + theReply;
                        }

                        // handle exception if error reading data
                        catch (Exception)
                        {
                            break;
                        }

                    } while ((theReply != "Client>>terminate") && (connection.Connected)); // important

                    output_text.Text += "\r\nUser terminated connection";

                    // Step 10: close connection

                    input_text.ReadOnly = false;

                    writer.Close();

                    reader.Close();

                    socket_stream.Close();

                    connection.Close();

                    ++counter;

                    Application.Exit();
                }
                } // end try

                catch ( Exception error )
                 {
                    MessageBox.Show( error.ToString() );
                 }

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //saveFileDialog1.Filter = "JPEG Image (*.jpg)*.jpg";
                if(socket_stream!=null)
                   pictureBox1.Image = Image.FromStream(socket_stream);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }





    }// End Class Server 
}
