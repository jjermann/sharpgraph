using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
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
        public string ArrowHeadGeometry { get; protected set; }
        public string StrokeColor { get; protected set; }
        public double StrokeThickness { get; protected set; }
        public string FontColor { get; protected set; }
        public string FontFamily { get; protected set; }
        public double FontSize { get; protected set; }

        private void UpdatePropertyValues() {
            Label = WpfHelper.ConvertIdToText(
                EdgeBehind.HasAttribute("label")
                    ? EdgeBehind.GetAttribute("label")
                    : null);
            if (Label != null) {
                var labelPos = WpfHelper.ConvertIdToText(EdgeBehind.GetAttribute("lp"))
                    .Split(',')
                    .Select(p => p + "pt")
                    .Select(WpfHelper.StringToPixel)
                    .ToList();
                LabelMargin = $"{labelPos[0]},{labelPos[1]},0,0";
            }
            Geometry = WpfHelper.PosToGeometry(WpfHelper.ConvertIdToText(
                EdgeBehind.GetAttribute("pos")));
            HasArrowHead = EdgeBehind.Root.IsDirected;
            ArrowHeadGeometry = WpfHelper.PosToArrowHeadGeometry(WpfHelper.ConvertIdToText(
                EdgeBehind.GetAttribute("pos")));

            StrokeColor = GetEdgeStrokeColor();
            StrokeThickness = GetEdgeStrokeThickness();
            FontFamily = GetFontFamily();
            FontColor = GetFontColor();
            FontSize = GetFontSize();
        }

        private string GetEdgeStrokeColor() {
            var color = WpfHelper.ConvertIdToText(
                EdgeBehind.HasAttribute("color", true)
                    ? EdgeBehind.GetAttribute("color", true)
                    : null);
            return color ?? "black";
        }

        private double GetEdgeStrokeThickness() {
            var thicknessStr = WpfHelper.ConvertIdToText(
                EdgeBehind.HasAttribute("penwidth", true)
                    ? EdgeBehind.GetAttribute("penwidth", true) + "pt"
                    : "1");
            return WpfHelper.StringToPixel(thicknessStr);
        }

        private string GetFontFamily() {
            //TODO
            //var fontname = WpfHelper.ConvertIdToText(
            //    EdgeBehind.HasAttribute("fontname", true)
            //        ? EdgeBehind.GetAttribute("fontname", true)
            //        : "Times-Roman");
            return null;
        }

        private string GetFontColor() {
            return WpfHelper.ConvertIdToText(
                EdgeBehind.HasAttribute("fontcolor", true)
                    ? EdgeBehind.GetAttribute("fontcolor", true)
                    : "black");
        }

        private double GetFontSize() {
            var sizeStr = WpfHelper.ConvertIdToText(
                EdgeBehind.HasAttribute("fontsize", true)
                    ? EdgeBehind.GetAttribute("fontsize", true)
                    : null);
            if (!string.IsNullOrEmpty(sizeStr)) {
                return double.Parse(sizeStr);
            }
            return 14.0;
        }
    }
}