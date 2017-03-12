using System.Windows;

namespace ExampleGraphView {
    public partial class DatabaseView {
        private GraphController DatabaseViewModel { get; set; }

        private void DatabaseView_Startup(object sender, StartupEventArgs e) {
            var databaseManager = new DatabaseManager();
            var graph = databaseManager.GetDatabase().RelationColumnsOnly().ToGraph();
            var graphDot = graph.ToDot();

            DatabaseViewModel = new GraphController {
                GraphUpdateMode = GraphController.UpdateMode.ImmediateUpdate,
                OriginalDotContent = graphDot,
                RestrictVisibility = false
            };

            var databaseWindow = new MainDatabaseWindow(DatabaseViewModel) {Title = "DatabaseView"};
            databaseWindow.Show();
        }
    }
}