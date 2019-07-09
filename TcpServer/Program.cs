using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    class Program
    {
        static string IpAddress = "192.168.1.103";
        static int TcpPort = 25656;
        static int UdpPort = 11001;

        static void StartTcpServer()
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse(IpAddress);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, TcpPort);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                byte[] bytes = new byte[256];
                string data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    while (true)
                    {
                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        int i;

                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = Encoding.ASCII.GetString(bytes, 0, i);
                            Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            byte[] msg = Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
                        }

                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        static void StartUdpServer()
        {
            UdpClient udpServer = new UdpClient(UdpPort);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(IpAddress), 0);

            while (true)
            {
                var data = udpServer.Receive(ref remoteEP);
                string result = Encoding.UTF8.GetString(data);
                Console.WriteLine(result);
            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("I'm Server2");
            Task.Run(() => StartTcpServer()).Wait();
            ////Task.Run(() => StartUdpServer()).Wait() ;

            //Console.WriteLine("\nHit enter to continue...");
            //Console.Read();

        }
    }
}
