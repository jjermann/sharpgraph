using System.Windows;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;

namespace SharpGraph.GraphView {
    public partial class DotInput {
        public DotInput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private void FileSave_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = vm.SavedOriginalDotFile
                           ?? FileDialogHandler<SaveFileDialog>.OpenDialog(vm.OriginalInputFile);
            if ((filename != null) && vm.SaveOriginalDotFileCommand.CanExecute(filename)) {
                vm.SaveOriginalDotFileCommand.Execute(filename);
            }
        }

        private void FileSaveAs_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = FileDialogHandler<SaveFileDialog>.OpenDialog(
                vm.SavedOriginalDotFile ?? vm.OriginalInputFile);
            if ((filename != null) && vm.SaveOriginalDotFileCommand.CanExecute(filename)) {
                vm.SaveOriginalDotFileCommand.Execute(filename);
            }
        }
    }
}