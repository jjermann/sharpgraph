using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IPort {
        string Name { get; }
        Compass Compass { get; }
    }
}