using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GeminiSharpLib
{
    public class GeminiListener
    {
        private const int portNum = 1965;

        public void run()
        {
            bool done = false;
            
            var listener = new TcpListener(IPAddress.Any, portNum);
            
            listener.Start();
            while (!done)
            {
                Console.Write("Waiting for connection");
                TcpClient client = listener.AcceptTcpClient();
                
                Console.WriteLine("Connection accepted");
                NetworkStream ns = client.GetStream();

                byte[] bytetime = Encoding.ASCII.GetBytes(DateTime.Now.ToString());

                try
                {
                    ns.Write(bytetime, 0, bytetime.Length);
                    ns.Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                done = true;
            }
            listener.Stop();
        }
    }
}