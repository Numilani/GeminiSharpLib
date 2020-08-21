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
                HandleRequest(routeHandlers, stream);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            _listener.Stop();
        }

        private static void HandleRequest(Dictionary<string, ContentProviderDelegate> routeHandlers, SslStream stream)
        {
            byte[] request = new byte[1024];
            stream.Read(request);
            GeminiUri uri = new GeminiUri(Encoding.UTF8.GetString(request).TrimEnd('\u0000').TrimEnd('\r', '\n'));

            if (routeHandlers.ContainsKey(uri.Path))
            {
                DeliverContent(stream, routeHandlers[uri.Path]);
            }
            else
            {
                stream.Write(GeminiProtocols.GetHeader(StatusCode.NOT_FOUND), 0,
                    GeminiProtocols.GetHeader(StatusCode.NOT_FOUND).Length);
                Console.WriteLine("no route for " + uri.Path + " was found.");
            }
        }

        public static void DeliverContent(SslStream stream, ContentProviderDelegate contentProvider)
        {
            RouteContent content = contentProvider();
            
            stream.Write(content.Header, 0, content.Header.Length);
            if (content.HasBody)
            {
                stream.Write(content.Body, 0, content.Body.Length);   
            }
        }
    }
}