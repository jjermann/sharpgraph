using System.Windows.Input;
using Microsoft.Win32;
using SharpGraph.GraphControllerViewModel;

namespace SharpGraph.GraphView {
    public partial class MainWindow {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.dot";
            }
            var vm = (GraphController) DataContext;
            if (vm.OpenFileCommand.CanExecute(initialFile)) {
                vm.OpenFileCommand.Execute(initialFile);
            }
            DotOutputWindow = new DotOutput(DataContext);
            ImageOutputWindow = new ImageOutput(DataContext);
            DotInputWindow = new DotInput(DataContext);

            DotOutputWindow.Show();
            ImageOutputWindow.Show();
            DotInputWindow.Show();
        }

        private DotOutput DotOutputWindow { get; }
        private ImageOutput ImageOutputWindow { get; }
        private DotInput DotInputWindow { get; }

        private RelayCommand _openCommand;
        public ICommand OpenCommand {
            get {
                return _openCommand ?? (_openCommand = new RelayCommand(
                           param => {
                               var vm = (GraphController) DataContext;
                               var filename = FileDialogHandler<OpenFileDialog>.OpenDialog(vm.OriginalInputFile);
                               if ((filename != null) && vm.OpenFileCommand.CanExecute(filename)) {
                                   vm.OpenFileCommand.Execute(filename);
                               }
                           }
                       ));
            }
        }
    }
}