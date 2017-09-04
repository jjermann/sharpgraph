using System.Diagnostics.CodeAnalysis;

namespace ExampleGraphView {
    public interface IDatabaseManager {
        // ReSharper disable once UnusedMemberInSuper.Global
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Database GetDatabase();
    }
}