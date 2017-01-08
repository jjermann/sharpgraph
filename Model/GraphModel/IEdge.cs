namespace SharpGraph {
    public interface IEdge : IBaseObject {
        INode SourceNode { get; }
        INode EndNode { get; }
    }
}