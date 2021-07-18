using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AppServer
{
    class Server
    {
        private TcpListener server = null;
        private static int connectedUsersAmount = 0;
        private static List<ClientData> clients = new List<ClientData>();
        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }
        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    connectedUsersAmount++;
                    Console.WriteLine(connectedUsersAmount);
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }
        public void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var localEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                    var localAddress = localEndPoint.Address;
                    var localPort = localEndPoint.Port;
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}, From : {2}-{3}", data, Thread.CurrentThread.ManagedThreadId, localAddress, localPort);
                    clients.Add(new ClientData() { tcpData = client, sendedData = data});
                    string str = "";
                    Console.WriteLine("Clients data : ");
                    foreach (ClientData o in clients)
                    {
                        var endPoint = o.tcpData.Client.RemoteEndPoint as IPEndPoint;
                        var port = endPoint.Port;
                        Console.WriteLine(port + " Data : " + o.sendedData);
                    }
                    if (connectedUsersAmount >= 2)
                    {
                        foreach (ClientData clientData in clients)
                        {
                            var point = clientData.tcpData.Client.RemoteEndPoint as IPEndPoint;
                            var p = point.Port;
                            Console.WriteLine(p);
                            Console.WriteLine(localPort);
                            if (p != localPort)
                            {
                                str += clientData.sendedData + "\n";
                            }
                        }
                        Console.WriteLine("str : " + str);
                    }
                    else
                    {
                        str = "Waiting for user";
                    }
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                    Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        class ClientData
        {
            public TcpClient tcpData { get; set; }
            public string sendedData { get; set; }
        }
    }
}
