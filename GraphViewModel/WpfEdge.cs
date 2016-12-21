using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfEdge {
        private readonly IEdge _edgeBehind;

        public WpfEdge(IEdge edgeBehind) {
            _edgeBehind = edgeBehind;
        }

        public string Label => _edgeBehind.HasAttribute("label") ? _edgeBehind.GetAttribute("label") : null;
        public string Geometry => PosToGeometry(_edgeBehind.GetAttribute("pos"));

        private string PosToGeometry(string pos) {
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
                var pathFigure = allPoints.Select((v, i) => i>0 ? ((i-1)%3 == 0 ? " C " : " ") + v: "M " + v);
                pathGeometry.Add(string.Concat(pathFigure));
            }

            return string.Join(" ", pathGeometry);
            //var points = pos.TrimStart('e', ',')
            //    .Split(' ', '\r', '\n', '\t')
            //    .Select(p => string.Join(",", p.Split(',').Select(ConvertPoint)))
            //    .ToList();
            //var initialPoint = points[0];
            //points.RemoveAt(0);
            //var geometry = "M " + initialPoint + " " + string.Join("", points.Select(p => " L " + p));
            //return geometry;
        }

        private string ConvertPoint(string p) {
            return float.Parse(p).ToString(CultureInfo.InvariantCulture);
        }
    }
}
