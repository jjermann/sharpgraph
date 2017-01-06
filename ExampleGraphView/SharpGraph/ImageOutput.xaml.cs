using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph.BaseViewModel;

namespace ExampleGraphView {
    public partial class ImageOutput {
        public ImageOutput(object dataContext) {
            DataContext = dataContext;
            InitializeComponent();
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand {
            get {
                return m_saveCommand ?? (m_saveCommand = new RelayCommand(
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

        private RelayCommand m_saveAsCommand;
        public ICommand SaveAsCommand {
            get {
                return m_saveAsCommand ?? (m_saveAsCommand = new RelayCommand(
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