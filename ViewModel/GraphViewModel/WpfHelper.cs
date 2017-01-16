using System.Collections.Generic;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
             "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pt")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
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

        public static string IdToShape(string id) {
            var shape = IdToText(id).ToLowerInvariant();
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

        public static IEnumerable<string> IdToStyles(string id) {
            if (id == null) {
                return new List<string>();
            }
            return IdToText(id).ToLowerInvariant()
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
    }
}