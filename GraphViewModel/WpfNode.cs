using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfNode {
        private readonly INode _nodeBehind;

        public WpfNode(INode nodeBehind) {
            _nodeBehind = nodeBehind;
        }

        private double CenterX => WpfHelper.StringToPixel(
            _nodeBehind.GetAttribute("pos").Trim('"').Split(',')[0] + "pt");
        private double CenterY => WpfHelper.StringToPixel(
            _nodeBehind.GetAttribute("pos").Trim('"').Split(',')[1] + "pt");
        private double X => CenterX - Width / 2;
        private double Y => CenterY - Height / 2;

        public string Id => _nodeBehind.Id;
        public string Label => WpfHelper.ConvertIdToText(
            _nodeBehind.HasAttribute("label") ? _nodeBehind.GetAttribute("label") : _nodeBehind.Id);
        public double Width => WpfHelper.StringToPixel(
            _nodeBehind.GetAttribute("width").Trim('"') + "in");
        public double Height => WpfHelper.StringToPixel(
            _nodeBehind.GetAttribute("height").Trim('"') + "in");
        public string Margin => $"{X},{Y},0,0";
        public string Shape => WpfHelper.ConvertIdToShape(
            _nodeBehind.HasAttribute("shape") ? _nodeBehind.GetAttribute("shape") : "ellipse");
    }
}
