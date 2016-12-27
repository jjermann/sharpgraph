using System.Collections.Generic;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfNode {
        public WpfNode(INode nodeBehind) {
            NodeBehind = nodeBehind;
            UpdatePropertyValues();
        }

        protected INode NodeBehind { get; }
        public string Id { get; protected set; }
        public string Label { get; protected set; }

        public string Shape { get; protected set; }

        private double CenterX { get; set; }
        private double CenterY { get; set; }
        private double X { get; set; }
        private double Y { get; set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public string Margin { get; protected set; }

        private IEnumerable<string> Styles { get; set; }
        public string FillColor { get; protected set; }
        public string StrokeColor { get; protected set; }

        private void UpdatePropertyValues() {
            Id = NodeBehind.Id;
            Label = WpfHelper.ConvertIdToText(
                NodeBehind.HasAttribute("label")
                    ? NodeBehind.GetAttribute("label")
                    : NodeBehind.Id);

            Shape = WpfHelper.ConvertIdToShape(
                NodeBehind.HasAttribute("shape", true)
                    ? NodeBehind.GetAttribute("shape", true)
                    : "ellipse");

            CenterX = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("pos").Trim('"').Split(',')[0] + "pt");
            CenterY = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("pos").Trim('"').Split(',')[1] + "pt");
            Width = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("width", true).Trim('"') + "in");
            Height = WpfHelper.StringToPixel(
                NodeBehind.GetAttribute("height", true).Trim('"') + "in");
            X = CenterX - Width/2;
            Y = CenterY - Height/2;
            Margin = $"{X},{Y},0,0";

            Styles = WpfHelper.ConvertIdToStyles(
                NodeBehind.HasAttribute("style", true)
                    ? NodeBehind.GetAttribute("style", true)
                    : null);
            FillColor = GetNodeFillColor();
            StrokeColor = GetNodeStrokeColor();
        }

        private string GetNodeFillColor() {
            if (Styles.Contains("filled")) {
                var color = WpfHelper.ConvertIdToText(
                    NodeBehind.HasAttribute("color", true)
                        ? NodeBehind.GetAttribute("color", true)
                        : null);
                var fillcolor = WpfHelper.ConvertIdToText(
                    NodeBehind.HasAttribute("fillcolor", true)
                        ? NodeBehind.GetAttribute("fillcolor", true)
                        : null);
                return fillcolor ?? color;
            }
            return null;
        }

        private string GetNodeStrokeColor() {
            var color = WpfHelper.ConvertIdToText(
                NodeBehind.HasAttribute("color", true)
                    ? NodeBehind.GetAttribute("color", true)
                    : "black");
            return color;
        }
    }
}