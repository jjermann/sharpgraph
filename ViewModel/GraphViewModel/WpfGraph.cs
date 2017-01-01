using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WpfGraph : WpfSubGraph {
        public WpfGraph(IGraph graphBehind, IList<string> selectedNodeIds = null) : base(graphBehind) {
            GraphBehind = graphBehind;
            SelectedNodeIds = selectedNodeIds;
            WpfNodes = SelectedNodeIds == null
                ? GraphBehind.GetNodes().Select(n => new WpfNode(this, n)).ToList()
                : GraphBehind.GetNodes().Select(n => new WpfNode(this, n, SelectedNodeIds.Contains(n.Id))).ToList();
            WpfEdges = GraphBehind.GetEdges().Select(e => new WpfEdge(e)).ToList();
            WpfSubGraphs = GraphBehind.GetSubGraphs().Select(g => new WpfSubGraph(g)).ToList();
            DotContent = GraphBehind.ToDot();

            UpdatePropertyValues();
        }

        protected IGraph GraphBehind { get; }
        protected IList<string> SelectedNodeIds { get; }
        public IList<WpfNode> WpfNodes { get; protected set; }
        public IList<WpfEdge> WpfEdges { get; protected set; }
        public IList<WpfSubGraph> WpfSubGraphs { get; protected set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string DotContent { get; protected set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsDirected { get; protected set; }
        public string Pad { get; protected set; }

        private void UpdatePropertyValues() {
            IsDirected = GraphBehind.IsDirected;
            Label = WpfHelper.ConvertIdToText(
                GraphBehind.HasAttribute("label")
                    ? GraphBehind.GetAttribute("label")
                    : GraphBehind.Id);
            FillColor = GetGraphFillColor();
            Pad = GetGraphPad();
        }

        private string GetGraphFillColor() {
            var bgColor = WpfHelper.ConvertIdToText(
                GraphBehind.HasAttribute("bgcolor", true)
                    ? GraphBehind.GetAttribute("bgcolor", true)
                    : null);
            return bgColor;
        }

        private string GetGraphPad() {
            var padStr = WpfHelper.ConvertIdToText(
                GraphBehind.HasAttribute("pad")
                    ? GraphBehind.GetAttribute("pad")
                    : "0.0555");
            return string.Join(",", padStr.Split(',').Select(s => WpfHelper.StringToPixel(s + "in")));
        }

        public event EventHandler Changed;

        internal void RaiseChanged() {
            Changed?.Invoke(this, new EventArgs());
        }
    }
}