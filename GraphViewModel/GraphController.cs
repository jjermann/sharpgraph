using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class GraphController {
        private string _inputFile;
        private IGraph _originalGraph;
        private IGraph _originalGraphLayout;

        public IEnumerable<WpfNode> WpfNodes { get; private set; }
        public IEnumerable<WpfEdge> WpfEdges { get; private set; }
        public IEnumerable<WpfSubGraph> WpfSubGraphs { get; private set; }

        public string Foo => GraphViewModelHelper.PosToGeometry("e,99.992,50.161 79.71,90.149 84.609,79.337 90.046,68.253 95.1,58.884");
        //TODO: Move to Subgraphs and check if the attribute exists...
        //private double LowerLeftX => GraphViewModelHelper.StringToPixel(_originalGraphLayout.GetAttribute("bb").Split(',')[0] + "pt");
        //private double LowerLeftY => GraphViewModelHelper.StringToPixel(_originalGraphLayout.GetAttribute("bb").Split(',')[1] + "pt");
        //private double UpperRightX => GraphViewModelHelper.StringToPixel(_originalGraphLayout.GetAttribute("bb").Split(',')[2] + "pt");
        //private double UpperRightY => GraphViewModelHelper.StringToPixel(_originalGraphLayout.GetAttribute("bb").Split(',')[3] + "pt");
        //public double Width => UpperRightX - LowerLeftX;
        //public double Height => UpperRightY - LowerLeftY;

        public string InputFile {
            get { return _inputFile; }
            set { InitializeGraph(value); }
        }

        public GraphController() {
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
