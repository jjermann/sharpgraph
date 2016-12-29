using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph.GraphViewModel {
    public static class WpfHelper {
        private const double DeviceIndependentPpi = 96.0;

        public static double StringToPixel(string input) {
            var inches = Regex.Match(input, "^(?<num>.*)in$").Groups["num"].Value;
            if (!string.IsNullOrEmpty(inches)) {
                return InchToPixel(double.Parse(inches));
            }
            var points = Regex.Match(input, "^(?<num>.*)pt$").Groups["num"].Value;
            if (!string.IsNullOrEmpty(points)) {
                return PtToPixel(double.Parse(points));
            }
            return double.Parse(input);
        }

        public static double InchToPixel(double inches) {
            return inches*DeviceIndependentPpi;
        }

        public static double PtToPixel(double points) {
            return points*DeviceIndependentPpi/72;
        }

        public static string PosToGeometry(string pos) {
            var pathGeometryData = ParsePathGeometryData(pos);
            return pathGeometryData.PathGeometry;
        }

        public static string PosToArrowHeadGeometry(string pos) {
            return PosToArrowHead(pos).ToString();
        }

        private static ArrowHead PosToArrowHead(string pos) {
            var lastPathFigureData = ParsePathGeometryData(pos).FigureData.Last();
            var cubics = lastPathFigureData.CubicTriples.ToList();
            Point head, source;
            if (cubics.Any()) {
                var lastCubic = cubics.Last().ToList();
                if (lastPathFigureData.EndPoint != null) {
                    head = lastPathFigureData.EndPoint;
                    source = lastCubic[2];
                } else {
                    head = lastCubic[2];
                    source = lastCubic[0];
                }
            } else {
                if (lastPathFigureData.EndPoint != null) {
                    head = lastPathFigureData.EndPoint;
                    source = lastPathFigureData.MainPoint;
                } else {
                    if (lastPathFigureData.StartPoint == null) {
                        throw new NotImplementedException();
                    }
                    head = lastPathFigureData.MainPoint;
                    source = lastPathFigureData.StartPoint;
                }
            }

            return new ArrowHead(head, source);
        }

        private static PathGeometryData ParsePathGeometryData(string pos) {
            pos = pos.Replace("\r", "")
                .Replace("\n", "")
                .Replace("\\", "")
                .Replace("\t", " ")
                .Trim('"')
                .Trim(' ');
            var splines = pos.Split(';');
            return new PathGeometryData(splines.Select(ParsePathFigureData));
        }

        private static PathFigureData ParsePathFigureData(string spline) {
            const string num = @"[-]?([\.[0-9]+]|[0-9]+(\.[0-9]*)?)";
            var pointStr = $"({num},{num})";
            var splineExp = new Regex(
                $"^"
                + $"(e,(?<endPoint>{pointStr}) )?"
                + $"(s,(?<startPoint>{pointStr}) )?"
                + $"(?<mainPoint>{pointStr})"
                + $"( (?<cubicTriple>{pointStr} {pointStr} {pointStr}))*"
                + "$");

            var match = splineExp.Match(spline);
            if (!match.Success) {
                throw new ArgumentException($"Unable to interpret as a spline: {spline}");
            }

            var startPointStr = match.Groups["startPoint"].Value;
            var mainPointStr = match.Groups["mainPoint"].Value;
            var cubicTriplesStr = match.Groups["cubicTriple"].Captures
                .OfType<Group>()
                .Select(g => g.Value).ToList();
            var endPointStr = match.Groups["endPoint"].Value;

            var startPoint = string.IsNullOrEmpty(startPointStr) ? null : ConvertStringToPoint(startPointStr);
            var mainPoint = string.IsNullOrEmpty(mainPointStr) ? null : ConvertStringToPoint(mainPointStr);
            var cubicTriples = cubicTriplesStr.Select(t => t.Split(' ').Select(ConvertStringToPoint));
            var endPoint = string.IsNullOrEmpty(endPointStr) ? null : ConvertStringToPoint(endPointStr);

            return new PathFigureData {
                StartPoint = startPoint,
                MainPoint = mainPoint,
                CubicTriples = cubicTriples,
                EndPoint = endPoint
            };
        }

        private static Point ConvertStringToPoint(string point) {
            var p1 = StringToPixel(point.Split(',')[0] + "pt");
            var p2 = StringToPixel(point.Split(',')[1] + "pt");
            return new Point(p1, p2);
        }

        public static string ConvertIdToText(string id) {
            return id?.Trim('"')
                .Replace(@"\n", "\n")
                .Replace(@"\t", "\t")
                .Replace(@"\r", "\r");
        }

        public static string ConvertIdToShape(string id) {
            var shape = ConvertIdToText(id).ToLower();
            switch (shape) {
                case "box":
                case "rect":
                case "rectangle":
                case "square":
                    return "Rectangle";
                default:
                    return "Ellipse";
            }
        }

        public static IEnumerable<string> ConvertIdToStyles(string id) {
            if (id == null) {
                return new List<string>();
            }
            return ConvertIdToText(id).ToLower()
                .Split(';')
                .Select(s => s.Trim());

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

        public class Point {
            public Point(double x, double y) {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }

            public override string ToString() {
                return $"{X},{Y}";
            }

            public Point Rotate(double angle, Point center = null) {
                if (center == null) {
                    center = new Point(0, 0);
                }
                var x = (X - center.X)*Math.Cos(angle) - (Y - center.Y)*Math.Sin(angle) + center.X;
                var y = (X - center.X)*Math.Sin(angle) + (Y - center.Y)*Math.Cos(angle) + center.Y;
                return new Point(x, y);
            }

            public Point Translate(Point shift) {
                var x = X + shift.X;
                var y = Y + shift.Y;
                return new Point(x, y);
            }
        }

        public class ArrowHead {
            private const double BaseWidth = 8.0;
            private const double Length = BaseWidth*2;

            public ArrowHead(Point head, Point source) {
                Head = head;
                Source = source;
                Angle = Math.Atan2(Head.Y - Source.Y, Head.X - Source.X);
                LeftBase = new Point(-Length, BaseWidth/2.0).Rotate(Angle).Translate(Head);
                RightBase = new Point(-Length, -BaseWidth/2.0).Rotate(Angle).Translate(Head);
            }

            public Point Head { get; }
            public Point Source { get; }
            public double Angle { get; }
            public Point LeftBase { get; }
            public Point RightBase { get; }

            public override string ToString() {
                return "M " + Head + " L " + LeftBase + " L " + RightBase + " Z";
            }
        }

        public class PathGeometryData {
            public PathGeometryData(IEnumerable<PathFigureData> figureData) {
                FigureData = figureData;
            }

            public IEnumerable<PathFigureData> FigureData { get; }

            public string PathGeometry {
                get { return string.Join(" ", FigureData.Select(d => d.FigureGeometry)); }
            }
        }

        public class PathFigureData {
            public Point StartPoint { get; set; }
            public Point MainPoint { get; set; }
            public IEnumerable<IEnumerable<Point>> CubicTriples { get; set; }
            public Point EndPoint { get; set; }
            public string FigureGeometry {
                get {
                    var pathFigureGeometry = new List<string>();
                    if (StartPoint == null) {
                        pathFigureGeometry.Add($"M {MainPoint}");
                    } else {
                        pathFigureGeometry.Add($"M {StartPoint}");
                        pathFigureGeometry.Add($"L {MainPoint}");
                    }
                    foreach (var cubicTriple in CubicTriples) {
                        var bezierSegment = "C " + string.Join(" ", cubicTriple);
                        pathFigureGeometry.Add(bezierSegment);
                    }
                    if (EndPoint != null) {
                        pathFigureGeometry.Add($"L {EndPoint}");
                    }
                    return string.Join(" ", pathFigureGeometry);
                }
            }
        }
    }
}