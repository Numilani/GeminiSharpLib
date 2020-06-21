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
            GeminiURI uri = new GeminiURI("gemini://localhost:1965/foo/abc.xyz?id=bar&stuff=baz");
            
            Assert.AreEqual(uri.host, "gemini://localhost");
            Assert.AreEqual(uri.path, "/foo/abc.xyz");
            Assert.AreEqual(uri.query, "id=bar&stuff=baz");
        }

        [Test]
        public void testListenerLoop()
        {
            GeminiListener listener = new GeminiListener(new X509Certificate("C:\\x509\\geminiserver.pfx"));

            Dictionary<String, ContentProviderDelegate> routes = new Dictionary<string, ContentProviderDelegate>();
            routes.Add("/foo", delegate { return "foo"; });
            routes.Add("/bar", delegate { return "bar"; });
            
            
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