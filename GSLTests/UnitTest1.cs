using GeminiSharpLib;
using NUnit.Framework;

namespace GSLTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            GeminiListener listener = new GeminiListener();
            listener.run();
        }
    }
}