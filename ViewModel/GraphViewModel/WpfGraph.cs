using System.Collections.Generic;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfGraph : WpfSubGraph {
        public WpfGraph(IGraph graphBehind) : base(graphBehind) {
            GraphBehind = graphBehind;
            UpdatePropertyValues();
        }

        protected IGraph GraphBehind { get; }
        public IEnumerable<WpfNode> WpfNodes { get; protected set; }
        public IEnumerable<WpfEdge> WpfEdges { get; protected set; }
        public IEnumerable<WpfSubGraph> WpfSubGraphs { get; protected set; }
        public string DotContent { get; protected set; }

        private void UpdatePropertyValues() {
            WpfNodes = GraphBehind.GetNodes().Select(n => new WpfNode(n));
            WpfEdges = GraphBehind.GetEdges().Select(e => new WpfEdge(e));
            WpfSubGraphs = GraphBehind.GetSubGraphs().Select(g => new WpfSubGraph(g));
            DotContent = GraphBehind.ToDot();
            Label = WpfHelper.ConvertIdToText(GraphBehind.HasAttribute("label")
                ? GraphBehind.GetAttribute("label")
                : GraphBehind.Id);
        }
    }
}