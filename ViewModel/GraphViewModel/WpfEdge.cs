using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class WpfEdge {
        public WpfEdge(IEdge edgeBehind) {
            EdgeBehind = edgeBehind;
            UpdatePropertyValues();
        }

        protected IEdge EdgeBehind { get; }
        public string Label { get; protected set; }
        public string LabelMargin { get; protected set; }
        public string Geometry { get; protected set; }
        public bool HasArrowHead { get; protected set; }
        public bool HasArrowTail { get; protected set; }
        public string ArrowHeadGeometry { get; protected set; }
        public string ArrowTailGeometry { get; protected set; }
        public string StrokeColor { get; protected set; }
        public double StrokeThickness { get; protected set; }
        public string FontColor { get; protected set; }
        //public string FontFamily { get; protected set; }
        public double FontSize { get; protected set; }

        private void UpdatePropertyValues() {
            Label = WpfHelper.IdToText(
                EdgeBehind.HasAttribute("label")
                    ? EdgeBehind.GetAttribute("label")
                    : null);
            if (Label != null) {
                var labelPos = WpfHelper.IdToText(EdgeBehind.GetAttribute("lp"))
                    .Split(',')
                    .Select(p => p + "pt")
                    .Select(WpfHelper.StringToPixel)
                    .ToList();
                LabelMargin = FormattableString.Invariant($"{labelPos[0]},{labelPos[1]},0,0");
            }
            var pos = WpfHelper.IdToText(EdgeBehind.GetAttribute("pos"));
            var pathGeometryData = new PathGeometryData(pos);
            Geometry = pathGeometryData.PathGeometry;
            HasArrowHead = pathGeometryData.HasArrowHead;
            HasArrowTail = pathGeometryData.HasArrowTail;
            ArrowHeadGeometry = pathGeometryData.ArrowHeadGeometry;
            ArrowTailGeometry = pathGeometryData.ArrowTailGeometry;

            StrokeColor = GetEdgeStrokeColor();
            StrokeThickness = GetEdgeStrokeThickness();
            //FontFamily = GetFontFamily();
            FontColor = GetFontColor();
            FontSize = GetFontSize();
        }

        private string GetEdgeStrokeColor() {
            var color = WpfHelper.IdToText(
                EdgeBehind.HasAttribute("color", true)
                    ? EdgeBehind.GetAttribute("color", true)
                    : null);
            return color ?? "black";
        }

        private double GetEdgeStrokeThickness() {
            var thicknessStr = WpfHelper.IdToText(
                EdgeBehind.HasAttribute("penwidth", true)
                    ? EdgeBehind.GetAttribute("penwidth", true) + "pt"
                    : "1");
            return WpfHelper.StringToPixel(thicknessStr);
        }

        //private string GetFontFamily() {
        //    var fontname = WpfHelper.IdToText(
        //        EdgeBehind.HasAttribute("fontname", true)
        //            ? EdgeBehind.GetAttribute("fontname", true)
        //            : "Times-Roman");
        //    return null;
        //}

        private string GetFontColor() {
            return WpfHelper.IdToText(
                EdgeBehind.HasAttribute("fontcolor", true)
                    ? EdgeBehind.GetAttribute("fontcolor", true)
                    : "black");
        }

        private double GetFontSize() {
            var sizeStr = WpfHelper.IdToText(
                EdgeBehind.HasAttribute("fontsize", true)
                    ? EdgeBehind.GetAttribute("fontsize", true)
                    : null);
            if (!string.IsNullOrEmpty(sizeStr)) {
                return double.Parse(sizeStr, CultureInfo.InvariantCulture);
            }
            return 14.0;
        }
    }
}