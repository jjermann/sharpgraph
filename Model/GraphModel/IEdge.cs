namespace SharpGraph {
    public interface IEdge : IBaseObject {
        INode SourceNode { get; }
        INode EndNode { get; }
        // ReSharper disable once UnusedMemberInSuper.Global
        IPort SourcePort { get; }
        // ReSharper disable once UnusedMemberInSuper.Global
        IPort EndPort { get; }
    }
}