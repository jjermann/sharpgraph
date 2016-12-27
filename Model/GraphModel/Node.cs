using System.Collections.Generic;
using System.Linq;

namespace SharpGraph.GraphModel {
    public class Node : BaseObject, INode {
        public Node(ISubGraph parentGraph, string id) : base(parentGraph, id) {}

        public override bool HasAttribute(string attr, bool recursive = false) {
            var hasLocalAttr = Attributes.ContainsKey(attr);
            if (!recursive || hasLocalAttr || (Parent == null)) {
                return hasLocalAttr;
            }
            return Parent.HasNodeAttribute(attr, true);
        }

        //Warning: This function will throw an exception in case the attribute doesn't exist!
        public override string GetAttribute(string attr, bool recursive = false) {
            if (!recursive || (Parent == null) || Attributes.ContainsKey(attr)) {
                return Attributes[attr];
            }
            return Parent.GetNodeAttribute(attr, true);
        }

        public IEnumerable<IEdge> IncomingEdges() {
            return Root.GetEdges().Where(e => e.EndNode.Equals(this));
        }

        public IEnumerable<IEdge> OutgoingEdges() {
            return Root.GetEdges().Where(e => e.SourceNode.Equals(this));
        }

        public IEnumerable<IEdge> ConnectedEdges() {
            return Root.GetEdges().Where(e => e.SourceNode.Equals(this) || e.EndNode.Equals(this));
        }

        public IEnumerable<INode> IncomingNeighbours() {
            return IncomingEdges().Select(e => e.SourceNode).Distinct();
        }

        public IEnumerable<INode> OutgoingNeighbours() {
            return OutgoingEdges().Select(e => e.EndNode).Distinct();
        }

        public IEnumerable<INode> ConnectedNeighbours() {
            return IncomingNeighbours().Union(OutgoingNeighbours()).Distinct();
        }

        public override string ToString() {
            var output = Id;
            if (Attributes.Count > 0) {
                output += " " + Attributes;
            }
            return output;
        }
    }
}