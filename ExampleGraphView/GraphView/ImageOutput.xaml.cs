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
            var filename = vm.SavedImageFile ?? GetSaveFileDialogResult();
            if (vm.SaveImageFileCommand.CanExecute(filename)) {
                vm.SaveImageFileCommand.Execute(filename);
            }
        }

        private void FileSaveAs_OnClick(object sender, RoutedEventArgs e) {
            var vm = (GraphController) DataContext;
            var filename = GetSaveFileDialogResult();
            if (vm.SaveImageFileCommand.CanExecute(filename)) {
                vm.SaveImageFileCommand.Execute(filename);
            }
        }

        private string GetSaveFileDialogResult() {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true) {
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}