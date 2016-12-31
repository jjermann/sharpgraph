using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph.GraphViewModel;

namespace ExampleGraphView {
    public partial class DotOutput {
        public DotOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private RelayCommand _saveCommand;
        public ICommand SaveCommand {
            get {
                return _saveCommand ?? (_saveCommand = new RelayCommand(
                           param => {
                               var vm = (GraphController) DataContext;
                               var filename = vm.SavedDotFile ?? FileDialogHandler<SaveFileDialog>.OpenDialog();
                               if ((filename != null) && vm.SaveDotFileCommand.CanExecute(filename)) {
                                   vm.SaveDotFileCommand.Execute(filename);
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
                               var filename = FileDialogHandler<SaveFileDialog>.OpenDialog(vm.SavedDotFile);
                               if ((filename != null) && vm.SaveDotFileCommand.CanExecute(filename)) {
                                   vm.SaveDotFileCommand.Execute(filename);
                               }
                           }
                       ));
            }
        }
    }
}