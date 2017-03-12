using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph {
    public class PathGeometryData {
        private IEnumerable<PathFigureData> FigureData { get; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "pos")]
        public PathGeometryData(string pos) {
            if (pos == null) {
                throw new ArgumentNullException(nameof(pos));
            }
            var splines = pos.Replace("\r", "")
                .Replace("\n", "")
                .Replace("\\", "")
                .Replace("\t", " ")
                .Trim('"')
                .Trim(' ')
                .Split(';');
            FigureData = splines.Select(ParsePathFigureData);
        }

        private PathFigureData ParsePathFigureData(string spline) {
            const string num = @"[-]?(?:[\.[0-9]+]|[0-9]+(?:\.[0-9]*)?)(?:e\+[0-9][0-9][0-9])?";
            var point = FormattableString.Invariant($"(?:{num},{num})");
            var end = FormattableString.Invariant($"(e,(?<endPoint>{point}) )?");
            var start = FormattableString.Invariant($"(s,(?<startPoint>{point}) )?");
            var main = FormattableString.Invariant($"(?<mainPoint>{point})");
            var triple = FormattableString.Invariant($"(?<cubicTriple>{point} {point} {point})");
            var splineExp = new Regex(FormattableString.Invariant(
                $"^(?:{start}{end}|{end}{start})?{main}(?: {triple})*$"));

            var match = splineExp.Match(spline);
            if (!match.Success) {
                throw new ArgumentException(FormattableString.Invariant($"Unable to interpret as a spline: {spline}"));
            }

            var startPointStr = match.Groups["startPoint"].Value;
            var mainPointStr = match.Groups["mainPoint"].Value;
            var cubicTriplesStr = match.Groups["cubicTriple"].Captures
                .Cast<object>()
                .Select(o => o.ToString())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
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
            var p1 = WpfHelper.StringToPixel(point.Split(',')[0] + "pt");
            var p2 = WpfHelper.StringToPixel(point.Split(',')[1] + "pt");
            return new Point(p1, p2);
        }

        public string PathGeometry => string.Join(" ", FigureData.Select(d => d.FigureGeometry));
        public string ArrowHeadGeometry => GetArrowHead().ToString();
        public string ArrowTailGeometry => GetArrowTail().ToString();
        public bool HasArrowHead => FigureData.Last().EndPoint != null;
        public bool HasArrowTail => FigureData.First().StartPoint != null;

        private ArrowHead GetArrowHead() {
            var lastPathFigureData = FigureData.Last();
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

        private ArrowHead GetArrowTail() {
            var firstPathFigureData = FigureData.First();
            var cubics = firstPathFigureData.CubicTriples.ToList();
            Point head, source;
            if (firstPathFigureData.StartPoint != null) {
                head = firstPathFigureData.StartPoint;
                source = firstPathFigureData.MainPoint;
            } else if (cubics.Any()) {
                var firstCubic = cubics.First().ToList();
                head = firstPathFigureData.MainPoint;
                source = firstCubic[0];
            } else {
                throw new NotImplementedException();
            }

            return new ArrowHead(head, source);
        }
    }

    internal class Point {
        public Point(double x, double y) {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public override string ToString() {
            return FormattableString.Invariant($"{X},{Y}");
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

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal class ArrowHead {
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

    internal class PathFigureData {
        public Point StartPoint { get; set; }
        public Point MainPoint { get; set; }
        public IEnumerable<IEnumerable<Point>> CubicTriples { get; set; }
        public Point EndPoint { get; set; }
        public string FigureGeometry {
            get {
                var pathFigureGeometry = new List<string>();
                if (StartPoint == null) {
                    pathFigureGeometry.Add(FormattableString.Invariant($"M {MainPoint}"));
                } else {
                    pathFigureGeometry.Add(FormattableString.Invariant($"M {StartPoint}"));
                    pathFigureGeometry.Add(FormattableString.Invariant($"L {MainPoint}"));
                }
                foreach (var cubicTriple in CubicTriples) {
                    var bezierSegment = "C " + string.Join(" ", cubicTriple);
                    pathFigureGeometry.Add(bezierSegment);
                }
                if (EndPoint != null) {
                    pathFigureGeometry.Add(FormattableString.Invariant($"L {EndPoint}"));
                }
                return string.Join(" ", pathFigureGeometry);
            }
        }
    }
}