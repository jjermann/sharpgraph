namespace SharpGraph.ExternalRunners {
    public interface IDotRunner<out T> {
        T GetOutput(string input);
    }
}