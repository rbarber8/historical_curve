using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class sserver
    {
        // Incoming data from the client.
        public static string data = null;

        public static void start(string ip)
        {
            Thread aa = new Thread(() => StartListening(ip));
            aa.Start();
        }

        public static void StartListening(string ip)
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 15000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            // All the data has been read from the 
                            // client. Display it on the console.
                            Console.WriteLine("\n\n start:");
                            Console.WriteLine("Read {0} bytes from socket: {1} port: {2} \n Data : {3}",
                                data.Length, IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString()), ((IPEndPoint)handler.RemoteEndPoint).Port.ToString(), data);
                            // Echo the data back to the client.
                            //Send(handler, content);

                            //add analytic for the data (under + exp + callput) => find out live stock price + (-10% ~ +10% strikes) at the money qoutes + (weighted) svwaps / wbid / wask .
                            //trigger the quote processing after receive the data
                            //then calculate the svwap
                            //sent back to client 

                            string content = data.Split('<')[0];
                            if (content != "start")
                            {
                                Console.WriteLine(content);
                                output x = new output
                                {
                                    under = content.Split('|')[0],
                                    exp = Convert.ToInt32(content.Split('|')[1]),
                                    callput = content.Split('|')[2],
                                    ip = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString(),
                                };

                                if (Conn.Dict.ContainsKey(x.under + x.exp + x.callput))
                                {
                                    reqFlow.requestFlow(x);
                                    break;
                                }
                            }
                        }
                    }

                    // Show the data on the console.
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }
    }
}
