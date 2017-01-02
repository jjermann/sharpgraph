using System;
using System.Collections.Generic;

namespace SharpGraph.GraphModel {
    public interface IGraph : ISubGraph {
        bool IsStrict { get; }
        bool IsDirected { get; }
        // ReSharper disable once UnusedParameter.Global
        INode AddNode(INode node, bool checkParent = false);
        IEdge AddEdge(IEdge edge);
        ISubGraph AddSubGraph(ISubGraph subgraph);
        IEnumerable<INode> GetNodes();
        IEnumerable<IEdge> GetEdges();
        IEnumerable<ISubGraph> GetSubGraphs();

        Func<INode, bool> GetNodeSelector(
            IEnumerable<string> selectedIdList,
            Func<INode, bool> stopCondition = null,
            Func<INode, bool> acceptCondition = null);
    }
}