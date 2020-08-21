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
        public byte[] Header { get; private set; }
        public byte[] Body { get; private set; }
        public bool HasBody { get; private set; }

        public RouteContent(StatusCode status, string meta = "")
        {
            HasBody = false;
            Header = GeminiProtocols.GetHeader(status, meta);
        }

        public RouteContent(StatusCode status, string meta, string bodyContent)
        {
            HasBody = true;
            Header = GeminiProtocols.GetHeader(status, meta);
            Body = Encoding.UTF8.GetBytes(bodyContent);
        }
    }

    public class GeminiUri
    {
        public string Host { get; private set; }
        public string Path { get; private set; }
        public string Query { get; private set; }
        public List<string> Parameters = new List<string>();
        
        public GeminiUri(String uri)
        {
            if (uri.Contains("?"))
            {
                //TODO: the port probably should not be hardcoded.
                Host = uri.Split(":1965")[0];
                Path = uri.Split(":1965")[1].Split("?")[0];
                Query = uri.Split(":1965")[1].Split("?")[1];
                Parameters = Query.Split("&").ToList();
            }
            else
            {
                Path = uri.Split(":1965")[1];
            }
        }
    }
    public delegate RouteContent ContentProviderDelegate();
}