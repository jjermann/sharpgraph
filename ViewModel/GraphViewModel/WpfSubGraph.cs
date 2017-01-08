using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WpfSubGraph {
        public WpfSubGraph(ISubGraph subGraphBehind) {
            SubGraphBehind = subGraphBehind;
            UpdatePropertyValues();
        }

        private ISubGraph SubGraphBehind { get; }
        public string Id { get; protected set; }
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
        public string LabelMargin { get; protected set; }

        protected IEnumerable<string> Styles { get; set; }
        public string FillColor { get; protected set; }
        public string StrokeColor { get; protected set; }
        public double StrokeThickness { get; protected set; }
        public string FontColor { get; protected set; }
        //public string FontFamily { get; protected set; }
        public double FontSize { get; protected set; }

        private void UpdatePropertyValues() {
            Id = SubGraphBehind.Id;
            IsCluster = Id.StartsWith("cluster", StringComparison.OrdinalIgnoreCase);
            Label = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("label", true)
                    ? SubGraphBehind.GetAttribute("label", true)
                    : null);

            HasBoundingBox = SubGraphBehind.HasAttribute("bb");
            if (HasBoundingBox) {
                StrokeThickness = GetSubGraphStrokeThickness();

                UpperRightX = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                          SubGraphBehind.GetAttribute("bb")).Split(',')[2] + "pt") +
                              StrokeThickness/2.0;
                UpperRightY = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                          SubGraphBehind.GetAttribute("bb")).Split(',')[3] + "pt") +
                              StrokeThickness/2.0;
                X = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                SubGraphBehind.GetAttribute("bb")).Split(',')[0] + "pt") -
                    StrokeThickness/2.0;
                Y = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                SubGraphBehind.GetAttribute("bb")).Split(',')[1] + "pt") -
                    StrokeThickness/2.0;
                Width = UpperRightX - X;
                Height = UpperRightY - Y;
                Margin = FormattableString.Invariant($"{X},{Y},0,0");

                Styles = WpfHelper.ConvertIdToStyles(
                    SubGraphBehind.HasAttribute("style", true)
                        ? SubGraphBehind.GetAttribute("style", true)
                        : null);
                FillColor = GetSubGraphFillColor();
                StrokeColor = GetSubGraphStrokeColor();
                //FontFamily = GetFontFamily();
                FontColor = GetFontColor();
                FontSize = GetFontSize();

                if (Label != null) {
                    var labelPos = WpfHelper.ConvertIdToText(SubGraphBehind.GetAttribute("lp"))
                        .Split(',')
                        .Select(p => p + "pt")
                        .Select(WpfHelper.StringToPixel)
                        .ToList();
                    LabelMargin = FormattableString.Invariant($"{labelPos[0]},{labelPos[1]},0,0");
                }
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
                return fillcolor ?? color ?? bgColor ?? "Transparent";
            }
            return bgColor ?? "Transparent";
        }

        private string GetSubGraphStrokeColor() {
            var pencolor = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("pencolor", true)
                    ? SubGraphBehind.GetAttribute("pencolor", true)
                    : null);
            var color = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("color", true)
                    ? SubGraphBehind.GetAttribute("color", true)
                    : null);
            return pencolor ?? color ?? "black";
        }

        private double GetSubGraphStrokeThickness() {
            var thicknessStr = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("penwidth", true)
                    ? SubGraphBehind.GetAttribute("penwidth", true) + "pt"
                    : "1");
            return WpfHelper.StringToPixel(thicknessStr);
        }

        //private string GetFontFamily() {
        //    var fontname = WpfHelper.ConvertIdToText(
        //        SubGraphBehind.HasAttribute("fontname", true)
        //            ? SubGraphBehind.GetAttribute("fontname", true)
        //            : "Times-Roman");
        //    return null;
        //}

        private string GetFontColor() {
            return WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("fontcolor", true)
                    ? SubGraphBehind.GetAttribute("fontcolor", true)
                    : "black");
        }

        private double GetFontSize() {
            var sizeStr = WpfHelper.ConvertIdToText(
                SubGraphBehind.HasAttribute("fontsize", true)
                    ? SubGraphBehind.GetAttribute("fontsize", true)
                    : null);
            if (!string.IsNullOrEmpty(sizeStr)) {
                return double.Parse(sizeStr, CultureInfo.InvariantCulture);
            }
            return 14.0;
        }
    }
}