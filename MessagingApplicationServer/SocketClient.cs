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
    public class SocketClient
    {
        public Socket _Socket { get; set; }
        public SocketClient(Socket socket)
        {
            this._Socket = socket;
        }
    }
}
