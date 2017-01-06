using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using SharpGraph.BaseViewModel;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class WpfNode : INotifyPropertyChanged {
        public WpfNode(WpfGraph root, INode nodeBehind, bool isSelected = false) {
            Root = root;
            NodeBehind = nodeBehind;
            UpdatePropertyValues();
            _isSelected = isSelected;
        }

        private RelayCommand _toggleNodeSelectionCommand;
        public ICommand ToggleNodeSelectionCommand {
            get {
                return _toggleNodeSelectionCommand ?? (_toggleNodeSelectionCommand = new RelayCommand(
                           param => { IsSelected = !IsSelected; }
                       ));
            }
        }

        protected WpfGraph Root { get; }
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
        public double StrokeThickness { get; protected set; }
        public string FontColor { get; protected set; }
        //public string FontFamily { get; protected set; }
        public double FontSize { get; protected set; }

        //Properties that can be changed
        private bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            set {
                var hasChanged = _isSelected != value;
                _isSelected = value;
                OnPropertyChanged();
                if (hasChanged) {
                    Root.RaiseChanged();
                }
            }
        }

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

            //Dot extends the border in both directions but wpf only inwards
            //so we compensate this by increasing the Width/Height by StrokeThickness.
            StrokeThickness = GetNodeStrokeThickness();

            CenterX = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                  NodeBehind.GetAttribute("pos")).Split(',')[0] + "pt");
            CenterY = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                  NodeBehind.GetAttribute("pos")).Split(',')[1] + "pt");
            Width = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                NodeBehind.GetAttribute("width", true)) + "in")
                    + StrokeThickness;
            Height = WpfHelper.StringToPixel(WpfHelper.ConvertIdToText(
                                                 NodeBehind.GetAttribute("height", true)) + "in")
                     + StrokeThickness;
            X = CenterX - Width/2;
            Y = CenterY - Height/2;
            Margin = $"{X},{Y},0,0";

            Styles = WpfHelper.ConvertIdToStyles(
                NodeBehind.HasAttribute("style", true)
                    ? NodeBehind.GetAttribute("style", true)
                    : null);
            FillColor = GetNodeFillColor();
            StrokeColor = GetNodeStrokeColor();
            //FontFamily = GetFontFamily();
            FontColor = GetFontColor();
            FontSize = GetFontSize();
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
                return fillcolor ?? color ?? "Transparent";
            }
            return "Transparent";
        }

        private string GetNodeStrokeColor() {
            var color = WpfHelper.ConvertIdToText(
                NodeBehind.HasAttribute("color", true)
                    ? NodeBehind.GetAttribute("color", true)
                    : null);
            return color ?? "black";
        }

        private double GetNodeStrokeThickness() {
            var thicknessStr = WpfHelper.ConvertIdToText(
                NodeBehind.HasAttribute("penwidth", true)
                    ? NodeBehind.GetAttribute("penwidth", true) + "pt"
                    : "1");
            return WpfHelper.StringToPixel(thicknessStr);
        }

        //private string GetFontFamily() {
        //    var fontname = WpfHelper.ConvertIdToText(
        //        NodeBehind.HasAttribute("fontname", true)
        //            ? NodeBehind.GetAttribute("fontname", true)
        //            : "Times-Roman");
        //    return null;
        //}

        private string GetFontColor() {
            return WpfHelper.ConvertIdToText(
                NodeBehind.HasAttribute("fontcolor", true)
                    ? NodeBehind.GetAttribute("fontcolor", true)
                    : "black");
        }

        private double GetFontSize() {
            var sizeStr = WpfHelper.ConvertIdToText(
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