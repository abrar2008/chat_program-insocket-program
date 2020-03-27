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


namespace Socket_in_Client
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Client : Form
    {

        NetworkStream output_stream;
        BinaryReader reader;
        BinaryWriter writer;
        Thread readthread;
        string message = "";

        TcpClient client; // for Sending Data to Server

        // default constructor        
        public Client()
        {
            InitializeComponent();

            ThreadStart ts = new ThreadStart(Run_client);
            //ts += send_file;

            readthread = new Thread(ts);

            readthread.Start();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    writer.Write("Client>>" + input_text.Text); // Send to Sever 

                    output_text.Text += "\r\nClient Say:>> " + input_text.Text; 

                    input_text.Clear();

                    
                }
            }
            catch(SocketException se)
            {
                output_text.Text += "\n Error waiting Object";
            }
        }



        public void Run_client()
        {
            try
            {
                output_text.Text += "Attempting to Connecting to the server\n";

                // Step 1: Create TCPClient and connect to the Server 
                client = new TcpClient();
                client.Connect("localhost", 5000); // 5000 is the port number  that the Sever is listening on it 

                // Step 2: Get NetworkStream Associated With TcpClient 
                output_stream  = client.GetStream();

                // Step 3: Create Object for Writing and Reading Across Stream( NetworkStream)
                writer = new BinaryWriter(output_stream);
                reader = new BinaryReader(output_stream);
               // output_text.Text += "\r\nGot IO Stream \r\n";
                input_text.ReadOnly = false;

                do
                {
                    //Step 3: 
                    try
                    {
                        //Reading the message form Server  

                        message = reader.ReadString();

                        output_text.Text += "\r\n" + message;

                    }
                    catch (Exception e)
                    {
                        System.Environment.Exit(System.Environment.ExitCode);
                    }
                } while (message!="Server>>terminate");


                // Step 4: Closing Connection 
                writer.Close();
                reader.Close();
                output_stream.Close();
                client.Close();

                Application.Exit();
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
                
        }



        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void input_text_TextChanged(object sender, EventArgs e)
        {

        }

        public void send_file()
        {
            try
            {
                openFileDialog1.ShowDialog();

                string mypic_path = openFileDialog1.FileName;

                pictureBox1.Image = Image.FromFile(mypic_path);// view the pciture

                MemoryStream ms = new MemoryStream();// important 

                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat); // save the picture in the MemoryStream 

                byte[] arr_img = ms.GetBuffer(); // get byte array by Memory Stream 

                ms.Close();

                //TcpClient client = new TcpClient("localhost", 5000);//connecting to the Server


                //NetworkStream myns = client.GetStream(); // Get NetworkStream 

                BinaryWriter bw = new BinaryWriter(output_stream);// 

                bw.Write(arr_img); // send the image(after converting to byte array) to the server that have above Address 

                bw.Close();
//              myns.Close();
                client.Close();

            }

            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }

        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //send_file();

            try
            {
                openFileDialog1.ShowDialog();

                string mypic_path = openFileDialog1.FileName;

                pictureBox1.Image = Image.FromFile(mypic_path);// view the pciture

                MemoryStream ms = new MemoryStream();// important 

                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat); // save the picture in the MemoryStream 

                byte[] arr_img = ms.GetBuffer(); // get byte array by Memory Stream 

                ms.Close();

                TcpClient client = new TcpClient("localhost", 5000);//connecting to the Server

                NetworkStream myns = client.GetStream(); // Get NetworkStream 

                BinaryWriter bw = new BinaryWriter(myns);// 

                bw.Write(arr_img); // send the image(after converting to byte array) to the server that have above Address 

                //bw.Close();
                //myns.Close();
                //client.Close();

            }

            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }


        }

    }
}
