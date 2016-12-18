using System;
using System.IO;

namespace SharpGraph {
    public static class ConsoleProgram {
        private const string DefaultInputFile = @"Examples\simple.dot";

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
                var initialDot = GraphParser.GraphParser.GetGraphLayoutDot(file.OpenText().ReadToEnd());
                var reparsedDot = GraphParser.GraphParser.GetGraphLayoutDot(graphDot);
                var diff = DiffHelper.GetDiff(initialDot, reparsedDot);
                //var initialReparsed = GraphParser.GraphParser.GetGraph(initialDot).ToDot();
                //var reparsedReparsed = GraphParser.GraphParser.GetGraph(reparsedDot).ToDot();
                //var diff = DiffHelper.GetDiff(initialReparsed, reparsedReparsed);
                Console.Clear();
                Console.Write(diff);
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            } catch (Exception e) {
                Console.Clear();
                Console.Write(e.Message);
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }
    }
}
