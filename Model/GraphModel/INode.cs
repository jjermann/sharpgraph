using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpGraph.GraphModel {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface INode : IBaseObject {
        IEnumerable<IEdge> IncomingEdges();
        IEnumerable<IEdge> OutgoingEdges();
        IEnumerable<IEdge> ConnectedEdges();
        IEnumerable<INode> IncomingNeighbours();
        IEnumerable<INode> OutgoingNeighbours();
        IEnumerable<INode> ConnectedNeighbours();
    }
}