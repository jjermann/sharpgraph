using System;
using System.IO;
using SharpGraph.GraphViewModel;

namespace ExampleSimpleView {
    public sealed class GraphController {
        public GraphController() {
            if (Environment.GetCommandLineArgs().Length <= 1) {
                throw new ArgumentException("No filename argument given!");
            }
            var filename = Environment.GetCommandLineArgs()[1];
            var graph = SharpGraph.GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            var layoutGraph = SharpGraph.GraphParser.GraphParser.GetGraphLayout(graph.ToDot());
            Graph = new WpfGraph(layoutGraph);
        }

        public WpfGraph Graph { get; }
    }
}