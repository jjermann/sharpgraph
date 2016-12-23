using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfNode {
        private readonly INode _nodeBehind;

        public WpfNode(INode nodeBehind) {
            _nodeBehind = nodeBehind;
        }

        public string Label => _nodeBehind.HasAttribute("label") ? _nodeBehind.GetAttribute("label") : _nodeBehind.Id;
        public double Width => GraphViewModelHelper.StringToPixel(_nodeBehind.GetAttribute("width").Trim('"') + "in");
        public double Height => GraphViewModelHelper.StringToPixel(_nodeBehind.GetAttribute("height").Trim('"') + "in");
        private double CenterX => GraphViewModelHelper.StringToPixel(
            _nodeBehind.GetAttribute("pos").Trim('"').Split(',')[0] + "pt");
        private double CenterY => GraphViewModelHelper.StringToPixel(
            _nodeBehind.GetAttribute("pos").Trim('"').Split(',')[1] + "pt");
        private double X => CenterX - Width/2;
        private double Y => CenterY - Height/2;
        public string Margin => $"{X},{Y}";
    }
}
