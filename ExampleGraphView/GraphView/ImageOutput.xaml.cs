using System.Windows;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;

namespace SharpGraph.GraphView {
    public partial class ImageOutput {
        public ImageOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private void FileSave_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = vm.SavedImageFile ?? FileDialogHandler<SaveFileDialog>.OpenDialog();
            if ((filename != null) && vm.SaveImageFileCommand.CanExecute(filename)) {
                vm.SaveImageFileCommand.Execute(filename);
            }
        }

        private void FileSaveAs_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = FileDialogHandler<SaveFileDialog>.OpenDialog(vm.SavedImageFile);
            if ((filename != null) && vm.SaveImageFileCommand.CanExecute(filename)) {
                vm.SaveImageFileCommand.Execute(filename);
            }
        }
    }
}