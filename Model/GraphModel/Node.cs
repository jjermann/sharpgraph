using System;
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
            return Root.IsDirected
                ? Root.GetEdges().Where(e => e.EndNode.Equals(this))
                : ConnectedEdges();
        }

        public IEnumerable<IEdge> OutgoingEdges() {
            return Root.IsDirected
                ? Root.GetEdges().Where(e => e.SourceNode.Equals(this))
                : ConnectedEdges();
        }

        public IEnumerable<IEdge> ConnectedEdges() {
            return Root.GetEdges().Where(e => e.SourceNode.Equals(this) || e.EndNode.Equals(this));
        }

        public IEnumerable<INode> IncomingNeighbours() {
            return Root.IsDirected
                ? IncomingEdges().Select(e => e.SourceNode).Distinct()
                : ConnectedNeighbours();
        }

        public IEnumerable<INode> OutgoingNeighbours() {
            return Root.IsDirected
                ? OutgoingEdges().Select(e => e.EndNode).Distinct()
                : ConnectedNeighbours();
        }

        public IEnumerable<INode> ConnectedNeighbours() {
            return IncomingNeighbours().Union(OutgoingNeighbours()).Distinct();
        }

        public HashSet<INode> RecursiveSelect(Func<INode, IEnumerable<INode>> selectionFunc) {
            var selected = new HashSet<INode> {this};
            var affected = new HashSet<INode>(selectionFunc(this));
            foreach (var node in affected) {
                selected.UnionWith(node.RecursiveSelect(selectionFunc));
            }
            return selected;
        }

        public void VisitNeighbours(Func<INode, bool> stopCondition, Dictionary<INode, HashSet<INode>> visited) {
            if (stopCondition(this)) {
                return;
            }

            var neighbours = new HashSet<INode>(OutgoingNeighbours());
            foreach (var node in neighbours) {
                if (!visited.ContainsKey(node)) {
                    visited[node] = new HashSet<INode>();
                }
                if (!visited[node].Contains(this)) {
                    visited[node].Add(this);
                    node.VisitNeighbours(stopCondition, visited);
                }
            }
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