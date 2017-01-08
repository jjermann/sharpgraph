using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph;

namespace ExampleGraphView {
    public partial class DotOutput {
        public DotOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand {
            get {
                return m_saveCommand ?? (m_saveCommand = new RelayCommand(
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

        private RelayCommand m_saveAsCommand;
        public ICommand SaveAsCommand {
            get {
                return m_saveAsCommand ?? (m_saveAsCommand = new RelayCommand(
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