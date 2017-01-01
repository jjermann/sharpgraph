using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ExampleGraphView.Properties;
using Microsoft.Win32;

namespace ExampleGraphView {
    public partial class MainWindow : INotifyPropertyChanged {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.dot";
            }
            var vm = (GraphController) DataContext;
            if (vm.OpenFileCommand.CanExecute(initialFile)) {
                vm.OpenFileCommand.Execute(initialFile);
            }
            ShowDotOutput = false;
            ShowImageOutput = false;
            ShowDotInput = true;
        }

        private DotOutput DotOutputWindow { get; set; }
        private ImageOutput ImageOutputWindow { get; set; }
        private DotInput DotInputWindow { get; set; }

        private bool _showDotOutput;
        public bool ShowDotOutput {
            get { return _showDotOutput; }
            set {
                _showDotOutput = value;
                if (_showDotOutput) {
                    if (DotOutputWindow == null) {
                        DotOutputWindow = new DotOutput(DataContext);
                        DotOutputWindow.Show();
                    }
                } else {
                    if (DotOutputWindow != null) {
                        DotOutputWindow.Close();
                        DotOutputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool _showImageOutput;
        public bool ShowImageOutput {
            get { return _showImageOutput; }
            set {
                _showImageOutput = value;
                var vm = (GraphController) DataContext;
                if (_showImageOutput) {
                    vm.UpdateCurrentImage = true;
                    if (ImageOutputWindow == null) {
                        ImageOutputWindow = new ImageOutput(DataContext);
                        ImageOutputWindow.Show();
                    }
                } else {
                    vm.UpdateCurrentImage = false;
                    if (ImageOutputWindow != null) {
                        ImageOutputWindow.Close();
                        ImageOutputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool _showDotInput;
        public bool ShowDotInput {
            get { return _showDotInput; }
            set {
                _showDotInput = value;
                if (_showDotInput) {
                    if (DotInputWindow == null) {
                        DotInputWindow = new DotInput(DataContext);
                        DotInputWindow.Show();
                    }
                } else {
                    if (DotInputWindow != null) {
                        DotInputWindow.Close();
                        DotInputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}