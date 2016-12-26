using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfSubGraph {
        public WpfSubGraph(ISubGraph subGraphBehind) {
            SubGraphBehind = subGraphBehind;
            UpdatePropertyValues();
        }

        private ISubGraph SubGraphBehind { get; }
        public string Label { get; protected set; }
        public bool HasBoundingBox { get; protected set; }
        private double UpperRightX { get; set; }
        private double UpperRightY { get; set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        private double X { get; set; }
        private double Y { get; set; }
        public string Margin { get; protected set; }

        private void UpdatePropertyValues() {
            Label = SubGraphBehind.HasAttribute("label") ? SubGraphBehind.GetAttribute("label") : SubGraphBehind.Id;
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
            }
        }
    }
}