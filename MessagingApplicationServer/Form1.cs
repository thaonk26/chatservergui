using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace MessagingApplicationServer
{
    delegate void SetTextCallback(string text);
    public partial class FormServer : Form
    {
        ///private static readonly TcpListener listener = new TcpListener(IPAddress.Any, 12000);
        //private static int port = 12000;
        //private static TcpListener listener;
        //private static Thread thread;
        //private static int clientId = 0;
        static IPAddress ipAddress = IPAddress.Any;//Parse("10.2.20.22");
        static int PortNumber = 12000;
        TcpListener ServerListener = new TcpListener(ipAddress, PortNumber);
        TcpClient clientSocket = default(TcpClient);
        bool startUp = true;
        //public Dictionary<string, TcpClient> dictionary = new Dictionary<string, TcpClient>();
        //Hashtable myHashTable = new Hashtable();
        //private int hashNumber = 0;        
        public FormServer()           
        {
            InitializeComponent();
        }       
        public void SaveUser()
        {

        }     
        private void FormServer_Load(object sender, EventArgs e)
        {
            Thread ThreadServer = new Thread(StartServer);
            ThreadServer.Start();
        }
        private void ThreadMod(string mod)
        {
            txtChatBox.Text += Environment.NewLine + mod;
        }
        private void StartServer()
        {
            Action<string> DelegateModifyText = ThreadMod;           
            ServerListener.Start();
            Invoke(DelegateModifyText, "Server waiting for connection!");
            clientSocket = ServerListener.AcceptTcpClient();
            Invoke(DelegateModifyText, "Server ready!");
            while (startUp)
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();                    
                    byte[] bytesFrom = new byte[1024];
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    string serverResponse = "Recieved! \n";
                    byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    startUp = false;
                    
                }
                catch
                {
                    ServerListener.Stop();
                    ServerListener.Start();
                    Invoke(DelegateModifyText, "Server waiting connections!");
                    clientSocket = ServerListener.AcceptTcpClient();
                    Invoke(DelegateModifyText, "Server ready!");
                }
            }           
            Thread thread = new Thread(GetMessage);
            thread.Start();
            
        }
        //private void DisplayText()
        //{
        //    NetworkStream networkStream = clientSocket.GetStream();
        //    byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
        //    //byte[] bytesFrom = new byte[1024];
        //    while (true)
        //    {
        //        Action<string> DelegateModifyText = ThreadMod;
        //        int bytesRead = networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
        //        string dataReceived = Encoding.ASCII.GetString(bytesFrom, 0, bytesRead);
        //        //txtChatBox.Text += Environment.NewLine + dataReceived;
        //        Invoke(DelegateModifyText, dataReceived);
        //        //networkStream.Read(bytesFrom, 0, bytesFrom.Length);
        //        //string dataFromClient = Encoding.ASCII.GetString(bytesFrom);
        //        //dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
        //        //string serverResponse = txtUser.Text;
        //        //byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
        //        //networkStream.Write(sendBytes, 0, sendBytes.Length);
        //        //networkStream.Flush();
        //    }
        //}
        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string s = "Server Master: " + txtUser.Text;
            NetworkStream networkStream = clientSocket.GetStream();
            byte[] message = Encoding.ASCII.GetBytes(s);
            networkStream.Write(message, 0, message.Length);
            networkStream.Flush();
            txtUser.Clear();

        }
        public void GetMessage()
        {
            NetworkStream networkStream = clientSocket.GetStream();
            byte[] bytes = new byte[1024];
            while (true)
            {
                int bytesRead = networkStream.Read(bytes, 0, bytes.Length);
                this.SetText(Encoding.ASCII.GetString(bytes, 0, bytesRead));
            }
        }
        private void SetText(string text)
        {
            if (this.txtUser.InvokeRequired)
            {
                SetTextCallback setText = new SetTextCallback(SetText);
                this.Invoke(setText, new object[] { text });
            }
            else
            {
                this.txtChatBox.Text = this.txtChatBox.Text + "\r\n" + text;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();          
        }

        private void txtUserID_TextChanged(object sender, EventArgs e)
        {
            
        }

        //private static void ServeData(object clientSocket)
        //{
        //    Console.WriteLine("Started thread {0}", Thread.CurrentThread.ManagedThreadId);
        //    Random random = new Random();
        //    try
        //    {
        //        TcpClient client = (TcpClient)clientSocket;
        //        NetworkStream stream = client.GetStream();
        //        while (true)
        //        {
        //            if (random.NextDouble() < 0.1)
        //            {
        //                var msg = Encoding.ASCII.GetBytes("Status update from thread " + Thread.CurrentThread.ManagedThreadId);
        //                stream.Write(msg, 0, msg.Length);
        //                Console.WriteLine("Status update from thread " + Thread.CurrentThread.ManagedThreadId);
        //            }
        //            Thread.Sleep(new TimeSpan(0, 0, random.Next(1, 5)));
        //        }
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("Socket exception in thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e);
        //    }
        //}

    }
}
