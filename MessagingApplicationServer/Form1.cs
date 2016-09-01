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
        public Dictionary<int, Socket> dictionary;
        public Hashtable myHashTable = new Hashtable();
        //private int hashNumber = 0;        
        //IPEndPoint ip = new IPEndPoint(IPAddress.Any, PortNumber);
        //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket _serverSocket;
        private static readonly List<Socket> _clientSockets = new List<Socket>();
        private const int _BUFFER_SIZE = 2048;
        private const int _PORT = 12000;
        private static readonly byte[] _buffer = new byte[_BUFFER_SIZE];
        string serverName = "Server Master: ";
        private const int counter = 0;
        public List<SocketClient> clientList { get; set; }
        public FormServer()           
        {
            InitializeComponent();
            clientList = new List<SocketClient>();
            dictionary = new Dictionary<int, Socket>();
            CheckForIllegalCrossThreadCalls = false;
            
        }
        public void SaveUser()
        {

        }     
        private void FormServer_Load(object sender, EventArgs e)
        {
            Thread ThreadServer = new Thread(SetupServer);
            ThreadServer.Start();
            //try
            //{
            //    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 12000);
            //    serverSocket.Bind(ipEndPoint);
            //    serverSocket.Listen(3);
            //    serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "SGSserverTCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        //private void OnAccept(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket clientSocket = serverSocket.EndAccept(ar);
        //        serverSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
        //    }catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "SGSserverTCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //}
        //private void OnReceive(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket clientSocket = (Socket)ar.AsyncState;
        //        clientSocket.EndReceive(ar);
                
        //    }catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "SGSserverTCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
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
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _PORT));
            _serverSocket.Listen(5);
            string serverResponse = "Recieved! \n";
            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
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
            checkListUser.Items.Add(socket.RemoteEndPoint.ToString());
            _clientSockets.Add(socket);
            socket.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Invoke(DelegateModifyText, "Client connected, waiting for request...");
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            //int counter = 1;
            //dictionary.Add(counter, _serverSocket);
            
            SocketClient client = new SocketClient(current);
            //clientList.Add(client);
            //clientList[0] = client; refer to client
            int received;
            Action<string> DelegateModifyText = ThreadMod;
            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Invoke(DelegateModifyText, "Client forcefully disconnected");
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                _clientSockets.Remove(current);
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
                            checkListUser.Items.RemoveAt(clientList.Count -1);
                            checkListUser.Items.Insert(0, text.Substring(2, text.Length - 2));
                            dictionary.Add(clientList.Count, current);                    
                        }                    
                    }
                }
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
            }
            current.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
            //if (text.ToLower() == "get time") // Client requested time
            //{
            //    Invoke(DelegateModifyText, "Text is a 'get time' request");
            //    byte[] data = Encoding.ASCII.GetBytes(DateTime.Now.ToLongTimeString());
            //    current.Send(data);
            //    Invoke(DelegateModifyText, "Time sent to client");
            //}
            //else if (text.ToLower() == "exit") // Client wants to exit gracefully
            //{
            //    // Always Shutdown before closing
            //    current.Shutdown(SocketShutdown.Both);
            //    current.Close();
            //    _clientSockets.Remove(current);
            //    Invoke(DelegateModifyText, "Client disconnected");
            //    return;
            //}
            //else
            //{
            //    //Invoke(DelegateModifyText,("Text is an invalid request");
            //    //byte[] data = Encoding.ASCII.GetBytes("Invalid function request");
            //    //current.Send(data);
            //    //Invoke(DelegateModifyText,("Warning Sent");
            //}
            //Action<string> DelegateModifyText = ThreadMod;
            //Socket current = (Socket)AR.AsyncState;
            ////int counter = 1;
            //SocketClient client = new SocketClient(current);
            //int received;
            //try
            //{
            //    received = current.EndReceive(AR);
            //}
            //catch (SocketException)
            //{
            //    Invoke(DelegateModifyText, "Client forcefully disconnected");
            //    current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
            //    _clientSockets.Remove(current);
            //    return;
            //}
            //if (current.Connected)
            //{
            //    byte[] incomingMessage = new byte[received];
            //    string message = Encoding.ASCII.GetString(incomingMessage);
            //    Array.Copy(_buffer, incomingMessage, received);
            //    //Invoke(DelegateModifyText, message);
            //    Invoke(DelegateModifyText, message);
            //    //txtChatBox.Text = txtChatBox.Text + message;
            //    if (message.Contains("@#"))
            //    {
            //        dictionary.Add(counter, _serverSocket);
            //        clientList.Add(client);

            //    }

            //}
            //current.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);

        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            string s = serverName + txtUser.Text;
            byte[] message = Encoding.ASCII.GetBytes(s);

            for (int i = 0; i < clientList.Count; i++)
            {
                foreach (object itemChecked in checkListUser.CheckedItems)
                {
                    clientList[i]._Socket.Send(message, 0, message.Length, SocketFlags.None);
                }

            }
            txtUser.Clear();
        }
        private void SendData(Socket client, string dataSend)
        {
            
            //Socket client = socket.Accept();
            //NetworkStream networkStream = clientSocket.GetStream();
            //_serverSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clientSockets);
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
