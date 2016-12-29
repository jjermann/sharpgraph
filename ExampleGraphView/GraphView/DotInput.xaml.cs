using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;
using SharpGraph.GraphViewModel;

namespace SharpGraph.GraphView {
    public partial class DotInput {
        public DotInput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private RelayCommand _saveCommand;
        public ICommand SaveCommand {
            get {
                return _saveCommand ?? (_saveCommand = new RelayCommand(
                           param => {
                               var vm = (GraphController) DataContext;
                               var filename = vm.SavedOriginalDotFile
                                              ?? FileDialogHandler<SaveFileDialog>.OpenDialog(vm.OriginalInputFile);
                               if ((filename != null) && vm.SaveOriginalDotFileCommand.CanExecute(filename)) {
                                   vm.SaveOriginalDotFileCommand.Execute(filename);
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
                               var filename = FileDialogHandler<SaveFileDialog>.OpenDialog(
                                   vm.SavedOriginalDotFile ?? vm.OriginalInputFile);
                               if ((filename != null) && vm.SaveOriginalDotFileCommand.CanExecute(filename)) {
                                   vm.SaveOriginalDotFileCommand.Execute(filename);
                               }
                           }
                       ));
            }
        }
    }
}