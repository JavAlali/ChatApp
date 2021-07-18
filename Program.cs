using System;
using System.Net.Sockets;
using System.Threading;

namespace AppClient2
{
    class Program
    {
        public static Int32 port = 13000;
        public static TcpClient client;
        public static NetworkStream stream;
        static void Main(string[] args)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                client = new TcpClient("10.0.0.13", port);
                stream = client.GetStream();
                Connect("First");
                Connect("Second");
                Connect("Third");
            }).Start();
            Console.ReadLine();
        }
        static void Connect(String message)
        {
            try
            {
                // Translate the Message into ASCII.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);
                // Bytes Array to receive Server Response.
                data = new Byte[256];
                String response = String.Empty;
                // Read the Tcp Server Response Bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", response);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        static void Disconnect()
        {
            stream.Close();
            client.Close();
            Console.Read();
        }
    }
}