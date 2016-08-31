using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Collections;


namespace MessagingApplicationServer
{
    public class Server
    {
       public void startServer(TextBox textBox, RichTextBox richBox)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("10.2.20.16"), 12000);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ip);
            socket.Listen(20);
            richBox.Text = richBox.Text + "Waiting for client...";
            Socket client = socket.Accept();
            IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
            richBox.Text = richBox.Text + "Connected with " + clientEndPoint.Address + " at port \n" + clientEndPoint.Port;
            string welcome = textBox.Text;
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(welcome);
            client.Send(data, data.Length, SocketFlags.None);
            richBox.Text = richBox.Text + "Disconnected from " + clientEndPoint.Address;
            client.Close();
            socket.Close();
        }
    }
}
