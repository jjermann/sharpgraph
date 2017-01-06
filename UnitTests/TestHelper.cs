using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph {
    internal static class TestHelper {
        static TestHelper() {
            var executingAssembly = Assembly.GetExecutingAssembly();
            Stream stream = null;
            try {
                stream = executingAssembly.GetManifestResourceStream(@"SharpGraph.TestExamples.example.dot");
                Assert.IsTrue(stream != null);
                using (var reader = new StreamReader(stream)) {
                    stream = null;
                    ExampleDot = reader.ReadToEnd();
                }
            } finally {
                stream?.Dispose();
            }
        }

        public static string ExampleDot { get; }
    }
}