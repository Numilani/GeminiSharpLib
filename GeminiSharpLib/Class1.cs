using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GeminiSharpLib
{
    public delegate string GeminiRouteHandler();
    
    public class GeminiListener
    {
        // TODO: These should be moved to a config file.
        // private const int portNum = 1965;
        // private const string CertFilename = "C:\\x509\\geminiserver.pfx";

        private X509Certificate _serverCert;
        private TcpListener _listener;
        
        public GeminiListener(X509Certificate certificate, int portNum)
        {
            _serverCert = certificate;
            _listener = new TcpListener(IPAddress.Any, portNum);
        }

        public void Listen(Dictionary<String, GeminiRouteHandler> routeHandlers)
        {
            _listener.Start();
            
            TcpClient client = _listener.AcceptTcpClient();

            SslStream stream = new SslStream(client.GetStream());
            stream.AuthenticateAsServer(_serverCert);

            try
            {
                byte[] request = new byte[1024];
                stream.Read(request);
                String uri = Encoding.UTF8.GetString(request).TrimEnd('\u0000');
                
                List<String> path = new List<string>(uri.Split("/"));
                path.RemoveRange(0,2);
                
                // TestResponse(stream);
                // if (routeHandlers.ContainsKey(path[1]))
                // {
                    // Success(stream, routeHandlers[path[1]]);
                    Success(stream);
                // }

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            _listener.Stop();
        }

        // private static void Success(SslStream stream, GeminiRouteHandler body)
        private static void Success(SslStream stream)
        {
            byte[] byteHeader = Encoding.UTF8.GetBytes("20 SUCCESS\r\n");
            byte[] byteBody = Encoding.UTF8.GetBytes("This is a test\r\n");

            stream.Write(byteHeader, 0, byteHeader.Length);
            stream.Write(byteBody, 0, byteBody.Length);
        }
    }
}