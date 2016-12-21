using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class GraphControler {
        private string _inputFile;
        private IGraph _originalGraph;
        private IGraph _originalGraphLayout;

        public IEnumerable<WpfNode> WpfNodes { get; private set; }
        public IEnumerable<WpfEdge> WpfEdges { get; private set; }
        public IEnumerable<WpfSubGraph> WpfSubGraphs { get; private set; }
        public string LowerLeftX => _originalGraphLayout.GetAttribute("bb").Split(',')[0];
        public string LowerLeftY => _originalGraphLayout.GetAttribute("bb").Split(',')[1];
        public string UpperRightX => _originalGraphLayout.GetAttribute("bb").Split(',')[2];
        public string UpperRightY => _originalGraphLayout.GetAttribute("bb").Split(',')[3];

        public string InputFile {
            get { return _inputFile; }
            set { InitializeGraph(value); }
        }

        public GraphControler() {
            InputFile = @"example.dot";
        }

        private void InitializeGraph(string filename) {
            _inputFile = filename;
            _originalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(_inputFile));
            ReloadGraphLayout();
        }

        private void ReloadGraphLayout() {
            _originalGraphLayout = GraphParser.GraphParser.GetGraphLayout(_originalGraph.ToDot());
            WpfNodes = _originalGraphLayout.GetNodes().Select(n => new WpfNode(n));
            WpfEdges = _originalGraphLayout.GetEdges().Select(e => new WpfEdge(e));
            WpfSubGraphs = _originalGraphLayout.GetSubGraphs().Select(g => new WpfSubGraph(g));
        }
    }
}
