using System.Windows;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;

namespace SharpGraph.GraphView {
    public partial class MainWindow {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.dot";
            }
            var vm = (GraphController) DataContext;
            if (vm.OpenFileCommand.CanExecute(initialFile)) {
                vm.OpenFileCommand.Execute(initialFile);
            }
            DotOutputWindow = new DotOutput(DataContext);
            ImageOutputWindow = new ImageOutput(DataContext);

            DotOutputWindow.Show();
            ImageOutputWindow.Show();
        }

        private DotOutput DotOutputWindow { get; }
        private ImageOutput ImageOutputWindow { get; }

        private void FileOpen_OnClick(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                var vm = (GraphController) DataContext;
                if (vm.OpenFileCommand.CanExecute(openFileDialog.FileName)) {
                    vm.OpenFileCommand.Execute(openFileDialog.FileName);
                }
            }
        }
    }
}