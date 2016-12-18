using System;

namespace SharpGraph.GraphModel {
    public interface IEdge : IBaseObject, IComparable<IEdge>, IEquatable<IEdge> {
        INode SourceNode { get; }
        INode EndNode { get; }
    }
}