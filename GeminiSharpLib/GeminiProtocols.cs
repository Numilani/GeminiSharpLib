using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Text;

namespace GeminiSharpLib
{
    public enum StatusCode
    {
        INPUT = 10,
        SENSITIVE_INPUT = 11,
        SUCCESS = 20,
        REDIRECT_TEMPORARY = 30,
        REDIRECT = 31,
        FAILURE_TEMPORARY = 40,
        UNAVAILABLE = 41,
        CGI_ERROR = 42,
        PROXY_ERROR = 43,
        SLOW_DOWN = 44,
        FAILURE_PERMANENT = 50,
        NOT_FOUND = 51,
        GONE = 52,
        REFUSED_PROXY = 53,
        BAD_REQUEST = 59,
        CERT_REQUIRED = 60,
        CERT_UNAUTHORIZED = 61,
        CERT_INVALID = 62
    }
    public static class GeminiProtocols
    {
        public static byte[] GetHeader(StatusCode status, string meta = "")
        {
            if (meta == "" && new[] {StatusCode.INPUT, StatusCode.SENSITIVE_INPUT, StatusCode.SUCCESS, StatusCode.REDIRECT, StatusCode.REDIRECT_TEMPORARY}
                .Contains(status))
            {
                throw new SyntaxErrorException($"status code {(int) status} requires a specified meta.");
            }
            
            return Encoding.UTF8.GetBytes($"{(int) status} {meta}\r\n");
        }
    }

    public class RouteContent
    {
        public byte[] header { get; private set; }
        public byte[] body { get; private set; }
        public bool hasBody { get; private set; }

        public RouteContent(StatusCode status, string meta = "")
        {
            hasBody = false;
            header = GeminiProtocols.GetHeader(status, meta);
        }

        public RouteContent(StatusCode status, string meta, string bodyContent)
        {
            hasBody = true;
            header = GeminiProtocols.GetHeader(status, meta);
            body = Encoding.UTF8.GetBytes(bodyContent);
        }
    }

    public class GeminiURI
    {
        public string host { get; private set; }
        public string path { get; private set; }
        public string query { get; private set; }
        public List<string> parameters = new List<string>();
        
        public GeminiURI(String uri)
        {
            if (uri.Contains("?"))
            {
                //TODO: the port probably should not be hardcoded.
                host = uri.Split(":1965")[0];
                path = uri.Split(":1965")[1].Split("?")[0];
                query = uri.Split(":1965")[1].Split("?")[1];
                parameters = query.Split("&").ToList();
            }
            else
            {
                path = uri.Split(":1965")[1];
            }
        }
    }
    public delegate RouteContent ContentProviderDelegate();
}