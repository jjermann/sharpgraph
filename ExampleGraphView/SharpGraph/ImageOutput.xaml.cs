using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph.BaseViewModel;

namespace ExampleGraphView {
    public partial class ImageOutput {
        public ImageOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private RelayCommand _saveCommand;
        public ICommand SaveCommand {
            get {
                return _saveCommand ?? (_saveCommand = new RelayCommand(
                           param => {
                               var vm = (GraphController) DataContext;
                               var filename = vm.SavedImageFile ?? FileDialogHandler<SaveFileDialog>.OpenDialog();
                               if ((filename != null) && vm.SaveImageFileCommand.CanExecute(filename)) {
                                   vm.SaveImageFileCommand.Execute(filename);
                               }
                           }
                       ));
            }
        }

        private RelayCommand _saveAsCommand;
        public ICommand SaveAsCommand {
            get {
                return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(
                           param => {
                               var vm = (GraphController) DataContext;
                               var filename = FileDialogHandler<SaveFileDialog>.OpenDialog(vm.SavedImageFile);
                               if ((filename != null) && vm.SaveImageFileCommand.CanExecute(filename)) {
                                   vm.SaveImageFileCommand.Execute(filename);
                               }
                           }
                       ));
            }
        }
    }
}