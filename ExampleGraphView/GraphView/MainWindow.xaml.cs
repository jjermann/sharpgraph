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
            DotInputWindow = new DotInput(DataContext);

            DotOutputWindow.Show();
            ImageOutputWindow.Show();
            DotInputWindow.Show();
        }

        private DotOutput DotOutputWindow { get; }
        private ImageOutput ImageOutputWindow { get; }
        private DotInput DotInputWindow { get; }

        private void FileOpen_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = GetOpenFileDialogResult();
            if (vm.OpenFileCommand.CanExecute(filename)) {
                vm.OpenFileCommand.Execute(filename);
            }
        }

        private string GetOpenFileDialogResult() {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}