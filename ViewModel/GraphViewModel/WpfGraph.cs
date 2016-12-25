﻿using System.Collections.Generic;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfGraph : WpfSubGraph {
        private readonly IGraph _graphBehind;

        public WpfGraph(IGraph graphBehind) : base(graphBehind) {
            _graphBehind = graphBehind;
            WpfNodes = _graphBehind.GetNodes().Select(n => new WpfNode(n));
            WpfEdges = _graphBehind.GetEdges().Select(e => new WpfEdge(e));
            WpfSubGraphs = _graphBehind.GetSubGraphs().Select(g => new WpfSubGraph(g));
        }

        public IEnumerable<WpfNode> WpfNodes { get; private set; }
        public IEnumerable<WpfEdge> WpfEdges { get; private set; }
        public IEnumerable<WpfSubGraph> WpfSubGraphs { get; private set; }
        public string DotContent => _graphBehind.ToDot();

        public override string Label
            =>
            WpfHelper.ConvertIdToText(_graphBehind.HasAttribute("label")
                ? _graphBehind.GetAttribute("label")
                : _graphBehind.Id);
    }
}