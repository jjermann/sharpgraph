using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SharpGraph;

namespace ExampleConsoleProgram {
    public static class ConsoleProgram {
        private const string DefaultInputFile = @"example.gv";

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "gv")]
        private static void Main(string[] args) {
            var file = args.Length > 0 ? new FileInfo(args[0]) : new FileInfo(DefaultInputFile);
            if (!file.Exists) {
                throw new FileNotFoundException(FormattableString.Invariant($"File {file.FullName} not found!"));
            }

            var graph = GraphParser.GetGraph(file);
            var graphDot = graph.ToDot();
            Console.Write(graphDot);
            Console.WriteLine("Press any key...");
            Console.ReadKey();

            try {
                var originalDot = file.OpenText().ReadToEnd();
                var initialDot = GraphParser.GetGraphLayoutDot(originalDot);
                var reparsedDot = GraphParser.GetGraphLayoutDot(graphDot);
                var diff = DiffHelper.GetDiff(initialDot, reparsedDot);
                //var initialReparsed = GraphParser.GraphParser.GetGraph(initialDot).ToDot();
                //var reparsedReparsed = GraphParser.GraphParser.GetGraph(reparsedDot).ToDot();
                //var diff = DiffHelper.GetDiff(initialReparsed, reparsedReparsed);
                Console.Clear();
                Console.Write(diff);
                Console.WriteLine("Press any key to generate outOriginal.png, out.png and layout.gv...");
                Console.ReadKey();

                GenerateOutput(originalDot, graphDot, reparsedDot);

                Console.Clear();
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            } catch (Exception e) {
                Console.Clear();
                Console.Write(e.Message);
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }

        private static void GenerateOutput(string originalDot, string graphDot, string reparsedDot) {
            GraphParser.GetGraphImage(originalDot).Save("outOriginal.png");
            GraphParser.GetGraphImage(graphDot).Save("out.png");
            File.WriteAllText("layout.gv", reparsedDot);
        }
    }
}