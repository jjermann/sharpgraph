using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfEdge {
        public WpfEdge(IEdge edgeBehind) {
            EdgeBehind = edgeBehind;
            UpdatePropertyValues();
        }

        protected IEdge EdgeBehind { get; }
        public string Label { get; protected set; }
        public string Geometry { get; protected set; }

        private void UpdatePropertyValues() {
            Label = EdgeBehind.HasAttribute("label") ? EdgeBehind.GetAttribute("label") : null;
            Geometry = WpfHelper.PosToGeometry(EdgeBehind.GetAttribute("pos"));
        }
    }
}