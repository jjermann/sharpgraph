using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph.GraphViewModel {
    public static class GraphViewModelHelper {
        private const double DeviceIndependentPpi = 96.0;
        public static double StringToPixel(string input) {
            var inches = Regex.Match(input, "^(?<num>.*)in$").Groups["num"].Value;
            if (!string.IsNullOrEmpty(inches)) {
                return InchToPixel(double.Parse(inches));
            }
            var points = Regex.Match(input, "^(?<num>.*)pt$").Groups["num"].Value;
            if (!string.IsNullOrEmpty(points)) {
                return PointToPixel(double.Parse(points));
            }
            return double.Parse(input);
        }

        public static double InchToPixel(double inches) {
            return inches* DeviceIndependentPpi;
        }

        public static double PointToPixel(double points) {
            return points* DeviceIndependentPpi / 72;
        }

        public static string PosToGeometry(string pos) {
            const string num = @"[-]?([\.[0-9]+]|[0-9]+(\.[0-9]*)?)";
            var point = $"({num},{num})";
            var splineExp = new Regex(
                $"^"
                + $"(e,(?<endPoint>{point}) )?"
                + $"(s,(?<startPoint>{point}) )?"
                + $"(?<mainPoint>{point})"
                + $"( (?<cubicTriple>{point} {point} {point}))*"
                + "$");

            pos = pos.Replace("\r", "")
                .Replace("\n", "")
                .Replace("\\", "")
                .Replace("\t", " ")
                .Trim('"')
                .Trim(' ');
            var splines = pos.Split(';');
            var pathGeometry = new List<string>();
            foreach (var spline in splines) {
                var match = splineExp.Match(spline);
                if (!match.Success) {
                    throw new ArgumentException($"Unable to interpret as a spline: {spline}");
                }
                var startPoint = match.Groups["startPoint"].Value;
                var mainPoint = match.Groups["mainPoint"].Value;
                var cubicTriples = match.Groups["cubicTriple"].Captures
                    .OfType<Group>()
                    .Select(g => g.Value).ToList();

                var endPoint = match.Groups["endPoint"].Value;

                var pathFigureGeometry = new List<string>();
                if (string.IsNullOrEmpty(startPoint)) {
                    pathFigureGeometry.Add($"M {ConvertSegmentPointToPixel(mainPoint)}");
                } else {
                    pathFigureGeometry.Add($"M {ConvertSegmentPointToPixel(startPoint)}");
                    pathFigureGeometry.Add($"L {ConvertSegmentPointToPixel(mainPoint)}");
                }
                foreach (var cubicTriple in cubicTriples) {
                    var convertedPoints = cubicTriple.Split(' ').Select(ConvertSegmentPointToPixel);
                    var bezierSegment = "C " + string.Join(" ", convertedPoints);
                    pathFigureGeometry.Add(bezierSegment);
                }
                if (!string.IsNullOrEmpty(endPoint)) {
                    pathFigureGeometry.Add($"L {ConvertSegmentPointToPixel(endPoint)}");
                }
                pathGeometry.Add(string.Join(" ", pathFigureGeometry));
            }

            return string.Join(" ", pathGeometry);
        }

        private static string ConvertSegmentPointToPixel(string point) {
            var p1 = StringToPixel(point.Split(',')[0] + "pt");
            var p2 = StringToPixel(point.Split(',')[1] + "pt");
            return $"{p1},{p2}";
        }
    }
}
