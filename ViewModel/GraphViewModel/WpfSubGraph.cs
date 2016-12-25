﻿using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfSubGraph {
        private readonly ISubGraph _subGraphBehind;

        public WpfSubGraph(ISubGraph subGraphBehind) {
            _subGraphBehind = subGraphBehind;
        }

        public virtual string Label
            => _subGraphBehind.HasAttribute("label") ? _subGraphBehind.GetAttribute("label") : _subGraphBehind.Id;

        public bool HasBoundingBox => _subGraphBehind.HasAttribute("bb");

        private double UpperRightX
            => WpfHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[2] + "pt");

        private double UpperRightY
            => WpfHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[3] + "pt");

        public double Width => UpperRightX - X;
        public double Height => UpperRightY - Y;
        private double X => WpfHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[0] + "pt");
        private double Y => WpfHelper.StringToPixel(_subGraphBehind.GetAttribute("bb").Trim('"').Split(',')[1] + "pt");
        public string Margin => $"{X},{Y},0,0";
    }
}