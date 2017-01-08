using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IDotRunner<out T> {
        T GetOutput(string input);
    }
}