using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph.DotParser {
    [TestFixture]
    public class DotParserTests {
        [SetUp]
        public void Init() {}

        [Test]
        public void GetParseTreeTest() {
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream(@"SharpGraph.TestExamples.example.dot")) {
                if (stream == null) {
                    throw new Exception("example.dot not found!");
                }
                using (var reader = new StreamReader(stream)) {
                    try {
                        DotParser.GetParseTree(reader, null);
                    } catch (Exception e) {
                        Assert.Fail(e.Message);
                    }
                }
            }
        }
    }
}
