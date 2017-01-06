using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph.BaseViewModel;

namespace ExampleGraphView {
    public partial class DotInput {
        public DotInput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand {
            get {
                return m_saveCommand ?? (m_saveCommand = new RelayCommand(
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

        private RelayCommand m_saveAsCommand;
        public ICommand SaveAsCommand {
            get {
                return m_saveAsCommand ?? (m_saveAsCommand = new RelayCommand(
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