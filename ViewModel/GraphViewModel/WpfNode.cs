using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfNode {
        public WpfNode(INode nodeBehind) {
            NodeBehind = nodeBehind;
            UpdatePropertyValues();
        }

        protected INode NodeBehind { get; }
        private double CenterX { get; set; }
        private double CenterY { get; set; }
        private double X { get; set; }
        private double Y { get; set; }
        public string Id { get; protected set; }
        public string Label { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public string Margin { get; protected set; }
        public string Shape { get; protected set; }

        private void UpdatePropertyValues() {
            CenterX = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("pos").Trim('"').Split(',')[0] + "pt");
            CenterY = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("pos").Trim('"').Split(',')[1] + "pt");
            Width = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("width").Trim('"') + "in");
            Height = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("height").Trim('"') + "in");
            X = CenterX - Width/2;
            Y = CenterY - Height/2;
            Id = NodeBehind.Id;
            Label = WpfHelper.ConvertIdToText(
                NodeBehind.HasAttribute("label") ? NodeBehind.GetAttribute("label") : NodeBehind.Id);
            Margin = $"{X},{Y},0,0";
            Shape = WpfHelper.ConvertIdToShape(
                NodeBehind.HasAttribute("shape") ? NodeBehind.GetAttribute("shape") : "ellipse");
        }
    }
}