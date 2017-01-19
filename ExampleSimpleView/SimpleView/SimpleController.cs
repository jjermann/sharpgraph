using System;
using System.IO;
using SharpGraph;

namespace ExampleSimpleView {
    public sealed class SimpleController {
        public SimpleController() {
            if (Environment.GetCommandLineArgs().Length <= 1) {
                throw new ArgumentException("No filename argument given!");
            }
            var filename = Environment.GetCommandLineArgs()[1];
            var dotContent = File.ReadAllText(filename);
            try {
                GraphParser.CheckSyntax(dotContent);
            } catch (Exception e) {
                throw new ArgumentException("Invalid graph syntax:" + Environment.NewLine + e.Message);
            }
            var graph = GraphParser.GetGraph(dotContent);
            var layoutGraph = GraphParser.GetGraphLayout(graph.ToDot());
            Graph = new WpfGraph(layoutGraph);
        }

        public WpfGraph Graph { get; }
    }
}