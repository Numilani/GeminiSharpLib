using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GeminiSharpLib
{
    public class GeminiListener
    {

        private X509Certificate _serverCert;
        private TcpListener _listener;
        
        public GeminiListener(X509Certificate certificate, int portNum = 1965)
        {
            _serverCert = certificate;
            _listener = new TcpListener(IPAddress.Any, portNum);
        }

        public void Listen(Dictionary<String, ContentProviderDelegate> routeHandlers)
        {
            _listener.Start();
            
            Console.WriteLine("Listening...");
            TcpClient client = _listener.AcceptTcpClient();

            SslStream stream = new SslStream(client.GetStream());
            stream.AuthenticateAsServer(_serverCert);

            try
            {
                byte[] request = new byte[1024];
                stream.Read(request);
                GeminiURI uri = new GeminiURI(Encoding.UTF8.GetString(request).TrimEnd('\u0000').TrimEnd('\r', '\n'));

                if (routeHandlers.ContainsKey(uri.path))
                {
                    Success(stream, routeHandlers[uri.path]);
                }
                else
                {
                    Failure(stream, GeminiProtocols.GetHeader(StatusCode.NOT_FOUND));
                    Console.WriteLine("no route for " + uri.path + " was found.");
                }

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            _listener.Stop();
        }

        public static void Success(SslStream stream, ContentProviderDelegate bodyContent)
        {
            byte[] header = GeminiProtocols.GetHeader(StatusCode.SUCCESS, "text/plain");
            byte[] body = Encoding.UTF8.GetBytes(bodyContent());

            stream.Write(header, 0, header.Length);
            stream.Write(body, 0, body.Length);
        }

        private static void Failure(SslStream stream, byte[] header)
        {
            stream.Write(header, 0, header.Length);
        }
    }
    
    public delegate void RouteDelegate(SslStream stream);
}