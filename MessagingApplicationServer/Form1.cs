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
        public class SocketClient
        {
            public Socket _Socket { get; set; }
            public string _Name { get; set; }
            public SocketClient(Socket socket)
            {
                this._Socket = socket;
            }
        }
        //static IPAddress ipAddress = IPAddress.Any;//Parse("10.2.20.22");
        //static int PortNumber = 12000;
        //TcpListener ServerListener = new TcpListener(ipAddress, PortNumber);
        //TcpClient clientSocket = default(TcpClient);
        //bool startUp = true;
        public Dictionary<string, Socket> dictionary;
        public Hashtable myHashTable = new Hashtable();
        //private int hashNumber = 0;        
        //IPEndPoint ip = new IPEndPoint(IPAddress.Any, PortNumber);
        //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket _serverSocket;
        private static readonly List<Socket> _listSocket = new List<Socket>();
        private const int _BUFFER_SIZE = 2048;
        private const int _Port = 12000;
        private static readonly byte[] _buffer = new byte[_BUFFER_SIZE];
        string serverName = "Server Master: ";
        private const int counter = 0;
        public List<SocketClient> clientList { get; set; }
        public FormServer()           
        {
            InitializeComponent();
            clientList = new List<SocketClient>();
            dictionary = new Dictionary<string, Socket>();
            CheckForIllegalCrossThreadCalls = false;
            
        }
        public void SaveUser()
        {

        }
        private void FormServer_Load(object sender, EventArgs e)
        { 
            Thread ThreadServer = new Thread(SetupServer);
            ThreadServer.Start();
        }
        private void ThreadMod(string mod)
        {
            txtChatBox.Text += Environment.NewLine + mod;
        }
        //private void StartServer()
        //{
        //    Action<string> DelegateModifyText = ThreadMod;
        //    ServerListener.Start();
        //    //socket.Bind(ip);
        //    //socket.Listen(20);
        //    Invoke(DelegateModifyText, "Server waiting for connection!");
        //    //Socket client = socket.Accept();
        //    //IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
        //    //Invoke(DelegateModifyText, "Connected with " + clientEndPoint.Address + " at port \n" + clientEndPoint.Port);
        //    clientSocket = ServerListener.AcceptTcpClient();
        //    while (startUp)
        //    {
        //        try
        //        {
        //            NetworkStream networkStream = clientSocket.GetStream();                    
        //            byte[] bytesFrom = new byte[1024];
        //            //clientSocket = ServerListener.AcceptTcpClient();
        //            //client.Receive(bytesFrom, 0);
        //            networkStream.Read(bytesFrom, 0, bytesFrom.Length);
        //            string dataFromClient = Encoding.ASCII.GetString(bytesFrom);
        //            //dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
        //            myHashTable.Add(dataFromClient, clientSocket);
        //            //Invoke(DelegateModifyText,(dataFromClient + "Joined chat room");
        //            //HandleClient client = new HandleClient();
        //            //client.StartClient(clientSocket, dataFromClient, myHashTable);
        //            Invoke(DelegateModifyText, "Server ready!");
        //            string serverResponse = "Recieved! \n";
        //            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
        //            //client.Send(sendBytes, 0);
        //            networkStream.Write(sendBytes, 0, sendBytes.Length);
        //            networkStream.Flush();
        //            startUp = false;
                    
        //        }
        //        catch
        //        {
        //            //StartServer();
        //            ServerListener.Stop();
        //            ServerListener.Start();
        //            Invoke(DelegateModifyText, "Server waiting connections!");
        //            clientSocket = ServerListener.AcceptTcpClient();
        //            Invoke(DelegateModifyText, "Server ready!");
        //        }
        //    }
        //    Thread thread = new Thread(GetMessage);
        //    thread.Start();
        //}
        private void SetupServer()
        {
            Action<string> DelegateModifyText = ThreadMod;
            Invoke(DelegateModifyText, "Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _Port));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(AcceptCallback, null);
            Invoke(DelegateModifyText, "Server setup complete");
        }
        private void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;
            Action<string> DelegateModifyText = ThreadMod;
            try
            {
                socket = _serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            //checkListUser.Items.Add(socket.RemoteEndPoint.ToString());
            _listSocket.Add(socket);
            socket.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Invoke(DelegateModifyText, "Client connected");
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            SocketClient client = new SocketClient(current);
            int received;
            Action<string> DelegateModifyText = ThreadMod;
            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Invoke(DelegateModifyText, "Client forcefully disconnected");
                ClientDisconnect();
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                _listSocket.Remove(current);
                return;
            }
            byte[] recBuf = new byte[received];
            Array.Copy(_buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Invoke(DelegateModifyText, text);
            if (current.Connected)
            {
                if (text.Contains("@#"))
                {            
                    clientList.Add(client);
                    for (int i = 0; i < clientList.Count; i++)
                    {
                        if (clientList.Count != dictionary.Count)
                        {
                            //checkListUser.Items.RemoveAt(clientList.Count -1);
                            checkListUser.Items.Insert(0, text.Substring(2, text.Length - 2));
                            dictionary.Add(text, current);
                            
                            string serverResponse = "Welcome to Nate's Server! \n";
                            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                            current.Send(sendBytes, 0, sendBytes.Length, 0);
                        }                    
                    }
                }
            }
            current.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
                //for (int i = 0; i < checkListUser.Items.Count; i++)
                //{
                //    if (checkListUser.GetItemChecked(i))
                //    {
                //        string str = (string)checkListUser.Items[i];
                //        MessageBox.Show(str);
                //    }
                //}
                //string checkedItems = string.Empty;
                //foreach (object user in checkListUser.CheckedItems)
                //{
                //    checkedItems = checkedItems + user.ToString();
                //}
                //checkedItems = checkedItems.Substring(0, checkedItems.Length);
                //txtUser.Text = checkedItems;
            ////new Thread(() =>
            ////{
            ////    while (true)
            ////    {
            ////        byte[] sizeBuf = new byte[4];
            ////        current.Receive(sizeBuf, 0, sizeBuf.Length, 0);
            ////        int size = BitConverter.ToInt32(sizeBuf, 0);
            ////        MemoryStream ms = new MemoryStream();  //holds data for buffer we recieve
            ////        while (size > 0)
            ////        {
            ////            byte[] buffer;
            ////            if (size < current.ReceiveBufferSize)
            ////            {
            ////                buffer = new byte[size];
            ////            }
            ////            else
            ////                buffer = new byte[current.ReceiveBufferSize];
            ////               int rec = current.Receive(buffer, 0, buffer.Length, 0);
            ////               size -= rec;
            ////               ms.Write(buffer, 0, buffer.Length);
            ////        }
            ////        ms.Close();
            ////        byte[] data = ms.ToArray();
            ////        ms.Dispose();
            ////        Invoke((MethodInvoker)delegate
            ////        {
            ////            txtChatBox.Text = Encoding.Default.GetString(data);
            ////        });
            ////    }
            ////}).Start();
        }
        private void ClientDisconnect()
        {
            string clearSocket = "";
            foreach(string name in dictionary.Keys) 
            {
                if (_listSocket.Count == dictionary.Count)              //if its not in here, remove it
                {
                    clearSocket = name;
                }
            }
                    dictionary.Remove(clearSocket);
            
            for (int i = 0; i < checkListUser.Items.Count; i++)
            {
                if (clearSocket.Contains(checkListUser.Items[i].ToString()))
                {
                    checkListUser.Items.RemoveAt(i);
                    clientList.RemoveAt(i);
                }
            }
        }
        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            string s = serverName + txtUser.Text;
            byte[] message = Encoding.ASCII.GetBytes(s);

            foreach (string numKey in dictionary.Keys)
            {
                for (int i = 1; i <= checkListUser.Items.Count; i++)
                {
                    if(numKey.Contains(checkListUser.Items[i - 1].ToString()))
                    {
                        if (checkListUser.GetItemChecked(i - 1))
                        {
                            //clientList[i]._Socket.Send(message, 0, message.Length, 0);
                            dictionary[numKey].Send(message, 0, message.Length, 0);

                        }
                    }
                }

            }
            txtUser.Clear();
            //byte[] data = Encoding.Default.GetBytes(txtUser.Text);
            //_serverSocket.Send(data, 0, data.Length, 0);
        }

        private void SendData(Socket client, string dataSend)
        {
            
            //Socket client = socket.Accept();
            //NetworkStream networkStream = clientSocket.GetStream();
            //_serverSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _listSocket);
            //_serverSocket.Send(message);                  
            //client.Send(message, 0);
            //networkStream.Write(message, 0, message.Length);
            //networkStream.Flush();
            //txtUser.Clear();

        }
        //public void GetMessage()
        //{
        //    NetworkStream networkStream = clientSocket.GetStream();
        //    byte[] bytes = new byte[1024];
        //    while (clientSocket.Connected)
        //    {
        //        //int bytesRead = socket.Receive(bytes, 0);
        //        int bytesRead = networkStream.Read(bytes, 0, bytes.Length);
        //        //SetText(Encoding.ASCII.GetString(bytes, 0, bytesRead));
        //        this.SetText(Encoding.ASCII.GetString(bytes, 0, bytesRead));
        //    }
        //}
        //private void SetText(string text)
        //{
        //    if (this.txtUser.InvokeRequired)
        //    {
        //        SetTextCallback setText = new SetTextCallback(SetText);
        //        this.Invoke(setText, new object[] { text });
        //    }
        //    else
        //    {
        //        this.txtChatBox.Text = this.txtChatBox.Text + "\r\n" + text;
        //    }
        //}

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

        private void checkListUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = checkListUser.SelectedIndex;
            if(selected != -1)
            {
                Text = checkListUser.Items[selected].ToString();
            }
        }

        private void txtChatBox_TextChanged(object sender, EventArgs e)
        {
            txtChatBox.SelectionStart = txtChatBox.Text.Length;
            txtChatBox.ScrollToCaret();
        }
    }
}
