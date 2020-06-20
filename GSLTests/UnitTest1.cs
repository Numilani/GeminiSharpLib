using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GeminiSharpLib;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace GSLTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public string fooResponse()
        {
            return "Welcome to FOOLAND, the funnest place on earth. Check out our other page, the CrowBAR.";
        }
        
        public string barResponse()
        {
            return "Welcome to the CrowBAR; what can I get you to drink?";
        }

        [Test]
        public void Test1()
        {
            GeminiListener listener = new GeminiListener(new X509Certificate("C:\\x509\\geminiserver.pfx"), 1965);
            Dictionary<String, GeminiRouteHandler> routes = new Dictionary<string, GeminiRouteHandler>();
            routes.Add("foo", fooResponse);
            routes.Add("bar", barResponse);
            // while (true)
            // {
                listener.Listen(routes);   
            // }
        }
    }
}