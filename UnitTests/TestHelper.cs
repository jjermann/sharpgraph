using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SharpGraph {
    internal static class TestHelper {
        private static string s_exampleDot;

        public static string ExampleDot {
            get {
                if (s_exampleDot != null) {
                    return s_exampleDot;
                }

                var executingAssembly = Assembly.GetExecutingAssembly();
                Stream stream = null;
                try {
                    stream =
                        executingAssembly.GetManifestResourceStream(@"SharpGraph.TestExamples.example.gv");
                    Assert.IsTrue(stream != null);
                    using (var reader = new StreamReader(stream)) {
                        stream = null;
                        s_exampleDot = reader.ReadToEnd();
                    }
                } finally {
                    stream?.Dispose();
                }
                return s_exampleDot;
            }
        }
    }
}