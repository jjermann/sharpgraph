using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph.DotParser {
    [TestFixture]
    public static class DotParserTests {
        [SetUp]
        public static void Init() {}

        [Test]
        public static void GetParseTreeTest() {
            var executingAssembly = Assembly.GetExecutingAssembly();

            Stream stream = null;
            try {
                stream = executingAssembly.GetManifestResourceStream(@"SharpGraph.TestExamples.example.dot");
                Assert.IsTrue(stream != null);
                using (var reader = new StreamReader(stream)) {
                    stream = null;
                    try {
                        DotParser.GetParseTree(reader);
                    } catch (Exception e) {
                        Assert.Fail(e.Message);
                    }
                }
            } finally {
                stream?.Dispose();
            }
        }
    }
}