using System;
using System.Collections.Generic;

namespace SharpGraph {
    public interface IGraph : ISubGraph, ICloneable {
        bool IsStrict { get; }
        bool IsDirected { get; }
        // ReSharper disable once UnusedParameter.Global
        INode AddNode(INode node, bool checkParent = false);
        IEdge AddEdge(IEdge edge);
        ISubGraph AddSubGraph(ISubGraph subgraph);
        IEnumerable<INode> GetNodes();
        IEnumerable<IEdge> GetEdges();
        IEnumerable<ISubGraph> GetSubGraphs();
        IGraph GetReducedGraph(ICollection<INode> nodes);
        void RemoveEmptySubgraphs();
        IDictionary<INode, HashSet<INode>> GetOutgoingNeighboursDictionary();
        ICollection<HashSet<INode>> GetStronglyConnectedComponents();

        Func<INode, bool> GetNodeSelector(
            IEnumerable<string> selectedIdList,
            Func<INode, bool> stopCondition = null,
            Func<INode, bool> acceptCondition = null);
    }
}