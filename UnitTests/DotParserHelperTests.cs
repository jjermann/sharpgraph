using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public static class DotParserHelperTests {
        [SetUp]
        public static void Init() {}

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [Test]
        public static void GetParseTreeTest() {
            var executingAssembly = Assembly.GetExecutingAssembly();

            Stream stream = null;
            try {
                stream = executingAssembly.GetManifestResourceStream(@"SharpGraph.TestExamples.example.gv");
                Assert.IsTrue(stream != null);
                using (var reader = new StreamReader(stream)) {
                    stream = null;
                    try {
                        DotParserHelper.GetParseTree(reader);
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