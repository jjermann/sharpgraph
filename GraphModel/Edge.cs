using System;
using System.Runtime.CompilerServices;

namespace SharpGraph.GraphModel {
    public class Edge : BaseObject, IEdge {
        public Edge(ISubGraph parentGraph, INode a, INode b)
            : base(parentGraph, GetNodeIdFromParameters(parentGraph, a, b)) {
            IsDirected = parentGraph.Root.IsDirected;
            IsStrict = parentGraph.Root.IsStrict;
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

        //Also see Equals and GetHashCode, since this sets the id and the comparison/equality is based on the id that should already work
        private static string GetNodeIdFromParameters(IBaseObject parent, IBaseObject a, IBaseObject b) {
            var id = @"""";
            id += "node" + ModelHelper.ReduceId(a.Id) + ModelHelper.ReduceId(b.Id);
            //id += "node" + ModelHelper.ReduceId(parent.Id) + ModelHelper.ReduceId(a.Id) + ModelHelper.ReduceId(b.Id);
            if (parent.Root.IsStrict) {
                id += Guid.NewGuid();
            }
            id += @"""";
            return ModelHelper.ReduceId(id);
        }

        public bool IsDirected { get; }
        public bool IsStrict { get; }
        public virtual INode EndNode { get; }
        public virtual INode SourceNode { get; }

        public override string ToString() {
            var output = $"{SourceNode.Id} {Connector} {EndNode.Id}";
            if (Attributes.Count > 0) {
                output += " " + Attributes;
            }
            return output;
        }

        protected virtual string Connector => IsDirected
            ? ModelHelper.DirectedEdgeOpName
            : ModelHelper.UndirectedEdgeOpName;

        public virtual int CompareTo(IEdge other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            var parentCmp = Parent.CompareTo(other.Parent);
            if (parentCmp != 0) {
                return parentCmp;
            }
            var sourceCmp = SourceNode.CompareTo(other.SourceNode);
            if (sourceCmp != 0) {
                return sourceCmp;
            }
            var endCmp = EndNode.CompareTo(other.EndNode);
            if (endCmp != 0) {
                return sourceCmp;
            }
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        //Actually we don't need this, see GetNodeIdFromParameters
        public virtual bool Equals(IEdge other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (IsStrict) {
                return false;
            }
            return base.Equals(other) && SourceNode.Equals(other.SourceNode) && EndNode.Equals(other.EndNode);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as IEdge;
            return (other != null) && Equals(other);
        }

        //Actually we don't need this, see GetNodeIdFromParameters
        public override int GetHashCode() {
            unchecked {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ EndNode.GetHashCode();
                hashCode = (hashCode*397) ^ SourceNode.GetHashCode();
                if (IsStrict) {
                    hashCode = (hashCode*397) ^ RuntimeHelpers.GetHashCode(this);
                }
                return hashCode;
            }
        }
    }
}