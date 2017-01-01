using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph.GraphModel {
    public class SubGraph : BaseObject, ISubGraph {
        // ReSharper disable once MemberCanBeProtected.Global
        public SubGraph(ISubGraph parentGraph, string id) : base(parentGraph, id) {
            NodeAttributes = new AttributeDictionary();
            EdgeAttributes = new AttributeDictionary();
        }

        protected IAttributeDictionary NodeAttributes { get; }
        protected IAttributeDictionary EdgeAttributes { get; }

        public virtual void SetNodeAttributes(IAttributeDictionary attrs) {
            NodeAttributes.SetAttributes(attrs);
        }

        public virtual bool HasNodeAttribute(string attr, bool recursive = false) {
            var hasLocalAttr = NodeAttributes.ContainsKey(attr);
            if (!recursive || hasLocalAttr || (Parent == null)) {
                return hasLocalAttr;
            }
            return Parent.HasNodeAttribute(attr, true);
        }

        //Warning: This function will throw an exception in case the attribute doesn't exist!
        public virtual string GetNodeAttribute(string attr, bool recursive = false) {
            if (!recursive || (Parent == null) || NodeAttributes.ContainsKey(attr)) {
                return NodeAttributes[attr];
            }
            return Parent.GetNodeAttribute(attr, true);
        }

        public virtual IAttributeDictionary GetNodeAttributes() {
            var attrs = new AttributeDictionary();
            foreach (var attr in NodeAttributes) {
                attrs[attr.Key] = attr.Value;
            }
            return attrs;
        }

        public virtual void SetEdgeAttributes(IAttributeDictionary attrs) {
            EdgeAttributes.SetAttributes(attrs);
        }

        public virtual bool HasEdgeAttribute(string attr, bool recursive = false) {
            var hasLocalAttr = EdgeAttributes.ContainsKey(attr);
            if (!recursive || hasLocalAttr || (Parent == null)) {
                return hasLocalAttr;
            }
            return Parent.HasEdgeAttribute(attr, true);
        }

        //Warning: This function will throw an exception in case the attribute doesn't exist!
        public virtual string GetEdgeAttribute(string attr, bool recursive = false) {
            if (!recursive || (Parent == null) || EdgeAttributes.ContainsKey(attr)) {
                return EdgeAttributes[attr];
            }
            return Parent.GetEdgeAttribute(attr, true);
        }

        public virtual IAttributeDictionary GetEdgeAttributes() {
            var attrs = new AttributeDictionary();
            foreach (var attr in EdgeAttributes) {
                attrs[attr.Key] = attr.Value;
            }
            return attrs;
        }

        public INode CreateNode(
            string id,
            IAttributeDictionary attrs,
            bool checkParent = true) {
            var node = new Node(this, id);
            var addedNode = Root.AddNode(node, checkParent);
            addedNode.SetAttributes(attrs);
            return addedNode;
        }

        public IEdge CreateEdge(
            INode sourceNode,
            INode endNode,
            IAttributeDictionary attrs = null) {
            var edge = new Edge(this, sourceNode, endNode);
            var addedEdge = Root.AddEdge(edge);
            addedEdge.SetAttributes(attrs);
            return addedEdge;
        }

        public ISubGraph CreateSubGraph(
            string id,
            IAttributeDictionary graphAttrs,
            IAttributeDictionary nodeAttrs,
            IAttributeDictionary edgeAttrs) {
            var subgraph = new SubGraph(this, id);
            var addedSubGraph = Root.AddSubGraph(subgraph);
            addedSubGraph.SetAttributes(graphAttrs);
            addedSubGraph.SetNodeAttributes(nodeAttrs);
            addedSubGraph.SetEdgeAttributes(edgeAttrs);
            return addedSubGraph;
        }

        public virtual IEnumerable<INode> GetSubGraphNodes(bool recursive) {
            if (!recursive) {
                return Root.GetNodes().Where(n => n.Parent.Equals(this));
            }
            var subgraphs = new List<ISubGraph> {this}.Concat(GetSubGraphSubGraphs(true).ToList());
            return Root.GetNodes().Where(n => subgraphs.Contains(n.Parent));
        }

        public virtual IEnumerable<IEdge> GetSubGraphEdges(bool recursive) {
            if (!recursive) {
                return Root.GetEdges().Where(e => e.Parent.Equals(this));
            }
            var subgraphs = new List<ISubGraph> {this}.Concat(GetSubGraphSubGraphs(true).ToList());
            return Root.GetEdges().Where(e => subgraphs.Contains(e.Parent));
        }

        public virtual IEnumerable<ISubGraph> GetSubGraphSubGraphs(bool recursive) {
            var subgraphs = Root.GetSubGraphs().Where(g => g.Parent.Equals(this)).ToList();
            if (!recursive) {
                return subgraphs;
            }
            return subgraphs.Concat(subgraphs.SelectMany(g => g.GetSubGraphSubGraphs(true)));
        }

        public virtual string ToDot(bool orderedByName, bool showRedundantNodes, bool bodyOnly = false,
            Func<INode, bool> nodeSelector = null) {
            const string newLine = "\n";
            var depth = SubGraphDepth;
            var indent = string.Concat(Enumerable.Repeat("  ", depth));
            var subIndent = string.Concat(Enumerable.Repeat("  ", depth + 1));

            var graphString = "";
            if (!bodyOnly) {
                var idStr = Id != null ? Id + " " : "";
                graphString += indent + "subgraph " + idStr;
            }
            graphString += "{" + newLine;


            if (Attributes.Count > 0) {
                graphString += subIndent + "graph " + Attributes + newLine;
            }
            if (NodeAttributes.Count > 0) {
                graphString += subIndent + "node " + NodeAttributes + newLine;
            }
            if (EdgeAttributes.Count > 0) {
                graphString += subIndent + "edge " + EdgeAttributes + newLine;
            }

            var subgraphs = orderedByName
                ? GetSubGraphSubGraphs(false).OrderBy(v => v)
                : GetSubGraphSubGraphs(false);
            foreach (var subgraph in subgraphs) {
                graphString += subgraph.ToDot(orderedByName, showRedundantNodes, nodeSelector: nodeSelector);
            }

            var nodes = orderedByName
                ? GetSubGraphNodes(false).OrderBy(v => v)
                : GetSubGraphNodes(false);
            var edges = orderedByName
                ? GetSubGraphEdges(false).OrderBy(v => v)
                : GetSubGraphEdges(false);
            var allEdges = Root.GetEdges().ToList();

            if (nodeSelector != null) {
                nodes = nodes.Where(nodeSelector);
                edges = edges.Where(e => nodeSelector(e.SourceNode) && nodeSelector(e.EndNode));
                allEdges = allEdges.Where(e => nodeSelector(e.SourceNode) && nodeSelector(e.EndNode)).ToList();
            }

            foreach (var node in nodes) {
                var containedInSourceEdges =
                    allEdges.Any(e => e.SourceNode.Equals(node) && (e.Parent == node.Parent));
                var containedInEndEdges =
                    allEdges.Any(e => e.EndNode.Equals(node) && (e.Parent == node.Parent));
                var containedInEdges = containedInSourceEdges || containedInEndEdges;
                var nodeAttributes = node.GetAttributes();
                var hasAttribute = nodeAttributes.Count > 0;
                var hasRedundantLabel = (nodeAttributes.Count == 1) &&
                                        nodeAttributes.ContainsKey("label") &&
                                        (nodeAttributes["label"] == node.Id);
                if (showRedundantNodes
                    || !containedInEdges
                    || (hasAttribute && !hasRedundantLabel)) {
                    graphString += subIndent + node + newLine;
                }
            }

            foreach (var edge in edges) {
                graphString += subIndent + edge + newLine;
            }

            graphString += indent + "}" + newLine;

            return graphString;
        }

        public override string ToString() {
            return ToDot(ModelHelper.OrderedByNames, ModelHelper.ShowRedundantNodes);
        }
    }
}