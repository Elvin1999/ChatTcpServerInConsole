using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace TcpServer
{
    class Program
    {
        static string IpAddress = "10.1.16.38";
        static int TcpPort = 25656;
        static int UdpPort = 11001;
        public static string data = null;
        public static List<MyTcpClient> MyTcpClients { get; set; }
        public static int Id { get; set; } = 0;
        public static byte[] bytes = new byte[256];
        static void AddClientToServer()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse(IpAddress);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, TcpPort);
                // Start listening for client requests.
                server.Start();
                // Buffer for reading data

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    MyTcpClient myTcpClient = new MyTcpClient();
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    myTcpClient.TcpClient = client;
                    myTcpClient.Id = ++Id;
                    MyTcpClients.Add(myTcpClient);
                    Console.WriteLine("Connected!");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }
        public static TcpListener server = null;
        static void StartTcpServer()
        {
            TcpClient client = MyTcpClients[0].TcpClient;
            try
            {
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
                        Console.WriteLine("\nReceived: {0}", data);
                        var len = data.Length;
                        var idData = data[len - 1].ToString();
                        try
                        {
                            var id = int.Parse(idData);
                            var itemTcp = MyTcpClients.SingleOrDefault(x => x.Id == id);
                            client = itemTcp.TcpClient;
                            // Process the data sent by the client.
                            data = data.ToUpper();
                            stream = client.GetStream();
                            byte[] msg = Encoding.ASCII.GetBytes(data);
                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("\nSent: {0}", data);
                        }
                        catch (Exception)
                        {

                        }

                    }

                }

                // Shutdown and end connection
                client.Close();

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
        static void Main(string[] args)
        {
            MyTcpClients = new List<MyTcpClient>();
            Console.WriteLine("I'm Server2");
            Thread thread = new Thread(AddClientToServer);
            thread.Start();
            do
            {
                if (MyTcpClients.Count != 0)
                {
                    Thread thread2 = new Thread(StartTcpServer);
                    thread2.Start();
                    break;
                }
            } while (MyTcpClients.Count == 0);
            ////Task.Run(() => StartUdpServer()).Wait() ;

            //Console.WriteLine("\nHit enter to continue...");
            //Console.Read();

        }
    }
}
