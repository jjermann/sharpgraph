using System.IO;
using SharpGraph.ExternalRunners;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphParser {
    public static class GraphParser {
        public static IGraph GetGraph(StreamReader reader) {
            var listener = new GraphListener();
            DotParser.DotParser.GetParseTree(reader, listener);
            return listener.ParsedGraph;
        }

        public static IGraph GetGraph(string input) {
            using (var stream = new MemoryStream()) {
                using (var writer = new StreamWriter(stream)) {
                    writer.Write(input);
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream)) {
                        return GetGraph(reader);
                    }
                }
            }
        }

        public static IGraph GetGraph(FileInfo file) {
            using (var reader = file.OpenText()) {
                return GetGraph(reader);
            }
        }

        public static IGraph GetGraphLayout(string graphLayoutDot) {
            return GetGraph(GetGraphLayoutDot(graphLayoutDot));
        }

        public static string GetGraphLayoutDot(string graphDot) {
            return new DotExeRunner().GetGraphLayout(graphDot);
        }
    }
}

