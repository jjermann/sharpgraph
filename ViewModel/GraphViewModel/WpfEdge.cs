using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfEdge {
        private readonly IEdge _edgeBehind;

        public WpfEdge(IEdge edgeBehind) {
            _edgeBehind = edgeBehind;
        }

        public string Label => _edgeBehind.HasAttribute("label") ? _edgeBehind.GetAttribute("label") : null;
        public string Geometry => WpfHelper.PosToGeometry(_edgeBehind.GetAttribute("pos"));

    }
}
