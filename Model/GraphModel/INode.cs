using System.Collections.Generic;

namespace SharpGraph.GraphModel {
    public interface INode : IBaseObject {
        IEnumerable<IEdge> IncomingEdges();
        IEnumerable<IEdge> OutgoingEdges();
        IEnumerable<IEdge> ConnectedEdges();
        IEnumerable<INode> IncomingNeighbours();
        IEnumerable<INode> OutgoingNeighbours();
        IEnumerable<INode> ConnectedNeighbours();
    }
}