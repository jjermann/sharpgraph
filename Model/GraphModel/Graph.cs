using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpGraph {
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
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!Equals(node.Parent) && !SubGraphs.ContainsKey(node.Parent)) {
                throw new ArgumentException(FormattableString.Invariant($"Parent of Node {node} not within Graph!"));
            }
            if (!Nodes.ContainsKey(node)) {
                Nodes[node] = node;
            } else {}
            var addedNode = Nodes[node];
            if (checkParent && (addedNode.Parent != node.Parent)) {
                throw new ArgumentException(FormattableString.Invariant(
                    $"Mismatching node Parent ({addedNode.Parent.Id} vs {node.Parent.Id}) for node {node}!"));
            }
            addedNode.SetAttributes(node.GetAttributes());
            return addedNode;
        }

        public IEdge AddEdge(IEdge edge) {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
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
            if (subgraph == null) throw new ArgumentNullException(nameof(subgraph));
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

        private static HashSet<INode> RestrictVisited(
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

        public override string ToDot(DotFormatOptions dotOption = null) {
            var suboption = dotOption?.Clone() ?? new DotFormatOptions();
            suboption.BodyOnly = true;

            var strict = IsStrict ? ModelHelper.StrictGraphName + " " : "";
            var graphType = IsDirected ? ModelHelper.DirectedGraphName : ModelHelper.UndirectedGraphName;
            var graphId = " " + Id;
            var graphString = FormattableString.Invariant($"{strict}{graphType}{graphId} ") +
                              base.ToDot(suboption);
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

        public IDictionary<INode, HashSet<INode>> GetOutgoingNeighboursDictionary() {
            var neighbourDictionary = new Dictionary<INode, HashSet<INode>>();
            foreach (var node in GetNodes()) {
                neighbourDictionary[node] = new HashSet<INode>();
            }
            foreach (var edge in GetEdges()) {
                var sourceNode = edge.SourceNode;
                var endNode = edge.EndNode;
                neighbourDictionary[sourceNode].Add(endNode);
            }
            return neighbourDictionary;
        }

        public ICollection<HashSet<INode>> GetStronglyConnectedComponents() {
            var output = new Collection<HashSet<INode>>();
            var neighbourDictionary = GetOutgoingNeighboursDictionary();
            var nodeStack = new Stack<INode>();
            var currentIndex = 0;
            var indexDictionary = new Dictionary<INode, int>();
            var lowlinkDictionary = new Dictionary<INode, int>();
            var nodes = GetNodes().ToList();
            foreach (var node in nodes) {
                indexDictionary[node] = -1;
                lowlinkDictionary[node] = -1;
            }

            foreach (var node in nodes) {
                if (indexDictionary[node] < 0) {
                    StrongConnect(
                        output,
                        neighbourDictionary,
                        nodeStack,
                        ref currentIndex,
                        indexDictionary,
                        lowlinkDictionary,
                        node);
                }
            }
            return output;
        }

        private static void StrongConnect(
            ICollection<HashSet<INode>> output,
            IDictionary<INode, HashSet<INode>> neighbourDictionary,
            Stack<INode> nodeStack,
            ref int currentIndex,
            IDictionary<INode, int> indexDictionary,
            IDictionary<INode, int> lowlinkDictionary,
            INode node) {
            indexDictionary[node] = currentIndex;
            lowlinkDictionary[node] = currentIndex;
            currentIndex++;
            nodeStack.Push(node);

            foreach (var neighbour in neighbourDictionary[node]) {
                if (indexDictionary[neighbour] < 0) {
                    StrongConnect(
                        output,
                        neighbourDictionary,
                        nodeStack,
                        ref currentIndex,
                        indexDictionary,
                        lowlinkDictionary,
                        neighbour);
                    lowlinkDictionary[node] = Math.Min(lowlinkDictionary[node], lowlinkDictionary[neighbour]);
                } else if (nodeStack.Contains(neighbour)) {
                    lowlinkDictionary[node] = Math.Min(lowlinkDictionary[node], indexDictionary[neighbour]);
                }
            }

            if (lowlinkDictionary[node] == indexDictionary[node]) {
                var scc = new HashSet<INode>();
                INode nextNode;
                do {
                    nextNode = nodeStack.Pop();
                    scc.Add(nextNode);
                } while (node != nextNode);
                output.Add(scc);
            }
        }

        public object Clone() {
            return GetReducedGraph(null);
        }

        //nodes == null returns a complete clone, otherwise the clone is restricted to the specified nodes
        public IGraph GetReducedGraph(ICollection<INode> nodes) {
            var thisGraph = this as ISubGraph;
            var graph = CreateGraph(Id, IsDirected, IsStrict, Attributes, NodeAttributes, EdgeAttributes);
            var subgraphDictionary = new Dictionary<ISubGraph, ISubGraph> {[thisGraph] = graph};
            CloneAndAddAllSubgraphs(graph, thisGraph, subgraphDictionary);
            foreach (var node in GetNodes()) {
                if ((nodes != null) && !nodes.Contains(node)) {
                    continue;
                }
                var clonedNode = new Node(subgraphDictionary[node.Parent], node.Id);
                clonedNode.SetAttributes(node.GetAttributes());
                graph.AddNode(clonedNode);
            }
            foreach (var edge in GetEdges()) {
                if ((nodes != null) && (!nodes.Contains(edge.SourceNode) || !nodes.Contains(edge.EndNode))) {
                    continue;
                }
                var clonedEdge = new Edge(subgraphDictionary[edge.Parent], edge.SourceNode, edge.EndNode, edge.EndPort);
                clonedEdge.SetAttributes(edge.GetAttributes());
                graph.AddEdge(clonedEdge);
            }

            if (nodes != null) {
                graph.RemoveEmptySubgraphs();
            }
            return graph;
        }

        public void RemoveEmptySubgraphs() {
            IList<ISubGraph> emptySubgraphs;
            do {
                emptySubgraphs = GetSubGraphs().Where(
                    sg =>
                        !sg.GetSubGraphSubGraphs().Any() &&
                        !sg.GetSubGraphNodes().Any() &&
                        !sg.GetSubGraphEdges().Any()).ToList();
                foreach (var subgraph in emptySubgraphs) {
                    SubGraphs.Remove(subgraph);
                }
            } while (emptySubgraphs.Any());
        }

        private void CloneAndAddAllSubgraphs(IGraph root, ISubGraph currentSubgraph,
            IDictionary<ISubGraph, ISubGraph> subgraphDictionary) {
            foreach (var subgraph in currentSubgraph.GetSubGraphSubGraphs()) {
                var clonedSubgraph = new SubGraph(subgraphDictionary[currentSubgraph], subgraph.Id);
                clonedSubgraph.SetAttributes(subgraph.GetAttributes());
                clonedSubgraph.SetNodeAttributes(subgraph.GetNodeAttributes());
                clonedSubgraph.SetEdgeAttributes(subgraph.GetEdgeAttributes());
                var addedSubgraph = root.AddSubGraph(clonedSubgraph);
                subgraphDictionary[subgraph] = addedSubgraph;
                CloneAndAddAllSubgraphs(root, subgraph, subgraphDictionary);
            }
        }

        public override string ToString() {
            return ToDot();
        }
    }
}