using System.Collections.Generic;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfSubGraph {
        public WpfSubGraph(ISubGraph subGraphBehind) {
            SubGraphBehind = subGraphBehind;
            UpdatePropertyValues();
        }

        private ISubGraph SubGraphBehind { get; }
        public string Label { get; protected set; }
        public bool IsCluster { get; protected set; }

        public bool HasBoundingBox { get; protected set; }
        private double UpperRightX { get; set; }
        private double UpperRightY { get; set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        private double X { get; set; }
        private double Y { get; set; }
        public string Margin { get; protected set; }

        protected IEnumerable<string> Styles { get; set; }
        public string FillColor { get; protected set; }
        public string StrokeColor { get; protected set; }

        private void UpdatePropertyValues() {
            Label = SubGraphBehind.HasAttribute("label")
                ? SubGraphBehind.GetAttribute("label")
                : SubGraphBehind.Id;
            IsCluster = Label.StartsWith("cluster");

            HasBoundingBox = SubGraphBehind.HasAttribute("bb");
            if (HasBoundingBox) {
                UpperRightX = WpfHelper.StringToPixel(
                    SubGraphBehind.GetAttribute("bb").Trim('"').Split(',')[2] + "pt");
                UpperRightY = WpfHelper.StringToPixel(
                    SubGraphBehind.GetAttribute("bb").Trim('"').Split(',')[3] + "pt");
                X = WpfHelper.StringToPixel(
                    SubGraphBehind.GetAttribute("bb").Trim('"').Split(',')[0] + "pt");
                Y = WpfHelper.StringToPixel(
                    SubGraphBehind.GetAttribute("bb").Trim('"').Split(',')[1] + "pt");
                Width = UpperRightX - X;
                Height = UpperRightY - Y;
                Margin = $"{X},{Y},0,0";

                Styles = WpfHelper.ConvertIdToStyles(
                    SubGraphBehind.HasAttribute("style", true)
                        ? SubGraphBehind.GetAttribute("style", true)
                        : null);
                FillColor = GetSubGraphFillColor();
                StrokeColor = GetSubGraphStrokeColor();
            }
        }

        private string GetSubGraphFillColor() {
            var bgColor = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("bgcolor", true)
                    ? SubGraphBehind.GetAttribute("bgcolor", true)
                    : null);
            var color = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("color", true)
                    ? SubGraphBehind.GetAttribute("color", true)
                    : null);
            var fillcolor = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("fillcolor", true)
                    ? SubGraphBehind.GetAttribute("fillcolor", true)
                    : null);
            if (Styles.Contains("filled")) {
                return fillcolor ?? color ?? bgColor;
            }
            return bgColor;
        }

        private string GetSubGraphStrokeColor() {
            var color = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("color", true)
                    ? SubGraphBehind.GetAttribute("color", true)
                    : "black");
            return color;
        }
    }
}