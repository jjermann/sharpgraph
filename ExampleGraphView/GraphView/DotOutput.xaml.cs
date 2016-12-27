using System.Windows;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;

namespace SharpGraph.GraphView {
    public partial class DotOutput {
        public DotOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private void FileSave_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = vm.SavedDotFile ?? FileDialogHandler<SaveFileDialog>.OpenDialog();
            if ((filename != null) && vm.SaveDotFileCommand.CanExecute(filename)) {
                vm.SaveDotFileCommand.Execute(filename);
            }
        }

        private void FileSaveAs_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = FileDialogHandler<SaveFileDialog>.OpenDialog(vm.SavedDotFile);
            if ((filename != null) && vm.SaveDotFileCommand.CanExecute(filename)) {
                vm.SaveDotFileCommand.Execute(filename);
            }
        }
    }
}