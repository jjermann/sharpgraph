using System.Windows;

namespace ExampleGraphView {
    public partial class SharpGraph {
        private void SharpGraph_Startup(object sender, StartupEventArgs e) {
            string initialFile = null;
            if (e.Args.Length > 0) {
                initialFile = e.Args[0];
            }
            var mainWindow = new MainWindow(initialFile);
            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}