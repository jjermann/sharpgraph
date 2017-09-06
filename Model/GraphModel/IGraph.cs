using System;
using System.Collections.Generic;

namespace SharpGraph {
    public interface IGraph : ISubGraph, ICloneable {
        bool IsStrict { get; }
        bool IsDirected { get; }
        INode AddNode(INode node, bool checkParent = false);
        void RemoveNode(INode node);
        IEdge AddEdge(IEdge edge);
        void RemoveEdge(IEdge edge);
        ISubGraph AddSubGraph(ISubGraph subgraph);
        void RemoveSubGraph(ISubGraph subgraph);
        IEnumerable<INode> GetNodes();
        IEnumerable<IEdge> GetEdges();
        IEnumerable<ISubGraph> GetSubGraphs();

        IGraph GetReducedGraph(ICollection<INode> nodes);
        void RemoveEmptySubgraphs();
        IDictionary<INode, HashSet<INode>> GetOutgoingNeighboursDictionary();
        ICollection<HashSet<INode>> GetStronglyConnectedComponents();
        IGraph GetReachabilityGraph(ICollection<INode> nodes);

        Func<INode, bool> GetNodeSelector(
            IEnumerable<string> selectedIdList,
            Func<INode, bool> stopCondition = null,
            Func<INode, bool> acceptCondition = null);
    }
}