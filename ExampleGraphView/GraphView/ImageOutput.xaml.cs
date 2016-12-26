using System.Windows;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;

namespace SharpGraph.GraphView {
    public partial class ImageOutput {
        public ImageOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private void FileSaveAs_OnClick(object sender, RoutedEventArgs e) {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true) {
                var vm = (GraphController) DataContext;
                if (vm.SaveImageFileCommand.CanExecute(saveFileDialog.FileName)) {
                    vm.SaveImageFileCommand.Execute(saveFileDialog.FileName);
                }
            }
        }
    }
}