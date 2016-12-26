using System;
using System.Windows;
using Microsoft.Win32;

namespace SharpGraph.GraphView {
    public partial class MainWindow {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.dot";
            }
            ((GraphControllerViewModel.GraphController) DataContext).OriginalInputFile = initialFile;

            DotOutputWindow = new DotOutput(DataContext);
            ImageOutputWindow = new ImageOutput(DataContext);

            DotOutputWindow.Show();
            ImageOutputWindow.Show();
        }

        private DotOutput DotOutputWindow { get; }
        private ImageOutput ImageOutputWindow { get; }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                var vm = DataContext as GraphControllerViewModel.GraphController;
                if (vm == null) {
                    throw new ArgumentException("No valid ViewModel!");
                }
                vm.OpenFileCommand.Execute(openFileDialog.FileName);
            }
        }
    }
}