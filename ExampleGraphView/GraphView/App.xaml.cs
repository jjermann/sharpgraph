using System.Windows;

namespace SharpGraph.GraphView {
    public partial class App {
        private void Application_Startup(object sender, StartupEventArgs e) {
            string initialFile = null;
            if (e.Args.Length > 0) {
                initialFile = e.Args[0];
            }
            new MainWindow(initialFile).Show();
        }
    }
}