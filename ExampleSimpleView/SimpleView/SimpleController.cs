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
            var graph = GraphParser.GetGraph(new FileInfo(filename));
            var layoutGraph = GraphParser.GetGraphLayout(graph.ToDot());
            Graph = new WpfGraph(layoutGraph);
        }

        public WpfGraph Graph { get; }
    }
}