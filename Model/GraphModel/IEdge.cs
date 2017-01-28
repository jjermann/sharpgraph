using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IEdge : IBaseObject {
        INode SourceNode { get; }
        INode EndNode { get; }
        IPort SourcePort { get; }
        IPort EndPort { get; }
    }
}