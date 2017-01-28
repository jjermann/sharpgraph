using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class WpfNode : INotifyPropertyChanged {
        public WpfNode(WpfGraph root, INode nodeBehind, bool isSelected = false) {
            Root = root;
            NodeBehind = nodeBehind;
            UpdatePropertyValues();
            m_isSelected = isSelected;
        }

        private RelayCommand m_toggleNodeSelectionCommand;
        public ICommand ToggleNodeSelectionCommand {
            get {
                return m_toggleNodeSelectionCommand ?? (m_toggleNodeSelectionCommand = new RelayCommand(
                           param => { IsSelected = !IsSelected; }
                       ));
            }
        }

        protected WpfGraph Root { get; }
        protected INode NodeBehind { get; }
        public string Id { get; protected set; }
        public string Label { get; protected set; }

        public ShapeData NodeShapeData { get; protected set; }

        private double CenterX { get; set; }
        private double CenterY { get; set; }
        private double X { get; set; }
        private double Y { get; set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public string Margin { get; protected set; }

        private ICollection<string> Styles { get; set; }
        public string FillColor { get; protected set; }
        public string StrokeColor { get; protected set; }
        public double StrokeThickness { get; protected set; }
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<double> StrokeDashList { get; private set; }
        public string FontColor { get; protected set; }
        //public string FontFamily { get; protected set; }
        public double FontSize { get; protected set; }

        //Properties that can be changed
        private bool m_isSelected;
        public bool IsSelected {
            get { return m_isSelected; }
            set {
                var hasChanged = m_isSelected != value;
                m_isSelected = value;
                OnPropertyChanged();
                if (hasChanged) {
                    Root.RaiseChanged();
                }
            }
        }

        private void UpdatePropertyValues() {
            Id = NodeBehind.Id;
            Label = WpfHelper.IdToText(
                NodeBehind.HasAttribute("label")
                    ? NodeBehind.GetAttribute("label")
                    : NodeBehind.Id);

            Styles = WpfHelper.IdToStyles(
                NodeBehind.HasAttribute("style", true)
                    ? NodeBehind.GetAttribute("style", true)
                    : null);

            NodeShapeData = WpfHelper.ConvertToShapeData(
                NodeBehind.HasAttribute("shape", true)
                    ? NodeBehind.GetAttribute("shape", true)
                    : "ellipse",
                NodeBehind.HasAttribute("sides", true)
                    ? NodeBehind.GetAttribute("sides", true)
                    : "4",
                Styles.Contains("diagonals"));

            //Dot extends the border in both directions but wpf only inwards
            //so we compensate this by increasing the Width/Height by StrokeThickness.
            StrokeThickness = GetNodeStrokeThickness();

            CenterX = WpfHelper.StringToPixel(WpfHelper.IdToText(
                                                  NodeBehind.GetAttribute("pos")).Split(',')[0] + "pt");
            CenterY = WpfHelper.StringToPixel(WpfHelper.IdToText(
                                                  NodeBehind.GetAttribute("pos")).Split(',')[1] + "pt");
            Width = WpfHelper.StringToPixel(WpfHelper.IdToText(
                                                NodeBehind.GetAttribute("width", true)) + "in")
                    + StrokeThickness;
            Height = WpfHelper.StringToPixel(WpfHelper.IdToText(
                                                 NodeBehind.GetAttribute("height", true)) + "in")
                     + StrokeThickness;
            X = CenterX - Width/2;
            Y = CenterY - Height/2;
            Margin = FormattableString.Invariant($"{X},{Y},0,0");

            FillColor = GetNodeFillColor();
            StrokeColor = GetNodeStrokeColor();
            StrokeDashList = WpfHelper.AbsoluteStrokeDash(Styles, StrokeThickness);
            //FontFamily = GetFontFamily();
            FontColor = GetFontColor();
            FontSize = GetFontSize();
        }

        private string GetNodeFillColor() {
            if (Styles.Contains("filled")) {
                var color = WpfHelper.IdToText(
                    NodeBehind.HasAttribute("color", true)
                        ? NodeBehind.GetAttribute("color", true)
                        : null);
                var fillcolor = WpfHelper.IdToText(
                    NodeBehind.HasAttribute("fillcolor", true)
                        ? NodeBehind.GetAttribute("fillcolor", true)
                        : null);
                return fillcolor ?? color ?? "lightgray";
            }
            return "Transparent";
        }

        private string GetNodeStrokeColor() {
            var color = WpfHelper.IdToText(
                NodeBehind.HasAttribute("color", true)
                    ? NodeBehind.GetAttribute("color", true)
                    : null);
            return color ?? "black";
        }

        private double GetNodeStrokeThickness() {
            var thicknessStr = WpfHelper.IdToText(
                NodeBehind.HasAttribute("penwidth", true)
                    ? NodeBehind.GetAttribute("penwidth", true) + "pt"
                    : "1");
            return WpfHelper.StringToPixel(thicknessStr);
        }

        //private string GetFontFamily() {
        //    var fontname = WpfHelper.IdToText(
        //        NodeBehind.HasAttribute("fontname", true)
        //            ? NodeBehind.GetAttribute("fontname", true)
        //            : "Times-Roman");
        //    return null;
        //}

        private string GetFontColor() {
            return WpfHelper.IdToText(
                NodeBehind.HasAttribute("fontcolor", true)
                    ? NodeBehind.GetAttribute("fontcolor", true)
                    : "black");
        }

        private double GetFontSize() {
            var sizeStr = WpfHelper.IdToText(
                NodeBehind.HasAttribute("fontsize", true)
                    ? NodeBehind.GetAttribute("fontsize", true)
                    : null);
            if (!string.IsNullOrEmpty(sizeStr)) {
                return double.Parse(sizeStr, CultureInfo.InvariantCulture);
            }
            return 14.0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}