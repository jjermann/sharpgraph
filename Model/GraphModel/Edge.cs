using System;

namespace SharpGraph.GraphModel {
    public class Edge : BaseObject, IEdge {
        public Edge(ISubGraph parentGraph, INode a, INode b)
            : base(parentGraph, GetNodeIdFromParameters(parentGraph, a, b)) {
            IsDirected = parentGraph.Root.IsDirected;
            if (IsDirected) {
                SourceNode = a;
                EndNode = b;
            } else if (a.CompareTo(b) > 0) {
                SourceNode = b;
                EndNode = a;
            } else {
                SourceNode = a;
                EndNode = b;
            }
        }

        private bool IsDirected { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        protected string Connector => IsDirected
            ? ModelHelper.DirectedEdgeOpName
            : ModelHelper.UndirectedEdgeOpName;

        public INode EndNode { get; }
        public INode SourceNode { get; }

        public override bool HasAttribute(string attr, bool recursive = false) {
            var hasLocalAttr = Attributes.ContainsKey(attr);
            if (!recursive || hasLocalAttr || (Parent == null)) {
                return hasLocalAttr;
            }
            return Parent.HasEdgeAttribute(attr, true);
        }

        //Warning: This function will throw an exception in case the attribute doesn't exist!
        public override string GetAttribute(string attr, bool recursive = false) {
            if (!recursive || (Parent == null) || Attributes.ContainsKey(attr)) {
                return Attributes[attr];
            }
            return Parent.GetEdgeAttribute(attr, true);
        }

        //This method is very important for equality and comparison!
        //It is closely related to the constructor of Edge (there is some code duplication).
        private static string GetNodeIdFromParameters(IBaseObject parent, IBaseObject a, IBaseObject b) {
            var id = @"""";
            if (!parent.Root.IsDirected && (a.CompareTo(b) > 0)) {
                id += "node" + ModelHelper.ReduceId(b.Id) + ModelHelper.ReduceId(a.Id);
            } else {
                id += "node" + ModelHelper.ReduceId(a.Id) + ModelHelper.ReduceId(b.Id);
            }
            //TODO: How to handle changing parents for strict nodes?
            //id += "node" + ModelHelper.ReduceId(parent.Id) + ModelHelper.ReduceId(a.Id) + ModelHelper.ReduceId(b.Id);
            if (!parent.Root.IsStrict) {
                id += Guid.NewGuid();
            }
            id += @"""";
            return ModelHelper.ReduceId(id);
        }

        public override string ToString() {
            var output = $"{SourceNode.Id} {Connector} {EndNode.Id}";
            if (Attributes.Count > 0) {
                output += " " + Attributes;
            }
            return output;
        }
    }
}