namespace ExampleGraphView {
    public interface IDatabaseManager {
        // ReSharper disable once UnusedMemberInSuper.Global
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Database GetDatabase();
    }
}