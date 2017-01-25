using System;
using System.Text.RegularExpressions;

namespace SharpGraph {
    public static class ModelHelper {
        public const string DefaultGraphId = "GraphId";
        public const string DirectedEdgeOpName = "->";
        public const string UndirectedEdgeOpName = "--";
        public const string DirectedGraphName = "digraph";
        public const string UndirectedGraphName = "graph";
        public const string StrictGraphName = "strict";
        public const string NewSubGraphFormat = "subGraph{0:00000}";

        private static readonly Regex RegexForId = new Regex(@"^[a-zA-Z\u0080-\u00FF]([a-zA-Z\u0080-\u00FF]|[0-9])*$");
        private static readonly Regex RegexForString = new Regex(@"^""(\""|.)*?""$", RegexOptions.Singleline);
        private static readonly Regex RegexForHtml = new Regex(@"^<(<.*?>|[^<>])*>$", RegexOptions.Singleline);
        private static readonly Regex RegexForNumber = new Regex(@"^[-]?([\.[0-9]+]|[0-9]+(\.[0-9]*)?)$");

        public static string ReduceId(string id) {
            if (id == null) {
                return null;
            }
            var reduced = id;
            string last;
            var idType = GetIdType(reduced);
            if (idType == IdType.Invalid) {
                throw new ArgumentException(FormattableString.Invariant($"Id {id} was invalid!"));
            }
            do {
                last = reduced;
                reduced = reduced.Trim('"');
            } while ((GetIdType(reduced) != IdType.Invalid) && (last != reduced));

            return last;
        }

        private static IdType GetIdType(string id) {
            if (RegexForId.IsMatch(id)) {
                return IdType.Id;
            }
            if (RegexForNumber.IsMatch(id)) {
                return IdType.Number;
            }
            if (RegexForHtml.IsMatch(id)) {
                return IdType.Html;
            }
            if (RegexForString.IsMatch(id)) {
                return IdType.String;
            }
            return IdType.Invalid;
        }

        private enum IdType {
            Invalid = 0,
            Id = 1,
            String = 2,
            Html = 3,
            Number = 4
        }
    }
}