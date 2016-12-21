using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph.GraphViewModel {
    public static class GraphViewModelHelper {
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

        public static double InchToPixel(double inches, double ppi = 96) {
            return inches*ppi;
        }

        public static double PointToPixel(double points, double ppi = 96) {
            return points*ppi/72;
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
                var allPoints = new List<string>();

                if (!string.IsNullOrEmpty(startPoint)) {
                    allPoints.Add(startPoint);
                }
                allPoints.Add(mainPoint);
                allPoints.AddRange(cubicTriples.SelectMany(v => v.Split(' ')));
                //if (!string.IsNullOrEmpty(endPoint)) {
                //    allPoints.Add(endPoint);
                //}
                var pathFigure = allPoints.Select(ConvertSegmentPointToPixel)
                    .Select((v, i) => i > 0 ? ((i - 1)%3 == 0 ? " C " : " ") + v : "M " + v);
                pathGeometry.Add(string.Concat(pathFigure));
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
