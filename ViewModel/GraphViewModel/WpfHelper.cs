using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph {
    public static class WpfHelper {
        private const double DeviceIndependentPpi = 96.0;

        public static double StringToPixel(string input) {
            var inches = Regex.Match(input, "^(?<num>.*)in$").Groups["num"].Value;
            if (!string.IsNullOrEmpty(inches)) {
                return InchToPixel(double.Parse(inches, CultureInfo.InvariantCulture));
            }
            var points = Regex.Match(input, "^(?<num>.*)pt$").Groups["num"].Value;
            if (!string.IsNullOrEmpty(points)) {
                return PtToPixel(double.Parse(points, CultureInfo.InvariantCulture));
            }
            return double.Parse(input, CultureInfo.InvariantCulture);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static double InchToPixel(double inches) {
            return inches*DeviceIndependentPpi;
        }

        [SuppressMessage("Microsoft.Naming",
             "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pt")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
             MessageId = "Pt")]
        // ReSharper disable once MemberCanBePrivate.Global
        public static double PtToPixel(double points) {
            return points*DeviceIndependentPpi/72;
        }

        public static string IdToText(string id) {
            return id?.Trim('"')
                .Replace(@"\n", "\n")
                .Replace(@"\t", "\t")
                .Replace(@"\r", "\r");
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static ShapeData ConvertToShapeData(string shapeId, string sideId, bool hasDiagonals = false) {
            var shape = IdToText(shapeId).ToLowerInvariant();
            int sides;
            if (!int.TryParse(IdToText(sideId), out sides)) {
                sides = 4;
            }
            if (sides < 3) {
                shape = "ellipse";
            }
            var shapeData = new ShapeData {
                Sides = sides,
                HasDiagonals = hasDiagonals
            };
            switch (shape) {
                case "plaintext":
                case "plain":
                case "none":
                    shapeData.Name = "None";
                    break;
                case "box":
                case "rect":
                case "rectangle":
                case "square":
                    shapeData.Name = "Rectangle";
                    break;
                case "triangle":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 3;
                    break;
                case "pentagon":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 5;
                    break;
                case "hexagon":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 6;
                    break;
                case "septagon":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 7;
                    break;
                case "octagon":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 8;
                    break;
                case "diamond":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 4;
                    shapeData.Angle = Math.PI/4.0;
                    break;
                case "invtriangle":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 3;
                    shapeData.Angle = Math.PI;
                    break;
                case "polygon":
                    shapeData.Name = "Polygon";
                    break;
                case "doublecircle":
                    shapeData.Name = "Ellipse";
                    shapeData.Layers = 2;
                    break;
                case "doubleoctagon":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 8;
                    shapeData.Layers = 2;
                    break;
                case "triplecircle":
                    shapeData.Name = "Ellipse";
                    shapeData.Layers = 3;
                    break;
                case "mdiamond":
                    shapeData.Name = "Polygon";
                    shapeData.Sides = 4;
                    shapeData.Angle = Math.PI/4.0;
                    shapeData.HasDiagonals = true;
                    break;
                case "msquare":
                    shapeData.Name = "Rectangle";
                    shapeData.HasDiagonals = true;
                    break;
                case "mcircle":
                    shapeData.Name = "Ellipse";
                    shapeData.HasDiagonals = true;
                    break;
                case "circle":
                case "ellipse":
                    shapeData.Name = "Ellipse";
                    break;
                //TODO
                //case "parallelogram":
                //case "trapezium":
                //case "invtrapezium":
                //case "house":
                //case "invhouse":
                //case "star":
                //case "oval":
                //case "egg":
                //...
                default:
                    shapeData.Name = "Ellipse";
                    break;
            }

            return shapeData;
        }

        public static ICollection<string> IdToStyles(string id) {
            if (id == null) {
                return new List<string>();
            }
            return IdToText(id).ToLowerInvariant()
                .Split(',')
                .Select(s => s.Trim()).ToList();

            //switch (style) {
            //    //For Nodes and Edges
            //    case "dashed":
            //    case "dotted":
            //    case "solid":
            //    case "invis":
            //    case "bold":

            //    //For Edges
            //    case "tapered":

            //    //For Nodes
            //    case "wedged":
            //    case "diagonals":

            //    //For Nodes and Clusters
            //    case "filled":
            //    case "striped":
            //    case "rounded":

            //    //For Nodes, Clusters and Graphs
            //    case "radial":
            //        return style;
            //    default:
            //        return null;
            //}
        }

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static List<double> AbsoluteStrokeDash(ICollection<string> styles, double strokeThickness) {
            if (styles == null) throw new ArgumentNullException(nameof(styles));
            if (styles.Contains("dashed")) {
                var col = new List<double>();
                var absoluteSize = 10.0/strokeThickness;
                col.Add(absoluteSize);
                col.Add(absoluteSize);
                return col;
            }
            if (styles.Contains("dotted")) {
                var col = new List<double>();
                var absoluteSize1 = 2.0/strokeThickness;
                col.Add(absoluteSize1);
                var absoluteSize2 = 10.0/strokeThickness;
                col.Add(absoluteSize2);
                return col;
            }
            return null;
        }
    }
}