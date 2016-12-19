using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph.ExternalRunners {
    [TestFixture]
    public class DotExeRunnerTests {
        private static readonly string ExampleDot;

        static DotExeRunnerTests() {
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream(@"SharpGraph.TestExamples.example.dot")) {
                if (stream == null) {
                    throw new Exception("example.dot not found!");
                }
                using (var reader = new StreamReader(stream)) {
                    ExampleDot = reader.ReadToEnd();
                }
            }
        }

        [SetUp]
        public void Init() {}

        [Test]
        public void GetGraphLayoutTest() {
            try {
                new DotExeRunner().GetGraphLayout(ExampleDot);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}
