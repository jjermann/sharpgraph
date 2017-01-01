using System.Diagnostics.CodeAnalysis;

namespace SharpGraph.ExternalRunners {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IDotRunner<out T> {
        T GetOutput(string input);
    }
}