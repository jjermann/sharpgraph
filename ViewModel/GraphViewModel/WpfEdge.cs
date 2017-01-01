using System.Diagnostics.CodeAnalysis;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WpfEdge {
        public WpfEdge(IEdge edgeBehind) {
            EdgeBehind = edgeBehind;
            UpdatePropertyValues();
        }

        protected IEdge EdgeBehind { get; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Label { get; protected set; }
        public string Geometry { get; protected set; }
        public bool HasArrowHead { get; protected set; }
        public string ArrowHeadGeometry { get; protected set; }

        private void UpdatePropertyValues() {
            Label = WpfHelper.ConvertIdToText(
                EdgeBehind.HasAttribute("label")
                    ? EdgeBehind.GetAttribute("label")
                    : null);
            Geometry = WpfHelper.PosToGeometry(WpfHelper.ConvertIdToText(
                EdgeBehind.GetAttribute("pos")));
            HasArrowHead = EdgeBehind.Root.IsDirected;
            ArrowHeadGeometry = WpfHelper.PosToArrowHeadGeometry(WpfHelper.ConvertIdToText(
                EdgeBehind.GetAttribute("pos")));
        }
    }
}