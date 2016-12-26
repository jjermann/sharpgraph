using System;
using System.Collections.Generic;

namespace SharpGraph.GraphModel {
    public class Graph : SubGraph, IGraph {
        public Graph(string id, bool isDirected, bool isStrict = false) : base(null, id) {
            IsDirected = isDirected;
            IsStrict = isStrict;
            Nodes = new Dictionary<INode, INode>();
            Edges = new Dictionary<IEdge, IEdge>();
            SubGraphs = new Dictionary<ISubGraph, ISubGraph>();
        }

        private IDictionary<INode, INode> Nodes { get; }
        private IDictionary<IEdge, IEdge> Edges { get; }
        private IDictionary<ISubGraph, ISubGraph> SubGraphs { get; }

        public bool IsStrict { get; }
        public bool IsDirected { get; }

        public INode AddNode(INode node, bool checkParent = true) {
            if (!Equals(node.Parent) && !SubGraphs.ContainsKey(node.Parent)) {
                throw new ArgumentException($"Parent of Node {node} not within Graph!");
            }
            if (!Nodes.ContainsKey(node)) {
                Nodes[node] = node;
            }
            var addedNode = Nodes[node];
            if (checkParent && (addedNode.Parent != node.Parent)) {
                throw new ArgumentException(
                    $"Mismatching node Parent ({addedNode.Parent.Id} vs {node.Parent.Id}) for node {node}!");
            }
            addedNode.SetAttributes(node.GetAttributes());
            return addedNode;
        }

        public IEdge AddEdge(IEdge edge) {
            if (!Equals(edge.Parent) && !SubGraphs.ContainsKey(edge.Parent)) {
                throw new ArgumentException($"Parent of Edge {edge} not within Graph!");
            }
            if ((edge.SourceNode == null) || !Nodes.ContainsKey(edge.SourceNode)) {
                throw new ArgumentException($"SourceNode {edge.SourceNode} not within Graph!");
            }
            if ((edge.EndNode == null) || !Nodes.ContainsKey(edge.EndNode)) {
                throw new ArgumentException($"EndNode {edge.EndNode} not within Graph!");
            }
            if (!Edges.ContainsKey(edge)) {
                Edges[edge] = edge;
            }
            var addedEdge = Edges[edge];
            addedEdge.SetAttributes(edge.GetAttributes());
            return addedEdge;
        }

        public ISubGraph AddSubGraph(ISubGraph subgraph) {
            if (!Equals(subgraph.Parent) && !SubGraphs.ContainsKey(subgraph.Parent)) {
                throw new ArgumentException($"Parent of SubGraph {subgraph.Id} not within Graph!");
            }
            if (!SubGraphs.ContainsKey(subgraph)) {
                SubGraphs[subgraph] = subgraph;
            }
            var addedSubGraph = SubGraphs[subgraph];
            addedSubGraph.SetAttributes(subgraph.GetAttributes());
            return addedSubGraph;
        }

        public IEnumerable<INode> GetNodes() {
            return Nodes.Values;
        }

        public IEnumerable<IEdge> GetEdges() {
            return Edges.Values;
        }

        public IEnumerable<ISubGraph> GetSubGraphs() {
            return SubGraphs.Values;
        }

        public override IGraph Root => this;

        public override string ToDot(bool orderedByName, bool showRedundantNodes, bool bodyOnly = false,
            Func<INode, bool> nodeSelector = null) {
            var strict = IsStrict ? ModelHelper.StrictGraphName + " " : "";
            var graphType = IsDirected ? ModelHelper.DirectedGraphName : ModelHelper.UndirectedGraphName;
            var graphId = " " + Id;
            var graphString = $"{strict}{graphType}{graphId} " +
                              base.ToDot(orderedByName, showRedundantNodes, true, nodeSelector);
            return graphString;
        }

        public static IGraph CreateGraph(
            string id = ModelHelper.DefaultGraphId,
            bool isDirected = true,
            bool isStrict = false,
            IAttributeDictionary graphAttrs = null,
            IAttributeDictionary nodeAttrs = null,
            IAttributeDictionary edgeAttrs = null) {
            var graph = new Graph(id, isDirected, isStrict);

            if (graphAttrs != null) {
                graph.Attributes.SetAttributes(graphAttrs);
            }
            if (nodeAttrs != null) {
                graph.NodeAttributes.SetAttributes(nodeAttrs);
            }
            if (edgeAttrs != null) {
                graph.EdgeAttributes.SetAttributes(edgeAttrs);
            }

            return graph;
        }

        public override string ToString() {
            return ToDot(ModelHelper.OrderedByNames, ModelHelper.ShowRedundantNodes);
        }
    }
}