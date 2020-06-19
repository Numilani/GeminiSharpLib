using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GeminiSharpLib
{
    public class GeminiListener
    {
        private const int portNum = 1965;
        private const string CertFilename = "C:\\x509\\geminiserver.pfx";
        
        public bool done = false;

        public void run()
        {
            var listener = new TcpListener(IPAddress.Any, portNum);
            
            listener.Start();
            while (!done)
            {
                Console.Write("Waiting for connection");
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Connection accepted");
                SslStream stream = new SslStream(client.GetStream());
                
                stream.AuthenticateAsServer(new X509Certificate(CertFilename));

                byte[] byteHeader = Encoding.UTF8.GetBytes("20 SUCCESS\r\n");
                byte[] byteBody = Encoding.UTF8.GetBytes("This is a test.\r\n");

                try
                {
                    byte[] request = new byte[1024];
                    stream.Read(request);
                    Console.WriteLine("Request is: " + Encoding.UTF8.GetString(request).TrimEnd('\u0000'));
                    stream.Write(byteHeader, 0, byteHeader.Length);
                    stream.Write(byteBody, 0, byteBody.Length);
                    stream.Close();
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