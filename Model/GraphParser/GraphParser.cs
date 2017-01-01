using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using SharpGraph.ExternalRunners;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphParser {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class GraphParser {
        public static readonly Func<string, string> LayoutGenerator =
            new DotExeRunner<string>("-Tdot", reader => reader.ReadToEnd()).GetOutput;

        public static readonly Func<string, Image> ImageGenerator =
            new DotExeRunner<Image>("-Tpng", reader => Image.FromStream(reader.BaseStream)).GetOutput;

        public static readonly Func<string, bool> SyntaxChecker =
            new DotExeRunner<bool>("-Tcanon", reader => {
                reader.ReadToEnd();
                return true;
            }).GetOutput;

        public static IGraph GetGraph(StreamReader reader) {
            var tree = DotParser.DotParser.GetParseTree(reader);
            return new GraphVisitor().Visit(tree);
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
            return LayoutGenerator(graphDot);
        }

        public static Image GetGraphImage(string graphDot) {
            return ImageGenerator(graphDot);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static bool CheckSyntax(string graphDot) {
            return SyntaxChecker(graphDot);
        }
    }
}