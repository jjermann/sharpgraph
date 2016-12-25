namespace SharpGraph.GraphModel {
    public interface IEdge : IBaseObject {
        INode SourceNode { get; }
        INode EndNode { get; }
    }
}