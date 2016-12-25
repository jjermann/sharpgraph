using System;
using System.IO;

namespace SharpGraph {
    public static class ConsoleProgram {
        private const string DefaultInputFile = @"Examples\example.dot";

        private static void Main(string[] args) {
            var file = args.Length > 0 ? new FileInfo(args[0]) : new FileInfo(DefaultInputFile);
            if (!file.Exists) {
                throw new Exception($"File {file.FullName} not found!");
            }

            var graph = GraphParser.GraphParser.GetGraph(file);
            var graphDot = graph.ToDot();
            Console.Write(graphDot);
            Console.WriteLine("Press any key...");
            Console.ReadKey();

            try {
                var originalDot = file.OpenText().ReadToEnd();
                var initialDot = GraphParser.GraphParser.GetGraphLayoutDot(originalDot);
                var reparsedDot = GraphParser.GraphParser.GetGraphLayoutDot(graphDot);
                var diff = DiffHelper.GetDiff(initialDot, reparsedDot);
                //var initialReparsed = GraphParser.GraphParser.GetGraph(initialDot).ToDot();
                //var reparsedReparsed = GraphParser.GraphParser.GetGraph(reparsedDot).ToDot();
                //var diff = DiffHelper.GetDiff(initialReparsed, reparsedReparsed);
                Console.Clear();
                Console.Write(diff);
                Console.WriteLine("Press any key to generate outOriginal.png, out.png and layout.dot...");
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
            GraphParser.GraphParser.GetGraphImage(originalDot).Save("outOriginal.png");
            GraphParser.GraphParser.GetGraphImage(graphDot).Save("out.png");
            File.WriteAllText("layout.dot", reparsedDot);
        }
    }
}
