using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGraph.GraphModel {
    public class Graph : SubGraph, IGraph {
        // ReSharper disable once MemberCanBePrivate.Global
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
                throw new ArgumentException(FormattableString.Invariant($"Parent of Node {node} not within Graph!"));
            }
            if (!Nodes.ContainsKey(node)) {
                Nodes[node] = node;
            }
            var addedNode = Nodes[node];
            if (checkParent && (addedNode.Parent != node.Parent)) {
                throw new ArgumentException(FormattableString.Invariant(
                    $"Mismatching node Parent ({addedNode.Parent.Id} vs {node.Parent.Id}) for node {node}!"));
            }
            addedNode.SetAttributes(node.GetAttributes());
            return addedNode;
        }

        public IEdge AddEdge(IEdge edge) {
            if (!Equals(edge.Parent) && !SubGraphs.ContainsKey(edge.Parent)) {
                throw new ArgumentException(FormattableString.Invariant($"Parent of Edge {edge} not within Graph!"));
            }
            if ((edge.SourceNode == null) || !Nodes.ContainsKey(edge.SourceNode)) {
                throw new ArgumentException(
                    FormattableString.Invariant($"SourceNode {edge.SourceNode} not within Graph!"));
            }
            if ((edge.EndNode == null) || !Nodes.ContainsKey(edge.EndNode)) {
                throw new ArgumentException(FormattableString.Invariant($"EndNode {edge.EndNode} not within Graph!"));
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
                throw new ArgumentException(
                    FormattableString.Invariant($"Parent of SubGraph {subgraph.Id} not within Graph!"));
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

        public Func<INode, bool> GetNodeSelector(
            IEnumerable<string> selectedIdList,
            Func<INode, bool> stopCondition = null,
            Func<INode, bool> acceptCondition = null) {
            if (stopCondition == null) {
                stopCondition = node => true;
            }
            if (acceptCondition == null) {
                acceptCondition = node => true;
            }
            var initialNodes = Nodes.Values.Where(n => selectedIdList.Contains(n.Id)).ToList();
            var visited = VisitNodes(initialNodes, stopCondition);
            var restricted = RestrictVisited(initialNodes, visited, stopCondition, acceptCondition);

            return node => restricted.Contains(node);
        }

        private HashSet<INode> RestrictVisited(
            List<INode> initialNodes,
            HashSet<INode> visited,
            Func<INode, bool> stopCondition,
            Func<INode, bool> acceptCondition) {
            var restricted = new HashSet<INode>(visited);
            var newStopNodes = new HashSet<INode>(visited.Where(stopCondition).Except(initialNodes));
            var unacceptedNodes = new HashSet<INode>(newStopNodes.Where(n => !acceptCondition(n)));
            Func<INode, IEnumerable<INode>> selectionFunc = node => node.OutgoingNeighbours()
                .Except(initialNodes)
                .Where(restricted.Contains);
            var unwantedCollection = unacceptedNodes.Select(n => n.RecursiveSelect(selectionFunc));
            foreach (var unwanted in unwantedCollection) {
                restricted.ExceptWith(unwanted);
            }

            var keepGoing = true;
            while (keepGoing) {
                keepGoing = false;
                var toRemove = new HashSet<INode>(restricted.Where(n => !stopCondition(n))
                    .Except(initialNodes)
                    .Where(n => !n.OutgoingNeighbours().Intersect(restricted).Any()));
                if (toRemove.Any()) {
                    restricted.ExceptWith(toRemove);
                    keepGoing = true;
                }
            }

            return restricted;
        }

        private static HashSet<INode> VisitNodes(IEnumerable<INode> initial, Func<INode, bool> stopCondition) {
            var initialHashSet = new HashSet<INode>(initial);
            var visited = GetVisitedFromStopCondition(initialHashSet, stopCondition);
            ApplyIncomingCondition(visited, initialHashSet);
            return new HashSet<INode>(visited.Keys);
        }

        private static Dictionary<INode, HashSet<INode>> GetVisitedFromStopCondition(
            HashSet<INode> initialHashSet,
            Func<INode, bool> stopCondition) {
            var visited = new Dictionary<INode, HashSet<INode>>();
            foreach (var node in initialHashSet) {
                visited[node] = new HashSet<INode>(node.IncomingNeighbours());
            }
            foreach (var node in initialHashSet) {
                var outgoingNeighbours = new HashSet<INode>(
                    node.OutgoingNeighbours()
                        .Where(n => !visited.ContainsKey(n) || !visited[n].Contains(node)));
                foreach (var neighbour in outgoingNeighbours) {
                    if (!visited.ContainsKey(neighbour)) {
                        visited[neighbour] = new HashSet<INode>();
                    }
                    if (!visited[neighbour].Contains(node)) {
                        visited[neighbour].Add(node);
                        neighbour.VisitNeighbours(stopCondition, visited);
                    }
                }
            }
            return visited;
        }

        private static void ApplyIncomingCondition(Dictionary<INode, HashSet<INode>> visited,
            HashSet<INode> initialHashSet) {
            var keepGoing = true;
            while (keepGoing) {
                keepGoing = false;
                var newVisitedNodes = visited.Keys.Except(initialHashSet).ToList();
                foreach (var node in newVisitedNodes) {
                    var incomingNeighbours = new HashSet<INode>(node.IncomingNeighbours());
                    if (incomingNeighbours.Except(visited[node]).Any()) {
                        visited.Remove(node);
                        foreach (var n in visited.Values) {
                            n.Remove(node);
                        }
                        keepGoing = true;
                    }
                }
            }
        }

        public override IGraph Root => this;

        public override string ToDot(bool orderedByName, bool showRedundantNodes, bool bodyOnly = false,
            Func<INode, bool> nodeSelector = null) {
            var strict = IsStrict ? ModelHelper.StrictGraphName + " " : "";
            var graphType = IsDirected ? ModelHelper.DirectedGraphName : ModelHelper.UndirectedGraphName;
            var graphId = " " + Id;
            var graphString = FormattableString.Invariant($"{strict}{graphType}{graphId} ") +
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