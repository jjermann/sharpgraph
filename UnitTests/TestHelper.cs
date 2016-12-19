using System;
using System.IO;
using System.Reflection;

namespace SharpGraph {
    internal static class TestHelper {
        public static string ExampleDot { get; }

        static TestHelper() {
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
    }
}
