using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
   public class MyTcpClient
    {
        public int Id { get; set; }
        public TcpClient TcpClient { get; set; }
    }
}
