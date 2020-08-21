using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        
        [Test]
        public void test_Gemini0URI_Constructor()
        {
            GeminiUri uri = new GeminiUri("gemini://localhost:1965/foo/abc.xyz?id=bar&stuff=baz");
            
            Assert.AreEqual(uri.Host, "gemini://localhost");
            Assert.AreEqual(uri.Path, "/foo/abc.xyz");
            Assert.AreEqual(uri.Query, "id=bar&stuff=baz");
        }

        [Test]
        public void testListenerLoop()
        {
            GeminiListener listener = new GeminiListener(new X509Certificate("C:\\x509\\geminiserver.pfx"));

            Dictionary<String, ContentProviderDelegate> routes = new Dictionary<string, ContentProviderDelegate>();
            routes.Add("/foo", delegate { return new RouteContent(StatusCode.SUCCESS, "SUCCESS", "Foo"); });
            routes.Add("/bar", delegate { return new RouteContent(StatusCode.SUCCESS, "SUCCESS", "Bar"); });
            
            
            // while (true)
            // {
                listener.Listen(routes);   
            // }
        }

        [Test]
        public void testSuccessHeaderWithSuppliedMeta()
        {
            byte[] header = GeminiProtocols.GetHeader(StatusCode.SUCCESS, "FOO SUCCEEDED");
            Assert.AreEqual(Encoding.UTF8.GetBytes("20 FOO SUCCEEDED\r\n"), header);
        }

        [Test]
        public void testSuccessHeaderWithNoMeta()
        {
            Assert.Throws<SyntaxErrorException>(delegate { byte[] header = GeminiProtocols.GetHeader(StatusCode.SUCCESS); });
        }
    }
}