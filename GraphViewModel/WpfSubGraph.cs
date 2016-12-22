using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfSubGraph {
        private readonly ISubGraph _subGraphBehind;

        public WpfSubGraph(ISubGraph subGraphBehind) {
            _subGraphBehind = subGraphBehind;
        }

        public virtual string Label => _subGraphBehind.HasAttribute("label") ? _subGraphBehind.GetAttribute("label") : _subGraphBehind.Id;
        public bool HasBoundingBox => _subGraphBehind.HasAttribute("bb");
        public double X => GraphViewModelHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[0] + "pt");
        public double Y => GraphViewModelHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[1] + "pt");
        private double UpperRightX => GraphViewModelHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[2] + "pt");
        private double UpperRightY => GraphViewModelHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[3] + "pt");
        public double Width => UpperRightX - X;
        public double Height => UpperRightY - Y;
    }
}
