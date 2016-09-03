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
        public Dictionary<string, Socket> dictionary;
        private static Socket _serverSocket;
        private static readonly List<Socket> _listSocket = new List<Socket>();
        private const int bufferSize = 2048;
        private const int portNumber = 12000;
        private static readonly byte[] _buffer = new byte[bufferSize];
        string serverName = "Server Master: ";
        public List<SocketClient> clientList { get; set; }
        public FormServer()           
        {
            InitializeComponent();
            clientList = new List<SocketClient>();
            dictionary = new Dictionary<string, Socket>();
            CheckForIllegalCrossThreadCalls = false;   
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
        private void SetupServer()
        {
            Action<string> DelegateModifyText = ThreadMod;
            Invoke(DelegateModifyText, "Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
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
            _listSocket.Add(socket);
            socket.BeginReceive(_buffer, 0, bufferSize, SocketFlags.None, ReceiveCallback, socket);
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
                ClientDisconnect(current);
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
                            checkListUser.Items.Insert(0, text.Substring(2, text.Length - 2));
                            dictionary.Add(text, current);
                            ASCIIEncoding ascii = new ASCIIEncoding();
                            string serverResponse = "Welcome to Nate's Server! \n";
                            byte[] sendBytes = ascii.GetBytes(serverResponse);
                            //current.Send(sendBytes, 0, sendBytes.Length, 0);                
                            current.Send(sendBytes, 0, sendBytes.Length, 0);
                            SendClientName();                       
                        }
                    }
                }
            }
            current.BeginReceive(_buffer, 0, bufferSize, SocketFlags.None, ReceiveCallback, current);
        }
        private void ClientDisconnect(Socket socket)
        {
            Action<string> DelegateModifyText = ThreadMod;
            string clearSocket = "";
            foreach (string name in dictionary.Keys)
            {
                if (dictionary[name] == socket)              //if its not in here, remove it
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
            Invoke(DelegateModifyText, clearSocket.Substring(2, clearSocket.Length - 2) + " disconnected");
        }
        private void txtUser_TextChanged(object sender, EventArgs e)
        {           
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendData();
            //SendClientName();
        }
        private void SendData()
        {
            Action<string> DelegateModifyText = ThreadMod;
            string s = serverName + txtUser.Text;
            byte[] message = Encoding.ASCII.GetBytes(s);
            Invoke(DelegateModifyText, s);
            foreach (string numKey in dictionary.Keys)
            {
                for (int i = 1; i <= checkListUser.Items.Count; i++)
                {
                    if(numKey.Contains(checkListUser.Items[i - 1].ToString()))
                    {
                        if (checkListUser.GetItemChecked(i - 1))
                        {
                            dictionary[numKey].Send(message, 0, message.Length, 0);
                        }
                    }
                }
            }
            txtUser.Clear();
        }
        public void SendClientName()
        {
            byte[] recBuf = new byte[bufferSize];
            Array.Copy(_buffer, recBuf, bufferSize);
            string text = Encoding.ASCII.GetString(recBuf);
            ASCIIEncoding ascii = new ASCIIEncoding();
            foreach (string numKey in dictionary.Keys)
            {
                string s = text;// + "has Connected";
                byte[] list = ascii.GetBytes(s);
                //current.Send(list, 0, list.Length, 0);
                dictionary[numKey].Send(list, 0, list.Length, 0);
            }
            recBuf = null;
            text = null;
        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();          
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
